//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 09/02/2009
//
// Last modified 09/02/2009
//                      09/03/2009 (DAC) - Added group name property for saving/loading config
//                      09/18/2009 (DAC) - Added SaveSymbolSettings method for saving config
//                      12/02/2009 (DAC) - Added event for handling PAL tray list
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing;

namespace LcmsNet.FluidicsDesigner
{
    class classSymbolConnectionPort : IDeviceSymbol
    {
        //*********************************************************************************************************
        //Fluidics designer class for a simple stand-alone port
        //**********************************************************************************************************

        #region "Class variables"
            Group mobj_Symbol;
            IDeviceControl mobj_Device;
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
            public classSymbolConnectionPort(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram)
            {
                mobj_Diagram = TargetDiagram;
                mobj_Device = null;
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
            /// Brings up the device status page
            /// </summary>
            public void ShowProps()
            {
                // Do nothing
            }

            /// <summary>
            /// Unsubscribes event handlers
            /// </summary>
            public void Dispose()
            {
                // Do nothing - method is here to satisfy interface requirement
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
                Group NewSymGrp = new Group();
                NewSymGrp.CanUngroup = false;
                NewSymGrp.EditStyle.AllowDelete = false;
                NewSymGrp.EditStyle.HidePinPoint = true;
                NewSymGrp.EditStyle.HideRotationHandle = true;
                NewSymGrp.EditStyle.AllowChangeHeight = false;
                NewSymGrp.EditStyle.AllowChangeWidth = false;
                NewSymGrp.EnableCentralPort = false;

                // Frame containing symbol group
                Syncfusion.Windows.Forms.Diagram.Rectangle grpFrame = new Syncfusion.Windows.Forms.Diagram.Rectangle(10, 10, 24, 24);
                grpFrame.FillStyle.Color = Color.Transparent;
                grpFrame.LineStyle.LineColor = Color.Transparent;
                grpFrame.EnableCentralPort = false;
                grpFrame.Name = "Frame";
                grpFrame.EditStyle.AllowDelete = false;
                grpFrame.EditStyle.AllowChangeHeight = false;
                grpFrame.EditStyle.AllowChangeWidth = false;
                grpFrame.EditStyle.HidePinPoint = true;
                grpFrame.EditStyle.HideRotationHandle = true;

                // Port
                Ellipse Port1 = new Ellipse(5, 5, 14, 14);
                Port1.EnableCentralPort = false;
                Port1.EditStyle.AllowDelete = false;
                Port1.EditStyle.AllowChangeHeight = false;
                Port1.EditStyle.AllowChangeWidth = false;
                Port1.EditStyle.HidePinPoint = true;
                Port1.EditStyle.HideRotationHandle = true;

                // Connection point
                ConnectionPoint CP1 = new ConnectionPoint();
                CP1.OffsetX = 12F;
                CP1.OffsetY = 12F;

                // Build the symbol
                NewSymGrp.AppendChild(grpFrame);
                NewSymGrp.AppendChild(Port1);
                NewSymGrp.Ports.Add(CP1);

                // Add new symbol to drawing field
                mobj_Symbol = NewSymGrp;
            }
        #endregion

        #region "Event handlers"
            /// <summary>
            /// Dummy handler required by ISymbol interface
            /// </summary>
            /// <param name="NewName"></param>
            public void OnNameChange(object Sender, string NewName)
            {
                // Do nothing
            }
        #endregion
    }
}
