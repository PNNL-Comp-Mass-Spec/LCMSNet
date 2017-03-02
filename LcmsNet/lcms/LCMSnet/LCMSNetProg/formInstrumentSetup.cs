
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 10/18/2010
//
// Last modified 10/18/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetDmsTools;
using LcmsNetDataClasses.Logging;

namespace LcmsNet
{
    public partial class formInstrumentSetup : Form
    {
        //*********************************************************************************************************
        // Form for specifying instrument that is connected to cart
        //**********************************************************************************************************

        #region "Constants"
        #endregion

        #region "Delegates"
        #endregion

        #region "Events"
        #endregion

        #region "Properties"
        #endregion

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
                List<classInstrumentInfo> instList = classSQLiteTools.GetInstrumentList();

                if (instList == null)
                {
                    classApplicationLogger.LogError(0, "formInstrumentSetup: Instrument list retrieval returned null");
                    MessageBox.Show("Instrument list retrieval returned null");
                    return;
                }

                if (instList.Count < 1)
                {
                    classApplicationLogger.LogError(0, "formInstrumentSetup: No instruments found");
                    MessageBox.Show("No instruments found");
                    return;
                }

                comboBoxAvailInstruments.Items.Clear();
                foreach (classInstrumentInfo instData in instList)
                {
                    comboBoxAvailInstruments.Items.Add(instData.DMSName);
                }

                // Determine if presently specified instrument name is in list. If it is, display it.
                string currentName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_INSTNAME);
                int indx = 0;
                bool found = false;
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

        #region "Event handlers
            private void buttonCancel_Click(object sender, EventArgs e)
            {
                this.Close();
            }

            private void buttonAccept_Click(object sender, EventArgs e)
            {
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_INSTNAME, comboBoxAvailInstruments.SelectedItem.ToString());
                this.Close();
            }
        #endregion
    }   // End class
}   // End namespace
