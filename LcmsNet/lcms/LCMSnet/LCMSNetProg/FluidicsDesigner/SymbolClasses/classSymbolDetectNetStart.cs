
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/25/2009
//
// Last modified 08/25/2009
//						09/03/2009 (DAC) - Modified to obtain default caption from device control
//						09/03/2009 (DAC) - Added group name property for saving/loading config
//						09/18/2009 (DAC) - Added SaveSymbolSettings method for saving config
//						12/02/2009 (DAC) - Added event for handling PAL tray list
//						02/16/2010 (DAC) - Added Dispose method for event handler cleanup
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing;
using LcmsNet.Devices.NetworkStart;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.FluidicsDesigner
{
	class classSymbolDetectNetStart : IDeviceSymbol
	{
		//*********************************************************************************************************
		//Fluidics Designer class for detector triggered by network start signal
		//**********************************************************************************************************

		#region "Delegates"
			/// <summary>
			/// Used internally for setting the caption on a symbol
			/// </summary>
			/// <param name="text"></param>
			private delegate void DelegateSetCaption(string text);
		#endregion

		#region "Class variables"
			Group mobj_Symbol;
			controlNetStart mobj_Device;
			Syncfusion.Windows.Forms.Diagram.Controls.Diagram mobj_Diagram;
		#endregion

		#region "Events"
			public event DelegateOperationError     OperationError;
			public event DelegateNameListReceived   InstrumentMethodListReceived;	
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
		public classSymbolDetectNetStart(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram)
		{
			mobj_Diagram            = TargetDiagram;
			mobj_Device             = new controlNetStart();
			mobj_Device.Emulation   = bool.Parse(classLCMSSettings.GetParameter("EmulationEnabled"));
			mobj_Device.NameChanged                  += new DelegateNameChanged(OnNameChange);
			mobj_Device.SaveRequired                 += new LcmsNetDataClasses.Devices.DelegateSaveRequired(OnSaveRequired);
            mobj_Device.InstrumentMethodListReceived += new DelegateNameListReceived(mobj_Device_InstrumentMethodListReceived);
			try
			{
				CreateSymbolGroup();
			}
			catch (Exception Ex)
			{
				System.Windows.Forms.MessageBox.Show("Exception creating detector" + Ex.Message);
			}
		}

        void mobj_Device_InstrumentMethodListReceived(List<string> trayList)
        {
            if (InstrumentMethodListReceived != null)
                InstrumentMethodListReceived(trayList);
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
					OperationError("Detector already created");
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
			Syncfusion.Windows.Forms.Diagram.Rectangle grpFrame = new Syncfusion.Windows.Forms.Diagram.Rectangle(0, 0, 120, 80);
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
			RectangleF captRect = new RectangleF(0F, 60F, 120F, 20F);
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

			// Top Label
			RectangleF topLblRect = new RectangleF(0F, 0F, 120F, 20F);
			TextNode topLblField = new TextNode("Detector (NS)", topLblRect);
			topLblField.EnableCentralPort = false;
			topLblField.LineStyle.LineColor = Color.Transparent;
			topLblField.FontStyle.Bold = true;
			topLblField.FontStyle.Size = 10;
			topLblField.FontStyle.Family = "Arial";
			topLblField.HorizontalAlignment = StringAlignment.Center;
			topLblField.VerticalAlignment = StringAlignment.Center;
			topLblField.EditStyle.AllowDelete = false;
			topLblField.EditStyle.HidePinPoint = true;
			topLblField.EditStyle.AllowChangeHeight = false;
			topLblField.EditStyle.AllowChangeWidth = false;
			topLblField.EditStyle.HideRotationHandle = true;

			// Main net link
			PointF startPoint = new PointF(5F,40F);
			PointF endPoint = new PointF(115F,40F);
			Syncfusion.Windows.Forms.Diagram.Line mainLink = new Syncfusion.Windows.Forms.Diagram.Line(startPoint,endPoint);
			mainLink.EnableCentralPort = false;
			mainLink.EditStyle.AllowDelete = false;
			mainLink.EditStyle.HidePinPoint = true;
			mainLink.EditStyle.AllowChangeHeight = false;
			mainLink.EditStyle.AllowChangeWidth = false;
			mainLink.EditStyle.HideRotationHandle = true;

			// TopLink
			startPoint = new PointF(60F, 35F);
			endPoint = new PointF(60F, 40F);
			Syncfusion.Windows.Forms.Diagram.Line topLink = new Syncfusion.Windows.Forms.Diagram.Line(startPoint, endPoint);
			topLink.EnableCentralPort = false;
			topLink.EditStyle.AllowDelete = false;
			topLink.EditStyle.HidePinPoint = true;
			topLink.EditStyle.AllowChangeHeight = false;
			topLink.EditStyle.AllowChangeWidth = false;
			topLink.EditStyle.HideRotationHandle = true;

			// Left link
			startPoint = new PointF(30F, 40F);
			endPoint = new PointF(30F, 45F);
			Syncfusion.Windows.Forms.Diagram.Line leftLink = new Syncfusion.Windows.Forms.Diagram.Line(startPoint, endPoint);
			leftLink.EnableCentralPort = false;
			leftLink.EditStyle.AllowDelete = false;
			leftLink.EditStyle.HidePinPoint = true;
			leftLink.EditStyle.AllowChangeHeight = false;
			leftLink.EditStyle.AllowChangeWidth = false;
			leftLink.EditStyle.HideRotationHandle = true;

			// Right link
			startPoint = new PointF(90F, 40F);
			endPoint = new PointF(90F, 45F);
			Syncfusion.Windows.Forms.Diagram.Line rightLink = new Syncfusion.Windows.Forms.Diagram.Line(startPoint, endPoint);
			rightLink.EnableCentralPort = false;
			rightLink.EditStyle.AllowDelete = false;
			rightLink.EditStyle.HidePinPoint = true;
			rightLink.EditStyle.AllowChangeHeight = false;
			rightLink.EditStyle.AllowChangeWidth = false;
			rightLink.EditStyle.HideRotationHandle = true;

			// Top Client
			Syncfusion.Windows.Forms.Diagram.Rectangle topClient = new Syncfusion.Windows.Forms.Diagram.Rectangle(55, 25, 10, 10);
			topClient.FillStyle.Color = Color.Transparent;
			topClient.LineStyle.LineColor = Color.Black;
			topClient.EditStyle.AllowDelete = false;
			topClient.EditStyle.HidePinPoint = true;
			topClient.EditStyle.AllowChangeHeight = false;
			topClient.EditStyle.AllowChangeWidth = false;
			topClient.EditStyle.HideRotationHandle = true;
			topClient.EnableCentralPort = false;

			// Left Client
			Syncfusion.Windows.Forms.Diagram.Rectangle leftClient = new Syncfusion.Windows.Forms.Diagram.Rectangle(25, 45, 10, 10);
			leftClient.FillStyle.Color = Color.Transparent;
			leftClient.LineStyle.LineColor = Color.Black;
			leftClient.EditStyle.AllowDelete = false;
			leftClient.EditStyle.HidePinPoint = true;
			leftClient.EditStyle.AllowChangeHeight = false;
			leftClient.EditStyle.AllowChangeWidth = false;
			leftClient.EditStyle.HideRotationHandle = true;
			leftClient.EnableCentralPort = false;

			// Right Client
			Syncfusion.Windows.Forms.Diagram.Rectangle rightClient = new Syncfusion.Windows.Forms.Diagram.Rectangle(85, 45, 10, 10);
			rightClient.FillStyle.Color = Color.Transparent;
			rightClient.LineStyle.LineColor = Color.Black;
			rightClient.EditStyle.AllowDelete = false;
			rightClient.EditStyle.HidePinPoint = true;
			rightClient.EditStyle.AllowChangeHeight = false;
			rightClient.EditStyle.AllowChangeWidth = false;
			rightClient.EditStyle.HideRotationHandle = true;
			rightClient.EnableCentralPort = false;

			// Create the symbol
			newSymGrp.AppendChild(grpFrame);
			newSymGrp.AppendChild(topLblField);
			newSymGrp.AppendChild(mainLink);
			newSymGrp.AppendChild(topLink);
			newSymGrp.AppendChild(leftLink);
			newSymGrp.AppendChild(rightLink);
			newSymGrp.AppendChild(topClient);
			newSymGrp.AppendChild(leftClient);
			newSymGrp.AppendChild(rightClient);
			newSymGrp.AppendChild(captField);
			newSymGrp.PinPoint = new PointF(60F, 40F);

			// Assign the new symbol to its field
			mobj_Symbol = newSymGrp;
		}	// End sub
		#endregion

        /// <summary>
        /// Forwards instrument method name list received event to subscribed objects
        /// </summary>
        /// <param name="trayList">List of trays found on instrument.</param>
        void OnInstrumentMethodNameListReceived(List<string> methodList)
        {
            if (InstrumentMethodListReceived != null)
                InstrumentMethodListReceived(methodList);
        }

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
			/// Handles SaveRequired event from control
			/// </summary>
			/// <param name="sender"></param>
			void OnSaveRequired(object sender)
			{
				if (SaveRequired != null) SaveRequired(mobj_Device);
			}	// End sub
		#endregion
	}	
}
