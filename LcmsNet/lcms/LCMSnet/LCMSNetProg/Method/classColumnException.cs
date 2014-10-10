using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Method
{
    public class classColumnException:Exception
    {
        public classColumnException(int columnID, Exception innerEx):base("", innerEx)
        {
            ColumnID = columnID;
            Except = innerEx;
        }

        public int ColumnID
        {
            get;
            private set;
        }

        public Exception Except
        {
            get;
            private set;
        }
    }
}
