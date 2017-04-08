
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/24/2009
//
// Last modified 08/24/2009
//                      09/03/2009 (DAC) - Modified to obtain default caption from device control
//                      09/03/2009 (DAC) - Added group name property for saving/loading config
//                      09/18/2009 (DAC) - Added SaveSymbolSettings method for saving config
//                      12/02/2009 (DAC) - Added event for handling PAL tray list
//                      02/16/2010 (DAC) - Added Dispose method for event handler cleanup
//*********************************************************************************************************
using System;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing;
using LcmsNet.Devices.ContactClosure;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.FluidicsDesigner
{
    class classSymbolDetectCC : IDeviceSymbol
    {
        //*********************************************************************************************************
        //Fluidics Designer class for detector triggered by contact closure
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
            controlContactClosure mobj_Device;
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
            public classSymbolDetectCC(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram)
            {
                mobj_Diagram = TargetDiagram;
                mobj_Device = new controlContactClosure();
                mobj_Device.Emulation = bool.Parse(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_EMULATIONENABLED));
                mobj_Device.NameChanged += new DelegateNameChanged(OnNameChange);
                mobj_Device.SaveRequired += new LcmsNetDataClasses.Devices.DelegateSaveRequired(OnSaveRequired);
                try
                {
                    CreateSymbolGroup();
                }
                catch (Exception Ex)
                {
                    System.Windows.Forms.MessageBox.Show("Exception creating detector" + Ex.Message);
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
                catch (System.NotImplementedException)
                {
                    string msg = "Unable to load settings from device control";
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
                TextNode topLblField = new TextNode("Detector (CC)", topLblRect);
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

                // Left wire
                PointF startPoint = new PointF(25F,40F);
                PointF endPoint = new PointF(55F,40F);
                Syncfusion.Windows.Forms.Diagram.Line leftWire = new Syncfusion.Windows.Forms.Diagram.Line(startPoint,endPoint);
                leftWire.EnableCentralPort = false;
                leftWire.EditStyle.AllowDelete = false;
                leftWire.EditStyle.HidePinPoint = true;
                leftWire.EditStyle.AllowChangeHeight = false;
                leftWire.EditStyle.AllowChangeWidth = false;
                leftWire.EditStyle.HideRotationHandle = true;

                // Right wire
                startPoint = new PointF(65F, 40F);
                endPoint = new PointF(95F, 40F);
                Syncfusion.Windows.Forms.Diagram.Line rightWire = new Syncfusion.Windows.Forms.Diagram.Line(startPoint, endPoint);
                rightWire.EnableCentralPort = false;
                rightWire.EditStyle.AllowDelete = false;
                rightWire.EditStyle.HidePinPoint = true;
                rightWire.EditStyle.AllowChangeHeight = false;
                rightWire.EditStyle.AllowChangeWidth = false;
                rightWire.EditStyle.HideRotationHandle = true;

                // left contact
                startPoint = new PointF(55F, 30F);
                endPoint = new PointF(55F, 50F);
                Syncfusion.Windows.Forms.Diagram.Line leftContact = new Syncfusion.Windows.Forms.Diagram.Line(startPoint, endPoint);
                leftContact.EnableCentralPort = false;
                leftContact.EditStyle.AllowDelete = false;
                leftContact.EditStyle.HidePinPoint = true;
                leftContact.EditStyle.AllowChangeHeight = false;
                leftContact.EditStyle.AllowChangeWidth = false;
                leftContact.EditStyle.HideRotationHandle = true;

                // Right contact
                startPoint = new PointF(65F, 30F);
                endPoint = new PointF(65F, 50F);
                Syncfusion.Windows.Forms.Diagram.Line rightContact = new Syncfusion.Windows.Forms.Diagram.Line(startPoint, endPoint);
                rightContact.EnableCentralPort = false;
                rightContact.EditStyle.AllowDelete = false;
                rightContact.EditStyle.HidePinPoint = true;
                rightContact.EditStyle.AllowChangeHeight = false;
                rightContact.EditStyle.AllowChangeWidth = false;
                rightContact.EditStyle.HideRotationHandle = true;

                // NC symbol
                startPoint = new PointF(50F,35F);
                endPoint = new PointF(70F, 45F);
                Syncfusion.Windows.Forms.Diagram.Line ncSymbol = new Syncfusion.Windows.Forms.Diagram.Line(startPoint, endPoint);
                ncSymbol.EnableCentralPort = false;
                ncSymbol.EditStyle.AllowDelete = false;
                ncSymbol.EditStyle.HidePinPoint = true;
                ncSymbol.EditStyle.AllowChangeHeight = false;
                ncSymbol.EditStyle.AllowChangeWidth = false;
                ncSymbol.EditStyle.HideRotationHandle = true;

                // Create the symbol
                newSymGrp.AppendChild(grpFrame);
                newSymGrp.AppendChild(topLblField);
                newSymGrp.AppendChild(leftWire);
                newSymGrp.AppendChild(rightWire);
                newSymGrp.AppendChild(leftContact);
                newSymGrp.AppendChild(rightContact);
                newSymGrp.AppendChild(ncSymbol);
                newSymGrp.AppendChild(captField);
                newSymGrp.PinPoint = new PointF(60F, 40F);

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
