using System.Collections.Generic;
using LcmsNet.Data;
using LcmsNetSDK.Data;

namespace LcmsNet.IO.DMS
{
    public class BlockValidator
    {
        /// <summary>
        /// Validates a list of samples to make sure if they are from the same block, they run on the same column.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static List<SampleData> ValidateBlocks(List<SampleData> samples)
        {
            // First we want to bin the samples based on block number, then we want to
            // figure out for each block if we are scheduled to run on the same column.
            var tempDictionary = new Dictionary<string, List<SampleData>>();
            foreach (var sample in samples)
            {
                var block = sample.DmsData?.Block ?? 0;
                var batch = sample.DmsData?.Batch ?? 0;
                // If the items are blocked, then they need to run on one column.  For batched samples we don't care.
                if (block < 1)
                {
                    continue;
                }

                var key = batch.ToString() + "-" + block.ToString();
                if (tempDictionary.ContainsKey(key))
                {
                    tempDictionary[key].Add(sample);
                }
                else
                {
                    tempDictionary.Add(key, new List<SampleData>() { sample });
                }
            }

            var badSamples = new List<SampleData>();

            // Iterate over the batches
            foreach (var itemKey in tempDictionary.Keys)
            {
                // Iterate over the blocks
                var tempSamples = tempDictionary[itemKey];
                var method = tempSamples[0].LCMethodName;
                var columnID = tempSamples[0].ColumnIndex;

                // Find a mis match between any of the columns. By communicative property
                // we only need to use one of the column id values to do this and perform a
                // O(n) search.
                for (var i = 1; i < tempSamples.Count; i++)
                {
                    // Make sure we also look at the sample method ... this is important.
                    if (tempSamples[i].ColumnIndex != columnID || method != tempSamples[i].LCMethodName)
                    {
                        badSamples.AddRange(tempSamples);
                        break;
                    }
                }
            }

            return badSamples;
        }
    }
}
