namespace lab2
{
    internal class Process : Element
    {
        public int MaxQueue { get; set; } = Int32.MaxValue;
        public int @Queue { get; set; }
        public int Failure { get; private set; }
        public double MeanQueue { get; private set; }
        public double TBusy { get; private set; }

        private int _busyChannels;
        private List<double> _tnexts = new();
        private Dictionary<double, Element> _nextElements = new();

        private readonly int _numOfChannels;

        public Process(double delay, int numOfChannels)
            : base(delay)
        {
            _numOfChannels = numOfChannels;
        }

        public override void InAct()
        {
            if (_busyChannels != _numOfChannels)
            {
                State = 1;
                _busyChannels++;
                AddTnext(TCurr + GetDelay());
                Console.WriteLine("busy channels = " + _busyChannels);
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
            if (Queue > 0)
            {
                Queue--;
                State = 1;
                _busyChannels++;
                AddTnext(TCurr + GetDelay());
            }
            if (HasNextElements())
            {
                Element next = FindNextElement();
                if (next != null)
                {
                    next.InAct();
                }
            }
        }

        public void SetNextElement(Element nextElement, double probability)
        {
            _nextElements[probability] = nextElement;
        }

        private bool HasNextElements()
        {
            return _nextElements.Any();
        }

        private Element FindNextElement()
        {

            if (_nextElements.Count == 1)
            {
                return _nextElements.GetValueOrDefault(1.0);
            }

            var rand = new Random().NextDouble();
            Console.WriteLine("rand = " + rand);
            HashSet<double> probabilities = _nextElements.Keys.ToHashSet();
            double accumulator = 0.0;
            foreach (var p in probabilities)
            {
                if (accumulator < rand && rand < accumulator + p)
                {
                    Console.WriteLine(accumulator + "<" + rand + "<" + (accumulator + p));
                    return _nextElements[p];
                }
                accumulator += p;

            }
            return null;
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
            MeanQueue += + @Queue * delta;
            TBusy += State * delta;
        }
    }
}
