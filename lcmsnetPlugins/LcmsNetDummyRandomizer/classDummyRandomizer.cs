//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/04/2009
//
//*********************************************************************************************************

using System.Collections.Generic;
using System.Collections;
using LcmsNetSDK;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Plugin for testing randomizer loading. This merely inverts the input list
    /// </summary>
    [classPlugInDisplayName("Invert")]
    public class classDummyRandomizer : IRandomizerInterface, IEnumerable
    {
        #region "Class variables"
        private readonly List<long> m_Items;
        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        public classDummyRandomizer()
        {
            m_Items = new List<long> {0, 1, 2};
        }

        /// <summary>
        /// Implements IRandomizerInterface's RandomizeSamples method
        /// </summary>
        /// <param name="InputSampleList">List containing samples to be randomized</param>
        /// <returns>List containing same samples with sequence numbers randomized</returns>
        public List<classSampleData> RandomizeSamples(List<classSampleData> InputSampleList)
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
        List<long> GetSeqNumbers(List<classSampleData> InpSamples)
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
