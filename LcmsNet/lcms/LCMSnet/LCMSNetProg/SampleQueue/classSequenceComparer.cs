using System;
using System.Collections.Generic;
using LcmsNetDataClasses;

namespace LcmsNet.SampleQueue
{
    public class classSequenceComparer : IComparer<classSampleData>
    {
        #region IComparer<classSampleData> Members

        public int Compare(classSampleData x, classSampleData y)
        {
            return x.SequenceID.CompareTo(y.SequenceID);
        }

        #endregion
    }
}