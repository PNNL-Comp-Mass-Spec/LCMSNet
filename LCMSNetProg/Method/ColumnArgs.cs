using LcmsNet.Data;
using LcmsNetSDK.Data;

namespace LcmsNet.Method
{
    public class ColumnArgs
    {
        public ColumnArgs(SampleData data)
        {
            Sample = data;
        }

        public SampleData Sample { get; }
    }
}