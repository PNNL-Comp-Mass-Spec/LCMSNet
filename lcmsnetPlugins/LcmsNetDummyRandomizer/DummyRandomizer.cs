﻿using System.Collections;
using System.Collections.Generic;
using LcmsNetSDK;
using LcmsNetSDK.Data;

namespace LcmsNetPlugins.LcmsNetDummyRandomizer
{
    /// <summary>
    /// Plugin for testing randomizer loading. This merely inverts the input list
    /// </summary>
    [PlugInDisplayName("Invert")]
    public class DummyRandomizer : IRandomizerInterface, IEnumerable
    {
        #region "Class variables"
        private readonly List<long> m_Items;
        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        public DummyRandomizer()
        {
            m_Items = new List<long> {0, 1, 2};
        }

        /// <summary>
        /// Implements IRandomizerInterface's RandomizeSamples method
        /// </summary>
        /// <param name="InputSampleList">List containing samples to be randomized</param>
        /// <returns>List containing same samples with sequence numbers randomized</returns>
        public List<SampleData> RandomizeSamples(List<SampleData> InputSampleList)
        {
            // Create a list of the sequence numbers from the input sample list
            var SeqList = GetSeqNumbers(InputSampleList);

            // Invert the sequence number list
            SeqList.Reverse();

            // Reassign the sequence numbers and return
            for (var i = 0; i < InputSampleList.Count; i++)
            {
                InputSampleList[i].SequenceID = SeqList[i];
            }
            return InputSampleList;
        }

        /// <summary>
        /// Implements IEnumerable's GetEnumerator method for returning a list's enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }

        /// <summary>
        /// Retrieves the sequence numbers from input sample list
        /// </summary>
        /// <param name="InpSamples">List of sample data objects</param>
        /// <returns>List of sequence numbers</returns>
        List<long> GetSeqNumbers(List<SampleData> InpSamples)
        {
            var SeqNums = new List<long>();
            foreach (var Sample in InpSamples)
            {
                SeqNums.Add(Sample.SequenceID);
            }
            return SeqNums;
        }
        #endregion
    }
}
