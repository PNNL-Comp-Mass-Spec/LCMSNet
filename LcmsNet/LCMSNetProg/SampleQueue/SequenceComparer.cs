using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue
{
    public class SequenceComparer : IComparer<SampleData>
    {
        #region IComparer<SampleData> Members

        public int Compare(SampleData x, SampleData y)
        {
            return x.SequenceID.CompareTo(y.SequenceID);
        }

        #endregion
    }
}