using System;
using System.Collections.Generic;


namespace LcmsNet.Devices.NetworkStart.Socket
{
    class classNetStartMessage
    {
        private enumNetStartMessageTypes m_type;
        private int m_sequence;
        private string mstr_descriptor;
        private List<classNetStartArgument> m_arglist;

        public enumNetStartMessageTypes Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int Sequence
        {
            get { return m_sequence; }
            set { m_sequence = value; }
        }

        public string Descriptor
        {
            get { return mstr_descriptor; }
            set { mstr_descriptor = value; }
        }

        public List<classNetStartArgument> ArgumentList
        {
            get { return m_arglist; }
            set { m_arglist = value; }
        }
    }
}
