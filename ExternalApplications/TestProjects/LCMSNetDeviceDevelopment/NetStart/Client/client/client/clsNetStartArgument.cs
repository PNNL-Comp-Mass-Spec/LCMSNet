using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client
{
    public class clsNetStartArgument
    {
        private string mstr_key;
        private string mstr_value;
        public string Key
        {
            get { return mstr_key; }
            set { mstr_key = value; }
        }
        public string Value
        {
            get { return mstr_value; }
            set { mstr_value = value; }
        }
    }
}
