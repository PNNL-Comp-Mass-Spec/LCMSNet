﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace serv
{
    class classNetStartMessage
    {
        private enumNetStartMessageTypes mobj_type;
        private int mint_sequence;
        private string mstr_descriptor;
        private List<classNetStartArgument> mobj_arglist;

        public enumNetStartMessageTypes Type
        {
            get { return mobj_type; }
            set { mobj_type = value; }
        }

        public int Sequence
        {
            get { return mint_sequence; }
            set { mint_sequence = value; }
        }

        public string Descriptor
        {
            get { return mstr_descriptor; }
            set { mstr_descriptor = value; }
        }

        public List<classNetStartArgument> ArgList
        {
            get { return mobj_arglist; }
            set { mobj_arglist = value; }
        }
    }
}
