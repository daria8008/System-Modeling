namespace lab2
{
    public class Model
    {
        private double _tnext, _tcurr;
        private int _event;

        private readonly List<Element> _list;

        public Model(List<Element> elements)
        {
            _list = elements;
        }

        public void Simulate(double time)
        {
            while (_tcurr < time)
            {
                _tnext = Double.MaxValue;
                foreach (var e in _list)
                {
                    if (e.TNext < _tnext)
                    {
                        _tnext = e.TNext;
                        _event = e.Id;
                    }
                }
                Console.WriteLine("\nIt's time for event in " + _list[_event].Name + ", time = " + _tnext);
                foreach (var e in _list)
                {
                    e.DoStatistics(_tnext - _tcurr);
                }
                _tcurr = _tnext;
                foreach (var e in _list)
                {
                    e.TCurr = _tcurr;
                }
                _list[_event].OutAct();
                foreach (var e in _list)
                {
                    if (e.TNext == _tcurr)
                    {
                        e.OutAct();
                    }
                }
                PrintInfo();
            }
            PrintResult();
        }

        public void PrintInfo()
        {
            foreach (var e in _list)
            {
                e.PrintInfo();
            }
        }

        public void PrintResult()
        {
            Console.WriteLine("\nRESULTS:");
            foreach (var e in _list)
            {
                e.PrintResult();
                if (e is Process p)
                {
                    Console.WriteLine($"mean length of queue = {p.MeanQueue / _tcurr}"
                        + $"\nmean workload = {p.TBusy / _tcurr}" 
                        + $"\nfailure probability = {p.Failure / (double)p.Quantity}");
                }
                Console.WriteLine("\n");
            }
        }
    }
}
