using MathNet.Numerics;

namespace lab3_bank
{
    internal class Model
    {
        private double _tnext;
        private double _tcurr;
        private int _event;
        private int[] _queueChangeProcessIndexes;
        private int _queueChangeNumber;

        private readonly List<Element> _list;

        public Model(List<Element> elements)
        {
            _list = elements;
        }

        public void simulate(double time)
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
                Console.WriteLine($"\nIt's time for event in {_list[_event].Name}, time = {_tnext}");
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

                manageQueues();
                foreach (var e in _list)
                {
                    if (e.TNext == _tcurr)
                    {
                        e.OutAct();
                    }
                }
                printInfo();
            }
            printResult();
        }

        public void printInfo()
        {
            foreach (var e in _list)
            {
                e.PrintInfo();
            }
        }

        public void printResult()
        {
            Console.WriteLine("\nRESULTS:");

            double meanClientsInBank = 0.0;
            double meanTimeBetweenLeave = 0.0;
            double meanTimeInBank = 0.0;
            int numOfCashiers = 0;
            double failures = 0;
            double failureProbability = 0.0;

            foreach (var e in _list)
            {
                e.PrintResult();
                if (e is Process p) 
                {
                    Console.WriteLine($"mean length of queue = {p.MeanQueue / _tcurr}"
                            + $"\nmean workload = {p.TBusy / _tcurr}");
                    meanClientsInBank += p.MeanClients;
                    meanTimeBetweenLeave += p.GetMeanTBetweenLeave();
                    meanTimeInBank += p.GetMeanTimeInBank();
                    numOfCashiers++;
                }
                else if (e is Create c)
                {
                    failures = c.Failure;
                    failureProbability = (c.Failure / (double)c.Quantity) * 100;
                }
                Console.WriteLine();
            }
            Console.WriteLine($"mean number of clients in bank =  {meanClientsInBank / _tcurr}");
            Console.WriteLine($"mean time interval between clients leave bank =  {meanTimeBetweenLeave / numOfCashiers}");
            Console.WriteLine($"mean time client spent in bank =  {meanTimeInBank / numOfCashiers}");
            Console.WriteLine($"failure num = {failures}; probability = {Math.Round(failureProbability)}%");
            Console.WriteLine($"queue was changed {_queueChangeNumber} times");
        }

        public void setQueueChangeProcessIndexes(int[] indexes)
        {
            if (indexes.Length == 2)
            {
                _queueChangeProcessIndexes = indexes;
            }
        }

        private void manageQueues()
        {
            var process1 = _list[_queueChangeProcessIndexes[0]];
            var process2 = _list[_queueChangeProcessIndexes[1]];
            var queue1 = ((Process)process1).Queue;
            var queue2 = ((Process)process2).Queue;
            var diff = queue1 - queue2;
            if (Math.Abs(diff) > 1)
            {
                if (diff > 0)
                {
                    ((Process)process1).Queue = queue1 - 1;
                    ((Process)process2).Queue = queue2 + 1;
                }
                else
                {
                    ((Process)process1).Queue = (queue1 + 1);
                    ((Process)process2).Queue = (queue2 - 1);
                }
                _queueChangeNumber++;
            }
        }
    }
}