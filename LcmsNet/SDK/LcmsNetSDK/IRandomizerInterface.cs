using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNetSDK
{
    /// <summary>
    /// Inteface for sample run order randomizer
    /// </summary>
    public interface IRandomizerInterface
    {
        #region "Methods"

        List<SampleData> RandomizeSamples(List<SampleData> InputSampleList);

        #endregion
    }
}
