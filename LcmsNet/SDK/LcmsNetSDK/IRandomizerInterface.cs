using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNetSDK
{
    /// <summary>
    /// Interface for sample run order randomizer
    /// </summary>
    public interface IRandomizerInterface
    {
        #region "Methods"

        List<SampleData> RandomizeSamples(List<SampleData> InputSampleList);

        #endregion
    }
}
