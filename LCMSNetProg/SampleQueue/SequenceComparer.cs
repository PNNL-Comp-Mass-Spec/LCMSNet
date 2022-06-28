using System.Collections.Generic;
using LcmsNet.Data;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue
{
    public class SequenceComparer : IComparer<SampleData>
    {
        public int Compare(SampleData x, SampleData y)
        {
            return x.SequenceID.CompareTo(y.SequenceID);
        }
    }
}
