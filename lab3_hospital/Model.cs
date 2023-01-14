namespace lab3_hospital
{
    public class Model
    {
        private double _tnext;
        private double _tcurr;
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
                        _event = _list.IndexOf(e);
                    }
                }
                Console.WriteLine($"\nIt's time for event in {_list[_event].Name} , time = {_tnext}");
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
            var meanTimeInSystem = new double[] { 0, 0 };
            double meanLabArrivalTime = 0;
            foreach (var e in _list)
            {
                e.PrintResult();
                if (e is Create c)
                {
                    Console.WriteLine("created: " + c.GetNumOfTypes());
                }
                else if (e is Process p)
                {
                    switch (p.GetProcessType())
                    {
                        case ProcessType.ACCOMPANYING_DOCTORS:
                            meanTimeInSystem[0] = p.GetMeanPatientTimeInSystem(PatientType.TYPE1);
                            break;
                        case ProcessType.LABORATORY:
                            meanTimeInSystem[1] = p.GetMeanPatientTimeInSystem(PatientType.TYPE3);
                            break;
                        case ProcessType.REGISTRY:
                            meanLabArrivalTime = p.GetMeanTBetweenArrival();
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine($"mean time interval from arrival to hospital ward = {meanTimeInSystem[0]}");
            Console.WriteLine($"mean time interval from arrival to exit = {meanTimeInSystem[1]}");
            Console.WriteLine($"mean time interval between arrival to laboratory = {meanLabArrivalTime}");

        }
    }
}