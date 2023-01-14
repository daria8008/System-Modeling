namespace lab3_bank
{
    internal class Create : Element
    {
        public int Failure { get; private set; }

        private Dictionary<double, Element> _nextElements = new();

        public Create(double delay) 
          : base(delay)
        {
            TNext = 0;
        }

        public override void OutAct()
        {
            base.OutAct();
            TNext = TCurr + GetDelay();
            var next = FindNextElement();
            if (next != null)
            {
                next.InAct();
            }
        }

        public void SetNextElement(Element nextElement)
        {
            SetNextElement(nextElement, 1);
        }

        public void SetNextElement(Element nextElement, double priority)
        {
            _nextElements[priority] = nextElement;
        }

        private Element? FindNextElement()
        {
            if (_nextElements.Count == 1)
            {
                return _nextElements.GetValueOrDefault(1.0);
            }
            return FindNextPriorityElement();
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
            foreach (var element in _nextElements.Values)
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
            var elementSet = _nextElements.Values;
            int queueAcc = ((Process)elementSet.First()).Queue;
            foreach (var element in elementSet)
            {
                if (((Process)element).Queue != queueAcc)
                {
                    return false;
                }
            }
            return true;
        }

        private Element GetPriorityElement()
        {
            var minPriority = _nextElements.Keys.Min();
            return _nextElements[minPriority];
        }

        private Element GetLeastQueueElement()
        {
            var accQueue = Int32.MaxValue;
            var accElement = new Element();
            foreach (var element in _nextElements.Values)
            {
                int queue = ((Process)element).Queue;
                if (queue < accQueue)
                {
                    accQueue = queue;
                    accElement = element;
                }
            }
            return accElement;
        }
    }
}