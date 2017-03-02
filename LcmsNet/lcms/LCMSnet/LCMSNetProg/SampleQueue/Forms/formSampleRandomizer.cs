//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/29/2009
//
// Last modified 01/29/2009
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNetDataClasses;

namespace LcmsNet.SampleQueue.Forms
{
    public partial class formSampleRandomizer : Form
    {
        #region "Properties"

        public List<classSampleData> OutputSampleList => m_OutputSampleList;
        // End property

        #endregion

        #region "Events"

        public event EventHandler RandomizeComplete;

        # endregion

        #region "Event Handlers"

        /// <summary>
        /// Handles Randomize button event to randomize a list of samples
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRandomize_Click(object sender, EventArgs e)
        {
            // Verify a randomization method has been chosen
            if (comboBoxRandomizers.Text == "")
            {
                statusLabel.Text = "Randomization type must be selected.";
                return;
            }

            statusLabel.Text = "Randomizing input list.";
            RandomizeSamples(m_InputSampleList);
            LoadOuputListview();
            RaiseEventRandomizeComplete();
        }

        #endregion

        //*********************************************************************************************************
        // Form for choosing sample queue randomization technique and randomizing
        //**********************************************************************************************************

        #region "Class variables"

        readonly List<classSampleData> m_InputSampleList;
        List<classSampleData> m_OutputSampleList;
        Dictionary<string, Type> m_DictRandomizers = new Dictionary<string, Type>();

        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        public formSampleRandomizer(List<classSampleData> InputList)
        {
            InitializeComponent();
            m_InputSampleList = InputList;
            InitControls(InputList);
        }

        /// <summary>
        /// Initializes the form controls
        /// </summary>
        /// <param name="InputList">List of samples to be randomized</param>
        void InitControls(List<classSampleData> SampleList)
        {
            // Load list of randomizer types
            comboBoxRandomizers.Items.Clear();
            m_DictRandomizers = classRandomizerPluginTools.GetRandomizerPlugins();

            if (m_DictRandomizers.Count < 1)
                return;

            foreach (var randomizer in m_DictRandomizers.Keys)
            {
                comboBoxRandomizers.Items.Add(randomizer);
            }
            comboBoxRandomizers.SelectedIndex = 0;

            // Load input listview
            foreach (var Sample in m_InputSampleList)
            {
                var NewItem = new ListViewItem(Sample.SequenceID.ToString());
                NewItem.SubItems.Add(Sample.DmsData.DatasetName);
                listViewInput.Items.Add(NewItem);
            }
            buttonOK.Enabled = false;
        }

        /// <summary>
        /// Randomizes the sequence numbers for a list of samples;
        /// stores randomized samples for retrieval via OutputSampleList
        /// </summary>
        /// <param name="InputSamples">List of input samples to be randomized</param>
        void RandomizeSamples(List<classSampleData> InputSamples)
        {
            var randomizerObject = Activator.CreateInstance(m_DictRandomizers[comboBoxRandomizers.Text]);
            var randomizer = randomizerObject as IRandomizerInterface;
            m_OutputSampleList = randomizer.RandomizeSamples(InputSamples);

            statusLabel.Text = "Randomization Complete.";

            buttonOK.Enabled = true;
        }

        /// <summary>
        /// Loads the output listview with the randomized sample list
        /// </summary>
        void LoadOuputListview()
        {
            listViewOutput.Items.Clear();
            foreach (var Sample in m_OutputSampleList)
            {
                var NewItem = new ListViewItem(Sample.SequenceID.ToString());
                NewItem.SubItems.Add(Sample.DmsData.DatasetName);
                listViewOutput.Items.Add(NewItem);
            }
        }

        /// <summary>
        /// Raises the RandomizeComplete event
        /// </summary>
        void RaiseEventRandomizeComplete()
        {
            RandomizeComplete?.Invoke(this, new EventArgs());
        }

        #endregion
    }
} // End namespace