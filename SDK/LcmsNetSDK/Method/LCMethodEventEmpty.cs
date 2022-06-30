namespace LcmsNetSDK.Method
{
    public class LCMethodEventEmpty : ILCMethodEvent
    {
        /// <inheritdoc />
        public void Break()
        {
        }

        /// <inheritdoc />
        public void PassBreakPoint()
        {
        }

        /// <inheritdoc />
        public void IsDone()
        {
            Executing = false;
        }

        /// <inheritdoc />
        public void IsCurrent()
        {
            Executing = true;
        }

        /// <inheritdoc />
        public bool BreakPoint { get; set; }

        /// <inheritdoc />
        public bool Executing { get; private set; }
    }
}
