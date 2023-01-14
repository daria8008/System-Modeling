namespace lab3_bank
{
    public class Process : Element
    {
        public int MaxQueue { get; set; } = Int32.MaxValue;
        public int @Queue { get; set; }
        public int Failure { get; private set; }
        public double MeanClients { get; private set; }
        public double MeanQueue { get; private set; }
        public double TBusy { get; private set; }

        private double _tBetweenLeave;
        private double _tLastLeave;
        private double _tDelay;
        private int _busyChannels;

        private readonly int _numOfChannels;
        private readonly List<double> _tnexts = new();
        private readonly Dictionary<double, Element> nextElements = new();
        private readonly RouteSelectionMethod _routeSelectionMethod;

        public Process(double delay)
          : this(delay, 1, RouteSelectionMethod.NONE)
        {
        }

        public Process(double delay, int numOfChannels)
          : this(delay, numOfChannels, RouteSelectionMethod.NONE)
        {
        }

        public Process(double delay, RouteSelectionMethod routeSelectionMethod)
            : this(delay, 1, routeSelectionMethod)
        {
        }

        public Process(double delay, int numOfChannels, RouteSelectionMethod routeSelectionMethod)
         : base(delay)
        {
            _busyChannels = 0;
            _numOfChannels = numOfChannels;
            _routeSelectionMethod = routeSelectionMethod;
            _tLastLeave = -1;
        }

        public override void InAct()
        {
            if (_busyChannels != _numOfChannels)
            {
                State = 1;
                _busyChannels++;
                double delay = base.GetDelay();
                AddTnext(TCurr + delay);
                _tDelay += delay;
            }
            else
            {
                if (@Queue < MaxQueue)
                {
                    @Queue++;
                }
                else
                {
                    Failure++;
                }
            }
        }

        public override void OutAct()
        {
            base.OutAct();
            _busyChannels--;
            RemoveTnext();
            CheckState();
            ManageTimeBetweenClientsLeave();
            if (Queue > 0)
            {
                Queue--;
                State = 1;
                _busyChannels++;
                var delay = base.GetDelay();
                AddTnext(TCurr + delay);
                _tDelay += delay;
            }
            if (nextElements.Any())
            {
                var next = FindNextElement();
                if (next != null)
                {
                    next.InAct();
                }
            }
        }

        private void AddTnext(double tnext)
        {
            _tnexts.Add(tnext);
            _tnexts.Sort();
            TNext = _tnexts.First();
        }

        private void RemoveTnext()
        {
            if (_tnexts.Any())
            {
                _tnexts.RemoveAt(0);
            }
        }

        private void CheckState()
        {
            State = (_busyChannels == 0 ? 0 : 1);
        }

        private Element? FindNextElement()
        {
            if (nextElements.Count == 1)
            {
                return nextElements[1.0];
            }
            if (_routeSelectionMethod == RouteSelectionMethod.PROBABILITY)
            {
                return FindNextProbabilityElement();
            }
            else if (_routeSelectionMethod == RouteSelectionMethod.PRIORITY)
            {
                return FindNextPriorityElement();
            }
            return null;
        }

        private Element? FindNextProbabilityElement()
        {
            var rand = new Random().NextDouble();
            Console.WriteLine($"rand = {rand}");
            double accumulator = 0.0;
            foreach (var p in nextElements.Keys)
            {
                if (accumulator < rand && rand < accumulator + p)
                {
                    Console.WriteLine(accumulator + "<" + rand + "<" + (accumulator + p));
                    return nextElements[p];
                }
                accumulator += p;
            }
            return null;
        }

        private Element? FindNextPriorityElement()
        {
            if (QueuesAreFull())
            {
                Failure++;
                return null;
            }
            if (QueuesAreEqual())
            {
                return GetPriorityElement();
            }
            return GetLeastQueueElement();
        }

        private bool QueuesAreFull()
        {
            foreach (var element in nextElements.Values)
            {
                if (((Process)element).Queue != ((Process)element).MaxQueue)
                {
                    return false;
                }
            }
            return true;
        }

        private bool QueuesAreEqual()
        {
            var elementSet = nextElements.Values;
            var queueAcc = ((Process)elementSet.First()).Queue;
            foreach (var element in elementSet)
            {
                if (((Process)element).Queue != queueAcc)
                {
                    return false;
                }
            }
            return true;
        }

        private Element GetLeastQueueElement()
        {
            var accQueue = Int32.MaxValue;
            var accElement = new Element();
            foreach (var element in nextElements.Values)
            {
                var queue = ((Process)element).Queue;
                if (queue < accQueue)
                {
                    accQueue = queue;
                    accElement = element;
                }
            }
            return accElement;
        }

        private Element GetPriorityElement()
        {
            var minPriority = nextElements.Keys.Min();
            return nextElements[minPriority];
        }

        private void ManageTimeBetweenClientsLeave()
        {
            if (_tLastLeave == -1)
            {
                _tLastLeave = TCurr;
            }
            else
            {
                double newInterval = TCurr - _tLastLeave;
                _tBetweenLeave += newInterval;
                _tLastLeave = TCurr;
            }
        }

        public void SetNextElement(Element nextElement)
        {
            if (_routeSelectionMethod == RouteSelectionMethod.NONE)
            {
                SetNextElement(nextElement, 1.0);
            }
        }
        public void SetNextElement(Element nextElement, double routeParameter)
        {
            if (_routeSelectionMethod == RouteSelectionMethod.NONE && nextElements.Any())
            {
                nextElements[1.0] = nextElement;
                return;
            }
            if (_routeSelectionMethod == RouteSelectionMethod.PROBABILITY && routeParameter > 1)
            {
                return;
            }
            nextElements[routeParameter] = nextElement;
        }

        public override double TNext
        {
            get
            {
                if (!_tnexts.Any())
                {
                    return Double.MaxValue;
                }
                return _tnexts.First();
            }
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine("failure = " + Failure);
        }

        public override void DoStatistics(double delta)
        {
            MeanQueue += @Queue * delta;
            TBusy = TBusy + State * delta;
            MeanClients += (Queue + _busyChannels) * delta;
            ManageTimeBetweenClientsLeave();
        }

        public double GetMeanTimeInBank() => (MeanQueue + _tDelay) / Quantity;

        public double GetMeanTBetweenLeave() => _tBetweenLeave / (Quantity - 1);
    }
}
