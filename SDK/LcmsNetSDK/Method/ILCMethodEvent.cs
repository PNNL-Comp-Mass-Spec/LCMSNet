namespace LcmsNetSDK.Method
{
    public interface ILCMethodEvent
    {
        /// <summary>
        /// Notify UI that we're stopping simulation on this event as it is a breakpoint
        /// </summary>
        void Break();

        /// <summary>
        /// Notify UI that we're moving passed this breakpoint.
        /// </summary>
        void PassBreakPoint();

        void IsDone();

        void IsCurrent();

        bool BreakPoint { get; set; }

        bool Executing { get; }
    }
}
