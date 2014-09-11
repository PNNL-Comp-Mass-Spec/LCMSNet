
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 07/28/2009
//
// Last modified 07/28/2009
//						09/03/2009 (DAC) - Modified to obtain default caption from device control
//						09/03/2009 (DAC) - Added group name property for saving/loading config
//						09/18/2009 (DAC) - Added SaveSymbolSettings method for saving config
//						12/02/2009 (DAC) - Added event and handler for PAL tray list pass-through
//						02/16/2010 (DAC) - Added Dispose method for event handler cleanup
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing;
using LcmsNet.Devices.Pal;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.FluidicsDesigner
{
	class classSymbolPAL : IDeviceSymbol
	{
		//*********************************************************************************************************
		//Fluidics Designer class for PAL
		//**********************************************************************************************************

		#region "Delegates"
			/// <summary>
			/// Used internally for setting the caption on a valve
			/// </summary>
			/// <param name="text"></param>
			private delegate void DelegateSetCaption(string text);
		#endregion

		#region "Class variables"
			Group mobj_Symbol;
			controlPal mobj_Device;
			Syncfusion.Windows.Forms.Diagram.Controls.Diagram mobj_Diagram;
		#endregion

		#region "Events"
		public event DelegateOperationError     OperationError;
		public event DelegateNameListReceived   PalTrayListReceived;
		public event DelegateSaveRequired       SaveRequired;        
		#endregion

		#region "Properties"
			/// <summary>
			/// The symbol that has been created
			/// </summary>
			public Group DwgGroup
			{
				get
				{
					return mobj_Symbol;
				}
			}

			/// <summary>
			/// Device (valve, etc) to be added to the Designer
			/// </summary>
			public IDeviceControl Device
			{
				get
				{
					return mobj_Device;
				}
			}

			/// <summary>
			/// Syncfusion's name for this symbol
			/// </summary>
			public string GroupName { get; set; }
		#endregion

		#region "Constructors"
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="TargetDiagram"></param>
			public classSymbolPAL(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram)
			{
				mobj_Diagram = TargetDiagram;
				mobj_Device = new controlPal();
				mobj_Device.Emulation = bool.Parse(classLCMSSettings.GetParameter("EmulationEnabled"));
				mobj_Device.NameChanged += new DelegateNameChanged(OnNameChange);
				mobj_Device.PalTrayListReceived += new LcmsNetDataClasses.Devices.DelegateNameListReceived(OnPalTrayListReceived);
				mobj_Device.SaveRequired += new LcmsNetDataClasses.Devices.DelegateSaveRequired(OnSaveRequired);
				try
				{
					CreateSymbolGroup();
				}
				catch (Exception Ex)
				{
					System.Windows.Forms.MessageBox.Show("Exception creating PAL" + Ex.Message);
				}
			}
		#endregion

		#region "Methods"
			/// <summary>
			/// Sets text in symbol caption field using a delegate
			/// </summary>
			/// <param name="NewText">New text for field</param>
			private void SetCaption(string NewText)
			{
				if (mobj_Diagram.InvokeRequired)
				{
					DelegateSetCaption d = new DelegateSetCaption(SetCaption);
					mobj_Diagram.Invoke(d, new object[] { NewText });
				}
				else
				{
					TextNode SymTextNode = (TextNode)mobj_Symbol.GetChildByName("Caption");
					SymTextNode.Text = NewText;
				}
			}	// End sub

			/// <summary>
			/// Unsubscribes event handlers
			/// </summary>
			public void Dispose()
			{
				mobj_Device.NameChanged -= OnNameChange;
				mobj_Device.PalTrayListReceived -= OnPalTrayListReceived;
				mobj_Device.SaveRequired -= OnSaveRequired;
			}	// End sub

			/// <summary>
			/// Brings up the device status page
			/// </summary>
			public void ShowProps()
			{
				mobj_Device.ShowProps();
			}

			/// <summary>
			/// Adds the settings for this symbol to the config file
			/// </summary>
			/// <param name="parentName">Name of the parent node to attach this symbol's XML node to</param>
			public void SaveSymbolSettings(string parentName)
			{
				string xPath;

				// Add a couple sample symbol properties
				xPath = "//SymbolName[@Name='" + parentName + "']";
				classConfigTools.CreateElement(xPath, "SampleProperty", "Prop1", "Something");
				classConfigTools.CreateElement(xPath, "SampleProperty", "Prop2", "Different");

				// Import and save the device properties
				try
				{
					classConfigTools.CreateImportedElement(xPath, "DeviceData", mobj_Device.SaveDeviceSettings());
				}
				catch (System.NotImplementedException)
				{
					string msg = "Unable to load settings from device control";
					classConfigTools.CreateElement(xPath, "DeviceData", "ErrMsg", msg);
				}
			}	// End sub

			/// <summary>
			/// Creates the symbol to display on the diagram
			/// </summary>
			private void CreateSymbolGroup()
			{
				// Verify symbol hasn't already been created
				if (mobj_Symbol != null)
				{
					// Symbol already created, so toss an event
					if (OperationError != null)
					{
						OperationError("PAL already created");
					}
					return;
				}

				//Create a new symbol group
				Group newSymGrp = new Group();
				newSymGrp.CanUngroup = false;
				newSymGrp.EditStyle.AllowDelete = false;
				newSymGrp.EditStyle.HidePinPoint = true;
				newSymGrp.EditStyle.AllowChangeHeight = false;
				newSymGrp.EditStyle.AllowChangeWidth = false;
				newSymGrp.EditStyle.HideRotationHandle = true;
				newSymGrp.EnableCentralPort = false;

				// Frame containing symbol group
				Syncfusion.Windows.Forms.Diagram.Rectangle grpFrame = new Syncfusion.Windows.Forms.Diagram.Rectangle(0, 0, 120, 100);
				grpFrame.FillStyle.Color = Color.Transparent;
				grpFrame.LineStyle.LineColor = Color.Transparent;
				grpFrame.EditStyle.AllowDelete = false;
				grpFrame.EditStyle.HidePinPoint = true;
				grpFrame.EditStyle.AllowChangeHeight = false;
				grpFrame.EditStyle.AllowChangeWidth = false;
				grpFrame.EditStyle.HideRotationHandle = true;
				grpFrame.EnableCentralPort = false;
				grpFrame.Name = "Frame";

				// Caption field
				RectangleF captRect = new RectangleF(50F, 5F, 60F, 15F);
				TextNode captField = new TextNode(mobj_Device.Name, captRect);
				captField.Name = "Caption";
				captField.EnableCentralPort = false;
				captField.LineStyle.LineColor = Color.Transparent;
				captField.FontStyle.Bold = true;
				captField.FontStyle.Size = 10;
				captField.FontStyle.Family = "Arial";
				captField.HorizontalAlignment = StringAlignment.Center;
				captField.VerticalAlignment = StringAlignment.Center;
				captField.EditStyle.AllowDelete = false;
				captField.EditStyle.HidePinPoint = true;
				captField.EditStyle.AllowChangeHeight = false;
				captField.EditStyle.AllowChangeWidth = false;
				captField.EditStyle.HideRotationHandle = true;

				// PAL body
				Syncfusion.Windows.Forms.Diagram.Rectangle palBody = new Syncfusion.Windows.Forms.Diagram.Rectangle(5, 25, 105, 20);
				palBody.EnableCentralPort = false;
				palBody.FillStyle.Color = Color.LightGray;
				palBody.EditStyle.AllowDelete = false;
				palBody.EditStyle.HidePinPoint = true;
				palBody.EditStyle.AllowChangeHeight = false;
				palBody.EditStyle.AllowChangeWidth = false;
				palBody.EditStyle.HideRotationHandle = true;

				// PAL carriage
				Syncfusion.Windows.Forms.Diagram.Rectangle palCarriage = new Syncfusion.Windows.Forms.Diagram.Rectangle(20, 5, 20, 85);
				palCarriage.EnableCentralPort = false;
				palCarriage.FillStyle.Color = Color.LightGray;
				palCarriage.EditStyle.AllowDelete = false;
				palCarriage.EditStyle.HidePinPoint = true;
				palCarriage.EditStyle.AllowChangeHeight = false;
				palCarriage.EditStyle.AllowChangeWidth = false;
				palCarriage.EditStyle.HideRotationHandle = true;

				// PAL gauge
				Syncfusion.Windows.Forms.Diagram.Rectangle palGauge = new Syncfusion.Windows.Forms.Diagram.Rectangle(25, 15, 10, 55);
				palGauge.EnableCentralPort = false;
				palGauge.FillStyle.Color = Color.DarkGray;
				palGauge.EditStyle.AllowDelete = false;
				palGauge.EditStyle.HidePinPoint = true;
				palGauge.EditStyle.AllowChangeHeight = false;
				palGauge.EditStyle.AllowChangeWidth = false;
				palGauge.EditStyle.HideRotationHandle = true;

				// Sample drawer assembly
				Syncfusion.Windows.Forms.Diagram.Rectangle palDwrAssmbly = new Syncfusion.Windows.Forms.Diagram.Rectangle(60, 50, 40, 35);
				palDwrAssmbly.EnableCentralPort = false;
				palDwrAssmbly.FillStyle.Color = Color.LightGray;
				palDwrAssmbly.EditStyle.AllowDelete = false;
				palDwrAssmbly.EditStyle.HidePinPoint = true;
				palDwrAssmbly.EditStyle.AllowChangeHeight = false;
				palDwrAssmbly.EditStyle.AllowChangeWidth = false;
				palDwrAssmbly.EditStyle.HideRotationHandle = true;

				// Drawer 1
				Syncfusion.Windows.Forms.Diagram.Rectangle palDwr1 = new Syncfusion.Windows.Forms.Diagram.Rectangle(65, 55, 30, 5);
				palDwr1.EnableCentralPort = false;
				palDwr1.FillStyle.Color = Color.DarkGray;
				palDwr1.EditStyle.AllowDelete = false;
				palDwr1.EditStyle.HidePinPoint = true;
				palDwr1.EditStyle.AllowChangeHeight = false;
				palDwr1.EditStyle.AllowChangeWidth = false;
				palDwr1.EditStyle.HideRotationHandle = true;

				// Drawer 2
				Syncfusion.Windows.Forms.Diagram.Rectangle palDwr2 = new Syncfusion.Windows.Forms.Diagram.Rectangle(65, 65, 30, 5);
				palDwr2.EnableCentralPort = false;
				palDwr2.FillStyle.Color = Color.DarkGray;
				palDwr2.EditStyle.AllowDelete = false;
				palDwr2.EditStyle.HidePinPoint = true;
				palDwr2.EditStyle.AllowChangeHeight = false;
				palDwr2.EditStyle.AllowChangeWidth = false;
				palDwr2.EditStyle.HideRotationHandle = true;

				// Drawer 3
				Syncfusion.Windows.Forms.Diagram.Rectangle palDwr3 = new Syncfusion.Windows.Forms.Diagram.Rectangle(65, 75, 30, 5);
				palDwr3.EnableCentralPort = false;
				palDwr3.FillStyle.Color = Color.DarkGray;
				palDwr3.EditStyle.AllowDelete = false;
				palDwr3.EditStyle.HidePinPoint = true;
				palDwr3.EditStyle.AllowChangeHeight = false;
				palDwr3.EditStyle.AllowChangeWidth = false;
				palDwr3.EditStyle.HideRotationHandle = true;
				// Port 1
				Ellipse port1 = new Ellipse(23, 83, 14, 14);
				port1.EnableCentralPort = false;
				port1.FillStyle.Color = Color.Yellow;
				port1.EditStyle.AllowDelete = false;
				port1.EditStyle.HidePinPoint = true;
				port1.EditStyle.AllowChangeHeight = false;
				port1.EditStyle.AllowChangeWidth = false;
				port1.EditStyle.HideRotationHandle = true;

				// Connection point 1
				ConnectionPoint cp1 = new ConnectionPoint();
				cp1.OffsetX = 30F;
				cp1.OffsetY = 90F;

				// Create the symbol
				newSymGrp.AppendChild(grpFrame);
				newSymGrp.AppendChild(palBody);
				newSymGrp.AppendChild(palCarriage);
				newSymGrp.AppendChild(palGauge);
				newSymGrp.AppendChild(palDwrAssmbly);
				newSymGrp.AppendChild(palDwr1);
				newSymGrp.AppendChild(palDwr2);
				newSymGrp.AppendChild(palDwr3);
				newSymGrp.AppendChild(port1);
				newSymGrp.Ports.AddRange(new ConnectionPoint[] { cp1 });
				newSymGrp.AppendChild(captField);
				newSymGrp.PinPoint = new PointF(60F, 50F);

				// Assign the new symbol to its field
				mobj_Symbol = newSymGrp;
			}	// End sub
		#endregion

		#region "Event handlers"
			/// <summary>
			/// Handles the control name changing event via a delegate call
			/// </summary>
			/// <param name="NewName"></param>
			public void OnNameChange(object Sender, string NewName)
			{
				SetCaption(NewName);
			}	// End sub

			/// <summary>
			/// Forwards PAL tray list received event to subscribed objects
			/// </summary>
			/// <param name="trayList">List of trays found on PAL</param>
			void OnPalTrayListReceived(List<string> trayList)
			{
				if (PalTrayListReceived != null) 
                    PalTrayListReceived(trayList);
			}	// End sub

			/// <summary>
			/// Handles SaveRequired event from control
			/// </summary>
			/// <param name="sender"></param>
			void OnSaveRequired(object sender)
			{
				if (SaveRequired != null) SaveRequired(mobj_Device);
			}	// End sub
		#endregion
	}	// End class
}	// End namespace
