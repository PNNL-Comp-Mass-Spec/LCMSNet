
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/28/2009
//
// Updates
// - 03/04/2009 (DAC) - Added display name attribute
//
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections;
using LcmsNetSDK;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Plugin class for doing simple randomization of sample run order
    /// </summary>
    [classPlugInDisplayName("Simple")]
    public class classSampleRandomizer : IRandomizerInterface, IEnumerable
    {
        #region "Class variables"
            List<long> m_Items;
        #endregion

        #region "Methods"
            /// <summary>
            /// Implements IRandomizerInterface's RandomizeSamples method
            /// </summary>
            /// <param name="InputSampleList">List containing samples to be randomized</param>
            /// <returns>List containing same samples with sequence numbers randomized</returns>
            public List<classSampleData> RandomizeSamples(List<classSampleData> InputSampleList)
            {
                // Create a list of the sequence numbers from the input sample list
                var SeqList = GetSeqNumbers(InputSampleList);
                // Randomize the sequence number list
                RandomizeSequenceList(SeqList);
                // Reassign the sequence numbers and return
                for (var Indx = 0; Indx < InputSampleList.Count; Indx++)
                {
                    InputSampleList[Convert.ToInt32(Indx)].SequenceID = m_Items[Convert.ToInt32(Indx)];
                }
                // Return the original sample list, with new sequence assignment
                return InputSampleList;
            }

            /// <summary>
            /// Performs randomization of a list of sample sequence numbers
            /// </summary>
            /// <param name="InpSeqList">List containing sequence numbers</param>
            void RandomizeSequenceList(IEnumerable InpSeqList)
            {
                // Code for this function adapted from section 3.19 of The Microsoft Visual Basic.Net
                // Programmer's Cookbook, by Matthew MacDonald, published by Microsoft Press
                m_Items = new List<long>();
                var RandNumGen = new Random();
                foreach (long SeqID in InpSeqList)
                {
                    // Randomly pick sequence ID's from input list and store them in the return list
                    var NextIndx = RandNumGen.Next(0, m_Items.Count +1);
                    m_Items.Insert(NextIndx, SeqID);
                }
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
