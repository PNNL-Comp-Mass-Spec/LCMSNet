
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 07/08/2010
//
// Last modified 07/08/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNet;
using LcmsNet.Devices;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;

namespace BrukerStartTest
{
	public partial class formBSTest : Form
	{
		//*********************************************************************************************************
		// Form for testing Bruker Start class operation
		//**********************************************************************************************************

		#region "Constants"
		#endregion

		#region "Class variables"
			LcmsNet.Devices.BrukerStart.classBrukerStart mobject_BrukerStart;
		#endregion

		#region "Delegates"
		#endregion

		#region "Events"
		#endregion

		#region "Properties"
		#endregion

		#region "Constructors"
			public formBSTest()
			{
				InitializeComponent();

				InitForm();
			}
		#endregion

		#region "Methods"
			private void InitForm()
			{
				string msg = "======================================== Starting program ========================================";
				classTestLogger.LogDebugMessage(msg);

				mobject_BrukerStart = new LcmsNet.Devices.BrukerStart.classBrukerStart();
				//mobject_BrukerStart.Emulation = Properties.Settings.Default.EmulationEnabled;
				mobject_BrukerStart.Error += new EventHandler<LcmsNetDataClasses.Devices.classDeviceErrorArgs>(OnBrukerError);

				msg = "Program initialization completed";
				classTestLogger.LogDebugMessage(msg);
			}	// End sub

			private void GetMethodList()
			{
				classTestLogger.LogDebugMessage("Getting method list");

				List<string> methodNames = mobject_BrukerStart.GetMethods();
				
				if (methodNames == null)
				{
					lblStatus.Text = "Null received while getting methods";
					classTestLogger.LogDebugMessage(lblStatus.Text);
					return;
				}

				if (methodNames.Count < 1)
				{
					lblStatus.Text = "No mehod names found";
					classTestLogger.LogDebugMessage(lblStatus.Text);
					return;
				}

				cboMethodName.Items.Clear();
				foreach (string methodname in methodNames)
				{
					cboMethodName.Items.Add(methodname);
				}

				cboMethodName.SelectedIndex = 0;

				lblStatus.Text = cboMethodName.Items.Count.ToString() + " method names retrieved";
				classTestLogger.LogDebugMessage(lblStatus.Text);
			}	// End sub

			private void StartAcquisition()
			{
				classTestLogger.LogDebugMessage("Starting acquisition");

				if (cboMethodName.Text.Contains("none"))
				{
					lblStatus.Text = "No method selected";
					classTestLogger.LogDebugMessage(lblStatus.Text);
					return;
				}

				if (txtDSName.Text == "")
				{
					lblStatus.Text = "No dataset specified";
					classTestLogger.LogDebugMessage(lblStatus.Text);
					return;
				}

				classSampleData sample = new classSampleData();
				sample.DmsData.RequestName = txtDSName.Text;
				sample.InstrumentData.MethodName = cboMethodName.Text;

				bool result = mobject_BrukerStart.StartAcquisition(10000, sample);

				if (result)
				{
					lblStatus.Text = "Acquisition started";
				}
				else lblStatus.Text = "Acquisition start failed";
				
				classTestLogger.LogDebugMessage(lblStatus.Text);

			}	// End sub

			private void EndAcquisition()
			{
				classTestLogger.LogDebugMessage("Stopping acquistion");

				bool result = mobject_BrukerStart.StopAcquisition(10000);

				if (result)
				{
					lblStatus.Text = "Acquisition stopped";
				}
				else lblStatus.Text = "Acquisition stop failed";

				classTestLogger.LogDebugMessage(lblStatus.Text);

			}	// End sub
		#endregion

		#region "Event handlers"
			private void btnGetMethods_Click(object sender, EventArgs e)
			{
				GetMethodList();
			}	// End sub

			private void btnStartAcq_Click(object sender, EventArgs e)
			{
				StartAcquisition();
			}	// End sub

			private void btnEndAcq_Click(object sender, EventArgs e)
			{
				EndAcquisition();
			}	// End sub

			void OnBrukerError(object sender, LcmsNetDataClasses.Devices.classDeviceErrorArgs e)
			{
				string msg = e.Error;

				if (e.Exception != null) msg += " " + e.Exception.Message;

				classTestLogger.LogDebugMessage(msg);
			}

			private void formBSTest_FormClosing(object sender, FormClosingEventArgs e)
			{
				classTestLogger.LogDebugMessage("Closing program");
			}	// End sub
		#endregion
	}	// End class
}	// End namespace
