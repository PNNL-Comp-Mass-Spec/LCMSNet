using System;

namespace LcmsNet
{
    public class TackEventArgs : EventArgs
    {
        public TackEventArgs(bool tack)
        {
            Tacked = tack;
        }

        public bool Tacked { get; }
    }
}