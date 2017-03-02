using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNet
{
    public class TackEventArgs : EventArgs
    {
        private bool m_tacked;

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