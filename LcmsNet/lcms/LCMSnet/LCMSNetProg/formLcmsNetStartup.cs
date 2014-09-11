
//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
// Last modified 01/16/2009
//*********************************************************************************************************

using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;
using LcmsNetSampleQueue;
using System.Configuration;
using LcmsNetDataClasses;

namespace LCMSNetProg
{
	public partial class formLcmsNetStartup : Form
	{
		//*********************************************************************************************************
		//Startup form
		//**********************************************************************************************************

		#region "Class variabls"
			ILCMSSettingsInterface mobj_LCMSSettings;		
		#endregion

		#region "Event handlers"
		private void timerNextForm_Tick(object sender, EventArgs e)
			{
				timerNextForm.Enabled = false;
				//TODO: Put code here to load next form
				formDMSView nextForm = new formDMSView(mobj_LCMSSettings);
				DialogResult formRetVal =  nextForm.ShowDialog();
				// DAC test code -- delete or disable after testing complete
				if (formRetVal == DialogResult.OK)
				{
					List<classSampleData> sampleList = nextForm.GetNewSamplesDMSView();
					foreach (classSampleData tempSample in sampleList)
					{
						System.Diagnostics.Debug.WriteLine(tempSample.DmsData.RequestID.ToString() + ": " + tempSample.DmsData.RequestName);
					}
					System.Diagnostics.Debug.WriteLine("Clearing form");
					nextForm.ClearForm();
					sampleList = nextForm.GetNewSamplesDMSView();
					foreach (classSampleData tempSample in sampleList)
					{
						System.Diagnostics.Debug.WriteLine(tempSample.DmsData.RequestID.ToString() + ": " + tempSample.DmsData.RequestName);
					}
					System.Diagnostics.Debug.WriteLine("There should be no data since last entry");
				}
				// End DAC test code
			}	// End sub
		#endregion

		#region "Methods"
			public formLcmsNetStartup()
			{
				InitializeComponent();
				InitForm();
			}

			private void InitForm()
			{
				this.Text = "LCMSNet V"         + Application.ProductVersion;
				labelVersion.Text = "Version "  + Application.ProductVersion;

				//Load settings file
				LoadParamsFromSettingsFile();

				//Start timer for loading next form
				timerNextForm.Enabled = true;	// 3 second delay
			}	// End sub

			void LoadParamsFromSettingsFile()
			{
//				mstringdict_SettingsDict = new StringDictionary();
				mobj_LCMSSettings = new classLCMSSettings();
				SettingsPropertyCollection propColl = Properties.Settings.Default.Properties;
				foreach (SettingsProperty currProperty in propColl)
				{
					string propertyName = currProperty.Name;
					string propertyValue = Properties.Settings.Default[propertyName].ToString();
					mobj_LCMSSettings.SetParam(propertyName, propertyValue);
					//mobj_LCMSSettings.SetParam(currProperty.Name, currProperty.
				}
			}	// End sub

		#endregion
	}	// End class
}	// End namespace
