namespace lab3_bank
{
    public class Element
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double DelayMean { get; set; }
        public double DelayDev { get; set; }
        public int State { get; set; }
        public string Distribution { get; set; }
        public Element NextElement { get; set; }
        public virtual double TNext { get; set; }
        public int Quantity { get; private set; }
        public double TCurr { get; set; }

        private static int NextId = 0;

        public Element()
        {
            TNext = Double.MaxValue;
            DelayMean = 1.0;
            Distribution = "exp";
            TCurr = TNext;
            Id = NextId;
            NextId++;
            Name = "element" + Id;
        }

        public Element(double delay)
        {
            DelayMean = delay;
            Distribution = "";
            TCurr = TNext;
            Id = NextId;
            NextId++;
            Name = "element" + Id;
        }

        public double GetDelay() => Distribution switch
        {
            "exp" => FunRand.Exp(DelayMean),
            "norm" => FunRand.Norm(DelayMean, DelayDev),
            "unif" => FunRand.Unif(DelayMean, DelayDev),
            _ => DelayMean
        };

        public virtual void InAct()
        {
        }

        public virtual void OutAct()
        {
            Quantity++;
        }
        
        public void PrintResult() => Console.WriteLine($"{Name} quantity={Quantity}");
        public virtual void PrintInfo() => Console.WriteLine($"{Name} state={State} quantity={Quantity} tnext={TNext}");

        public virtual void DoStatistics(double delta)
        {
        }
    }
}
