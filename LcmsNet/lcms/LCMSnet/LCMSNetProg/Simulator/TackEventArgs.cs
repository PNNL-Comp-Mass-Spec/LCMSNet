using System;

namespace LcmsNet
{
    public class TackEventArgs : EventArgs
    {
        private readonly bool m_tacked;

        public TackEventArgs(bool tack)
        {
            m_tacked = tack;
        }

        public bool Tacked
        {
            get { return m_tacked; }
        }
    }
}