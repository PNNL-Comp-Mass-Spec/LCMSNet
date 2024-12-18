﻿using System;

namespace LcmsNet.Method
{
    public class ColumnException : Exception
    {
        public ColumnException(int columnID, Exception innerEx) : base("", innerEx)
        {
            ColumnID = columnID;
            Except = innerEx;
        }

        public int ColumnID { get; }

        public Exception Except { get; }
    }
}
