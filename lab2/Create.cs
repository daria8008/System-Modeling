namespace lab2
{
    internal class Create : Element
    {
        public Create(double delay) 
          : base(delay)
        {
            TNext = 0;
        }

        public override void OutAct()
        {
            base.OutAct();
            TNext = TCurr + GetDelay();
            NextElement.InAct();
        }
    }
}
