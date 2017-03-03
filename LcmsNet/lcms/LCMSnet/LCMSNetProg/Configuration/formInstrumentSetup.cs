//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 10/18/2010
//
// Last modified 10/18/2010
//*********************************************************************************************************

using System;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetSQLiteTools;

namespace LcmsNet
{
    /// <summary>
    /// Form for specifying instrument that is connected to cart
    /// </summary>
    public partial class formInstrumentSetup : Form
    {
        #region "Constructors"

        public formInstrumentSetup()
        {
            InitializeComponent();

            InitForm();
        }

        #endregion

        #region "Methods"

        private void InitForm()
        {
            // Load combo box
            var instList = classSQLiteTools.GetInstrumentList(false);

            if (instList == null)
            {
                classApplicationLogger.LogError(0, "formInstrumentSetup: Instrument list retrieval returned null");
                return;
            }

            if (instList.Count < 1)
            {
                classApplicationLogger.LogError(0, "formInstrumentSetup: No instruments found");
                return;
            }

            comboBoxAvailInstruments.Items.Clear();
            foreach (var instData in instList)
            {
                comboBoxAvailInstruments.Items.Add(instData.DMSName);
            }

            // Determine if presently specified instrument name is in list. If it is, display it.
            var currentName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_INSTNAME);
            var indx = 0;
            var found = false;
            foreach (string itemName in comboBoxAvailInstruments.Items)
            {
                if (itemName == currentName)
                {
                    found = true;
                    break;
                }
                indx++;
            }

            if (found)
            {
                comboBoxAvailInstruments.SelectedIndex = indx;
            }
            else comboBoxAvailInstruments.SelectedIndex = 0;
        }

        #endregion

        #region "Constants"

        #endregion

        #region "Delegates"

        #endregion

        #region "Events"

        #endregion

        #region "Properties"

        #endregion

        #region "Event handlers

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            classLCMSSettings.SetParameter(classLCMSSettings.PARAM_INSTNAME, comboBoxAvailInstruments.SelectedItem.ToString());
            Close();
        }

        #endregion
    }
}