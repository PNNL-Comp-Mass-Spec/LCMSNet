namespace LcmsNetSDK.Method
{
    internal class LCMethodEventEmpty : ILCMethodEvent
    {
        public void Break()
        {
        }

        public void PassBreakPoint()
        {
        }

        public void IsDone()
        {
            Executing = false;
        }

        public void IsCurrent()
        {
            Executing = true;
        }

        public bool BreakPoint { get; set; }
        public bool Executing { get; private set; }
    }
}
