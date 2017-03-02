
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 07/29/2009
//
// Last modified 07/29/2009
//                      09/03/2009 (DAC) - Added group name property for saving/loading config
//                      12/02/2009 (DAC) - Added event for handling PAL tray list
//                      02/16/2010 (DAC) - Added Dispose method for event handler cleanup
//*********************************************************************************************************
using Syncfusion.Windows.Forms.Diagram;
using LcmsNet.Devices.Valves;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.FluidicsDesigner
{
    public class classSymbolValveBase
    {
        //*********************************************************************************************************
        // Fluidics Destigner base class for 2-position valves
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
            protected controlValveVICI2Pos mobj_Device;
            protected Syncfusion.Windows.Forms.Diagram.Controls.Diagram mobj_Diagram;
        #endregion

        #region "Events"
        public event DelegateOperationError     OperationError;
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

        #region "Methods"
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="TargetDiagram"></param>
            public classSymbolValveBase(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram)
            {
                mobj_Diagram = TargetDiagram;
                mobj_Device = new controlValveVICI2Pos();
                mobj_Device.Emulation = bool.Parse(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_EMULATIONENABLED));
                mobj_Device.NameChanged += new DelegateNameChanged(OnNameChange);
                mobj_Device.PosChanged += new DelegatePositionChanged(OnPositionChange);
                mobj_Device.SaveRequired += new LcmsNetDataClasses.Devices.DelegateSaveRequired(OnSaveRequired);
                CreateSymbolGroup();
            }   // End sub

            /// <summary>
            /// Unsubscribes event handlers
            /// </summary>
            public void Dispose()
            {
                mobj_Device.NameChanged -= OnNameChange;
                mobj_Device.PosChanged -= OnPositionChange;
                mobj_Device.SaveRequired -= OnSaveRequired;
            }   // End sub

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
            /// Creates the symbol to display on the diagram (must be overridden)
            /// </summary>
            protected virtual void CreateSymbolGroup()
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
                // Remainder of method is in derived classes
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
