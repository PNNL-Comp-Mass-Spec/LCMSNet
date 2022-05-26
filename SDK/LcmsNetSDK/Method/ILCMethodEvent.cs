namespace LcmsNetSDK.Method
{
    public interface ILCMethodEvent
    {
        void Break();

        void PassBreakPoint();

        void IsDone();

        void IsCurrent();

        bool BreakPoint { get; set; }

        bool Executing { get; }
    }
}
