
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/13/2009
//
// Last modified 08/13/2009
//                      09/03/2009 (DAC) - Modified to obtain default caption from device control
//                      09/03/2009 (DAC) - Added group name property for saving/loading config
//                      09/18/2009 (DAC) - Added SaveSymbolSettings method for saving config
//                      12/02/2009 (DAC) - Added event for handling PAL tray list
//                      02/16/2010 (DAC) - Added Dispose method for event handler cleanup
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing;
using LcmsNet.Devices.Pumps;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.FluidicsDesigner
{
    class classSymbolAgilentPump : IDeviceSymbol
    {
        //*********************************************************************************************************
        //Fluidics Designer class for Agilent pump
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
            controlPumpAgilent mobj_Device;
            Syncfusion.Windows.Forms.Diagram.Controls.Diagram mobj_Diagram;
        #endregion

        #region "Events"
            public event DelegateOperationError OperationError;
            public event DelegateSaveRequired SaveRequired;
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
            public classSymbolAgilentPump(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram)
            {
                mobj_Diagram = TargetDiagram;
                mobj_Device = new controlPumpAgilent();

                mobj_Device.Emulation = bool.Parse(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_EMULATIONENABLED));
                mobj_Device.NameChanged += new DelegateNameChanged(OnNameChange);
                mobj_Device.SaveRequired += new LcmsNetDataClasses.Devices.DelegateSaveRequired(OnSaveRequired);
                try
                {
                    CreateSymbolGroup();
                }
                catch (Exception Ex)
                {
                    System.Windows.Forms.MessageBox.Show("Exception creating Agilent pump" + Ex.Message);
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
            }

            /// <summary>
            /// Unsubscribes event handlers
            /// </summary>
            public void Dispose()
            {
                mobj_Device.NameChanged -= OnNameChange;
                mobj_Device.SaveRequired -= OnSaveRequired;
            }

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
                catch (NotImplementedException)
                {
                    string msg = "The device load method was not implemented.";
                    classConfigTools.CreateElement(xPath, "DeviceData", "ErrMsg", msg);
                }
            }

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
                Syncfusion.Windows.Forms.Diagram.Rectangle grpFrame = new Syncfusion.Windows.Forms.Diagram.Rectangle(0, 0, 80, 120);
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
                RectangleF captRect = new RectangleF(0F, 5F, 80F, 20F);
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

                // Top block
                Syncfusion.Windows.Forms.Diagram.Rectangle blockTop = new Syncfusion.Windows.Forms.Diagram.Rectangle(0, 0, 80, 30);
                blockTop.EnableCentralPort = false;
                blockTop.FillStyle.Color = Color.LightGray;
                blockTop.EditStyle.AllowDelete = false;
                blockTop.EditStyle.HidePinPoint = true;
                blockTop.EditStyle.AllowChangeHeight = false;
                blockTop.EditStyle.AllowChangeWidth = false;
                blockTop.EditStyle.HideRotationHandle = true;

                // Middle block
                Syncfusion.Windows.Forms.Diagram.Rectangle blockMiddle = new Syncfusion.Windows.Forms.Diagram.Rectangle(0, 40, 80, 30);
                blockMiddle.EnableCentralPort = false;
                blockMiddle.FillStyle.Color = Color.LightGray;
                blockMiddle.EditStyle.AllowDelete = false;
                blockMiddle.EditStyle.HidePinPoint = true;
                blockMiddle.EditStyle.AllowChangeHeight = false;
                blockMiddle.EditStyle.AllowChangeWidth = false;
                blockMiddle.EditStyle.HideRotationHandle = true;

                // Bottom block
                Syncfusion.Windows.Forms.Diagram.Rectangle blockBottom = new Syncfusion.Windows.Forms.Diagram.Rectangle(0, 80, 80, 40);
                blockBottom.EnableCentralPort = false;
                blockBottom.FillStyle.Color = Color.LightGray;
                blockBottom.EditStyle.AllowDelete = false;
                blockBottom.EditStyle.HidePinPoint = true;
                blockBottom.EditStyle.AllowChangeHeight = false;
                blockBottom.EditStyle.AllowChangeWidth = false;
                blockBottom.EditStyle.HideRotationHandle = true;

                // Left block
                RectangleF leftBlockBounds = new RectangleF(0F, 90F, 25F, 23F);
                Syncfusion.Windows.Forms.Diagram.RoundRect blockLeft = new Syncfusion.Windows.Forms.Diagram.RoundRect(leftBlockBounds,5F);
                blockLeft.EnableCentralPort = false;
                blockLeft.FillStyle.Color = Color.Black;
                blockLeft.EditStyle.AllowDelete = false;
                blockLeft.EditStyle.HidePinPoint = true;
                blockLeft.EditStyle.AllowChangeHeight = false;
                blockLeft.EditStyle.AllowChangeWidth = false;
                blockLeft.EditStyle.HideRotationHandle = true;

                // Right block
                RectangleF rightBlockBounds = new RectangleF(55F, 90F, 25F, 23F);
                Syncfusion.Windows.Forms.Diagram.RoundRect blockRight = new Syncfusion.Windows.Forms.Diagram.RoundRect(rightBlockBounds, 5F);
                blockRight.EnableCentralPort = false;
                blockRight.FillStyle.Color = Color.Black;
                blockRight.EditStyle.AllowDelete = false;
                blockRight.EditStyle.HidePinPoint = true;
                blockRight.EditStyle.AllowChangeHeight = false;
                blockRight.EditStyle.AllowChangeWidth = false;
                blockRight.EditStyle.HideRotationHandle = true;

                // Port 1
                Ellipse port1 = new Ellipse(33, 106, 14, 14);
                port1.EnableCentralPort = false;
                port1.FillStyle.Color = Color.Yellow;
                port1.EditStyle.AllowDelete = false;
                port1.EditStyle.HidePinPoint = true;
                port1.EditStyle.AllowChangeHeight = false;
                port1.EditStyle.AllowChangeWidth = false;
                port1.EditStyle.HideRotationHandle = true;

                // Connection point 1
                ConnectionPoint cp1 = new ConnectionPoint();
                cp1.OffsetX = 40F;
                cp1.OffsetY = 113F;

                // Create the symbol
                newSymGrp.AppendChild(grpFrame);
                newSymGrp.AppendChild(blockTop);
                newSymGrp.AppendChild(blockMiddle);
                newSymGrp.AppendChild(blockBottom);
                newSymGrp.AppendChild(blockLeft);
                newSymGrp.AppendChild(blockRight);
                newSymGrp.AppendChild(port1);
                newSymGrp.Ports.AddRange(new ConnectionPoint[] { cp1 });
                newSymGrp.AppendChild(captField);
                newSymGrp.PinPoint = new PointF(40F, 60F);

                // Assign the new symbol to its field
                mobj_Symbol = newSymGrp;
            }
        #endregion

        #region "Event handlers"
            /// <summary>
            /// Handles the control name changing event via a delegate call
            /// </summary>
            /// <param name="NewName"></param>
            public void OnNameChange(object Sender, string NewName)
            {
                SetCaption(NewName);
            }

            /// <summary>
            /// Handles SaveRequired event from control
            /// </summary>
            /// <param name="sender"></param>
            void OnSaveRequired(object sender)
            {
                if (SaveRequired != null) SaveRequired(mobj_Device);
            }
        #endregion
    }
}
