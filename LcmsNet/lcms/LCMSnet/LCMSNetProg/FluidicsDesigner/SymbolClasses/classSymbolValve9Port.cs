//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 010/01/2009
//
// Last modified 10/01/2009
//                      12/02/2009 (DAC) - Added event for handling PAL tray list
//                      02/16/2010 (DAC) - Added Dispose method for event handler cleanup
//*********************************************************************************************************
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing;
using LcmsNet.Devices.Valves;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.FluidicsDesigner
{
    public class classSymbolValve9Port : IDeviceSymbol
    {
        //*********************************************************************************************************
        // Fluidics Destigner class for 9-port, multi-position valve
        //**********************************************************************************************************

        #region "Delegates"
            /// <summary>
            /// Used internally for setting the caption on a valve
            /// </summary>
            /// <param name="text"></param>
            protected delegate void DelegateSetCaption(string text);

            /// <summary>
            /// Used internally for updating position display on a valve
            /// </summary>
            /// <param name="NewPosition">New caption for device</param>
            protected delegate void DelegateSetPositionDisplay(string NewPosition);
        #endregion

        #region "Class variables"
            protected Group mobj_Symbol;
            protected controlValveVICIMultiPos mobj_Device;
            protected Syncfusion.Windows.Forms.Diagram.Controls.Diagram mobj_Diagram;
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

        #region "Constructor"
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="TargetDiagram"></param>
            public classSymbolValve9Port(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram)
            {
                mobj_Diagram = TargetDiagram;
                mobj_Device = new controlValveVICIMultiPos();
                mobj_Device.Emulation = bool.Parse(classLCMSSettings.GetParameter("EmulationEnabled"));
                mobj_Device.NameChanged += new DelegateNameChanged(OnNameChange);
                mobj_Device.PositionChanged += new DelegatePositionChanged(OnPositionChange);
                mobj_Device.SaveRequired += new LcmsNetDataClasses.Devices.DelegateSaveRequired(OnSaveRequired);
                CreateSymbolGroup();
            }   // end sub
        #endregion

        #region "Methods"
            /// <summary>
            /// Sets text in symbol caption field using a delegate
            /// </summary>
            /// <param name="NewText">New text for field</param>
            protected void SetCaption(string NewText)
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
            }   // End sub

            /// <summary>
            /// Unsubscribes event handlers
            /// </summary>
            public void Dispose()
            {
                mobj_Device.NameChanged -= OnNameChange;
                mobj_Device.PositionChanged -= OnPositionChange;
                mobj_Device.SaveRequired -= OnSaveRequired;
            }   // End sub

            /// <summary>
            /// Sets text in valve position field using a delegate
            /// </summary>
            /// <param name="NewPosition">Position to display</param>
            protected void SetPositionDisplay(string NewPosition)
            {
                if (mobj_Diagram.InvokeRequired)
                {
                    DelegateSetPositionDisplay d = new DelegateSetPositionDisplay(SetPositionDisplay);
                    mobj_Diagram.Invoke(d, new object[] { NewPosition });
                }
                else
                {
                    TextNode SymPositionNode = (TextNode)mobj_Symbol.GetChildByName("Position");
                    string vlvPosition = "POS: " + NewPosition;
                    SymPositionNode.Text = vlvPosition;
                }
            }   // End sub

            /// <summary>
            /// Brings up the device status page
            /// </summary>
            public void ShowProps()
            {
                mobj_Device.ShowProps();
            }

            /// <summary>
            /// Creates the symbol to display on the diagram
            /// </summary>
            public void CreateSymbolGroup()
            {
                // Verify symbol hasn't already been created
                if (mobj_Symbol != null)
                {
                    // Symbol already created, so toss an event
                    if (OperationError != null)
                    {
                        OperationError("Valve already created");
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
                RectangleF captRect = new RectangleF(0F, 0F, 80F, 20F);
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

                // Position field
                RectangleF posRect = new RectangleF(0F, 100F, 80F, 20F);
                TextNode posField = new TextNode("POS: A", posRect);
                posField.Name = "Position";
                posField.EnableCentralPort = false;
                posField.LineStyle.LineColor = Color.Transparent;
                posField.FontStyle.Bold = true;
                posField.FontStyle.Size = 10;
                posField.FontStyle.Family = "Arial";
                posField.HorizontalAlignment = StringAlignment.Center;
                posField.VerticalAlignment = StringAlignment.Center;
                posField.EditStyle.AllowDelete = false;
                posField.EditStyle.HidePinPoint = true;
                posField.EditStyle.AllowChangeHeight = false;
                posField.EditStyle.AllowChangeWidth = false;
                posField.EditStyle.HideRotationHandle = true;

                // Valve body
                Ellipse vlvBody = new Ellipse(0, 20, 80, 80);
                vlvBody.EnableCentralPort = false;
                vlvBody.FillStyle.Color = Color.LightGray;
                vlvBody.EditStyle.AllowDelete = false;
                vlvBody.EditStyle.HidePinPoint = true;
                vlvBody.EditStyle.AllowChangeHeight = false;
                vlvBody.EditStyle.AllowChangeWidth = false;
                vlvBody.EditStyle.HideRotationHandle = true;

                // Port 1
                Ellipse port1 = new Ellipse(33, 22, 14, 14);
                port1.EnableCentralPort = false;
                port1.FillStyle.Color = Color.Yellow;
                port1.EditStyle.AllowDelete = false;
                port1.EditStyle.HidePinPoint = true;
                port1.EditStyle.AllowChangeHeight = false;
                port1.EditStyle.AllowChangeWidth = false;
                port1.EditStyle.HideRotationHandle = true;

                // Port 2
                Ellipse port2 = new Ellipse(33, 84, 14, 14);
                port2.EnableCentralPort = false;
                port2.FillStyle.Color = Color.Yellow;
                port2.EditStyle.AllowDelete = false;
                port2.EditStyle.HidePinPoint = true;
                port2.EditStyle.AllowChangeHeight = false;
                port2.EditStyle.AllowChangeWidth = false;
                port2.EditStyle.HideRotationHandle = true;

                // Port 3
                Ellipse port3 = new Ellipse(2, 53, 14, 14);
                port3.EnableCentralPort = false;
                port3.FillStyle.Color = Color.Yellow;
                port3.EditStyle.AllowDelete = false;
                port3.EditStyle.HidePinPoint = true;
                port3.EditStyle.AllowChangeHeight = false;
                port3.EditStyle.AllowChangeWidth = false;
                port3.EditStyle.HideRotationHandle = true;

                // Port 4
                Ellipse port4 = new Ellipse(64, 53, 14, 14);
                port4.EnableCentralPort = false;
                port4.FillStyle.Color = Color.Yellow;
                port4.EditStyle.AllowDelete = false;
                port4.EditStyle.HidePinPoint = true;
                port4.EditStyle.AllowChangeHeight = false;
                port4.EditStyle.AllowChangeWidth = false;
                port4.EditStyle.HideRotationHandle = true;

                // Port 5
                Ellipse port5 = new Ellipse(55, 31, 14, 14);
                port5.EnableCentralPort = false;
                port5.FillStyle.Color = Color.Yellow;
                port5.EditStyle.AllowDelete = false;
                port5.EditStyle.HidePinPoint = true;
                port5.EditStyle.AllowChangeHeight = false;
                port5.EditStyle.AllowChangeWidth = false;
                port5.EditStyle.HideRotationHandle = true;

                // Port 6
                Ellipse port6 = new Ellipse(55, 75, 14, 14);
                port6.EnableCentralPort = false;
                port6.FillStyle.Color = Color.Yellow;
                port6.EditStyle.AllowDelete = false;
                port6.EditStyle.HidePinPoint = true;
                port6.EditStyle.AllowChangeHeight = false;
                port6.EditStyle.AllowChangeWidth = false;
                port6.EditStyle.HideRotationHandle = true;

                // Port 7
                Ellipse port7 = new Ellipse(11, 75, 14, 14);
                port7.EnableCentralPort = false;
                port7.FillStyle.Color = Color.Yellow;
                port7.EditStyle.AllowDelete = false;
                port7.EditStyle.HidePinPoint = true;
                port7.EditStyle.AllowChangeHeight = false;
                port7.EditStyle.AllowChangeWidth = false;
                port7.EditStyle.HideRotationHandle = true;

                // Port 8
                Ellipse port8 = new Ellipse(11, 31, 14, 14);
                port8.EnableCentralPort = false;
                port8.FillStyle.Color = Color.Yellow;
                port8.EditStyle.AllowDelete = false;
                port8.EditStyle.HidePinPoint = true;
                port8.EditStyle.AllowChangeHeight = false;
                port8.EditStyle.AllowChangeWidth = false;
                port8.EditStyle.HideRotationHandle = true;

                // Port 9
                Ellipse port9 = new Ellipse(33, 53, 14, 14);
                port9.EnableCentralPort = false;
                port9.FillStyle.Color = Color.Yellow;
                port9.EditStyle.AllowDelete = false;
                port9.EditStyle.HidePinPoint = true;
                port9.EditStyle.AllowChangeHeight = false;
                port9.EditStyle.AllowChangeWidth = false;
                port9.EditStyle.HideRotationHandle = true;

                // Connection point 1
                ConnectionPoint cp1 = new ConnectionPoint();
                cp1.OffsetX = 40F;
                cp1.OffsetY = 29F;

                // Connection point 2
                ConnectionPoint cp2 = new ConnectionPoint();
                cp2.OffsetX = 40F;
                cp2.OffsetY = 91F;

                // Connection point 3
                ConnectionPoint cp3 = new ConnectionPoint();
                cp3.OffsetX = 9F;
                cp3.OffsetY = 60F;

                // Connection point 4
                ConnectionPoint cp4 = new ConnectionPoint();
                cp4.OffsetX = 71F;
                cp4.OffsetY = 60F;

                // Connection point 5
                ConnectionPoint cp5 = new ConnectionPoint();
                cp5.OffsetX = 62F;
                cp5.OffsetY = 38F;

                // Connection point 6
                ConnectionPoint cp6 = new ConnectionPoint();
                cp6.OffsetX = 62F;
                cp6.OffsetY = 82F;

                // Connection point 7
                ConnectionPoint cp7 = new ConnectionPoint();
                cp7.OffsetX = 18F;
                cp7.OffsetY = 82F;

                // Connection point 8
                ConnectionPoint cp8 = new ConnectionPoint();
                cp8.OffsetX = 18F;
                cp8.OffsetY = 38F;

                // Connection point 9
                ConnectionPoint cp9 = new ConnectionPoint();
                cp9.OffsetX = 40F;
                cp9.OffsetY = 60F;

                // Create the symbol
                newSymGrp.AppendChild(grpFrame);
                newSymGrp.AppendChild(vlvBody);
                newSymGrp.AppendChild(port1);
                newSymGrp.AppendChild(port2);
                newSymGrp.AppendChild(port3);
                newSymGrp.AppendChild(port4);
                newSymGrp.AppendChild(port5);
                newSymGrp.AppendChild(port6);
                newSymGrp.AppendChild(port7);
                newSymGrp.AppendChild(port8);
                newSymGrp.AppendChild(port9);
                newSymGrp.Ports.AddRange(new ConnectionPoint[] { cp1, cp2, cp3, cp4, cp5, cp6, cp7, cp8, cp9 });
                newSymGrp.AppendChild(captField);
                newSymGrp.AppendChild(posField);
                newSymGrp.PinPoint = new PointF(40F, 60F);

                // Assign the new symbol to its field
                mobj_Symbol = newSymGrp;
            }   // End sub

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
            }   // End sub
        #endregion

        #region "Event handlers"
            /// <summary>
            /// Handles the control name changing event via a delegate call
            /// </summary>
            /// <param name="NewName"></param>
            public void OnNameChange(object Sender, string NewName)
            {
                SetCaption(NewName);
            }   // End sub

            /// <summary>
            /// Handles the valve position changing event via a delegate call
            /// </summary>
            /// <param name="Sender"></param>
            /// <param name="NewPosition">New position to display</param>
            public void OnPositionChange(object Sender, string NewPosition)
            {
                SetPositionDisplay(NewPosition);
            }   // End sub

            /// <summary>
            /// Handles SaveRequired event from control
            /// </summary>
            /// <param name="sender"></param>
            void OnSaveRequired(object sender)
            {
                if (SaveRequired != null) SaveRequired(mobj_Device);
            }   // End sub
        #endregion
    }   // End class
}   // End namespace
