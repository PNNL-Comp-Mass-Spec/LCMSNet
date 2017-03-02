//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 06/30/2009
//
// Last modified 06/30/2009
//                      09/18/2009 (DAC) - Added changes for saving symbol data
//                      11/05/2009 (DAC) - Added saving to and loading from a default configuration file
//                      12/02/2009 (DAC) - Added event and handler for passing PAL tray listing to main program
//*********************************************************************************************************
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Syncfusion.Windows.Forms.Diagram;
using System.IO;
using System.Xml;

using LcmsNet.Devices.Pumps;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.FluidicsDesigner
{
    public partial class formFluidicsDesigner : Form
    {
        //*********************************************************************************************************
        // Main Fluidics Designer form
        //**********************************************************************************************************

        #region "Constants"
            private const string DEFAULT_CONFIG_NAME = "DefaultConfig.xml";
        #endregion

        #region "Class variables"
            bool mbool_LeftButtonClicked;
            Dictionary<string, IDeviceSymbol> mobject_SymbolList = new Dictionary<string, IDeviceSymbol>();
            bool mbool_SaveRequired = false;
        /// <summary>
        /// Displays pump information for diagnostics for all available pumps.
        /// </summary>
        private formPumpDisplays mform_pumpDataDisplay;
        #endregion

        #region "Events"
            public event DelegateNameListReceived           PalTrayListReceived;
            public event DelegateNameListReceived           InstrumentMethodListReceived;
            public event EventHandler<classStatusEventArgs> Status;
        #endregion

        #region "Properties"
            public bool SaveRequired
            {
                get { return mbool_SaveRequired; }
                set { mbool_SaveRequired = value; }
            }
        #endregion

        #region "Constructors"
            /// <summary>
            /// Constructor
            /// </summary>
            public formFluidicsDesigner()
            {
                InitializeComponent();

                Init();
            }   // End sub
        #endregion

        #region "Methods"
            /// <summary>
            /// Initializes class variables
            /// </summary>
            private void Init()
            {
                // Assign event handlers
                diagramFluidiDesign.EventSink.NodeDoubleClick += new NodeMouseEventHandler(OnNodeDoubleClickEvent);
                diagramFluidiDesign.EventSink.NodeClick += new NodeMouseEventHandler(OnNodeRightClickEvent);
                diagramFluidiDesign.MouseDown += new MouseEventHandler(OnControlMouseDownEvent);

                mform_pumpDataDisplay       = new formPumpDisplays();
                mform_pumpDataDisplay.Icon  = Icon;
            }   // End sub

            /// <summary>
            /// Removes selected symbol from fluidics designer
            /// </summary>
            private void DeleteSymbol()
            {
                bool removed = false;
                if (diagramFluidiDesign.Controller.SelectionList.Count == 1)
                {
                    // Confirm delete
                    DialogResult response = MessageBox.Show("Delete selected component?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (response != DialogResult.OK)
                    {
                        return;
                    }

                    // Determine which symbol is selected
                    Node selectedItem = diagramFluidiDesign.Controller.SelectionList[0];
                    string selItemName = selectedItem.Name;
                    IDeviceSymbol selSymbol = mobject_SymbolList[selItemName];
                    selSymbol.SaveRequired -= OnSaveRequired;
                    if (selSymbol.GetType() != typeof(classSymbolConnectionPort))// Skip if it's a port, which doesn't have a device definition
                    {
                        //Remove from Device Manager lists
                        removed = selSymbol.Device.RemoveDevice();
                        if (!removed)
                        {
                            // Symbol couldn't be removed from device manager
                            MessageBox.Show("Unable to delete symbol", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        // If selected device is a PAL, disconnect the tray list receieved event
                        if (selSymbol.GetType() == typeof(classSymbolPAL))
                        {
                            classSymbolPAL pal        = selSymbol as classSymbolPAL;
                            pal.PalTrayListReceived  -= OnPalTrayListReceived;
                        }
                        else if (selSymbol.GetType() == typeof(classSymbolDetectNetStart))
                        {
                            classSymbolDetectNetStart netstart = selSymbol as classSymbolDetectNetStart;
                            netstart.InstrumentMethodListReceived -= OnInstrumentMethodListReceived;
                        }
                    }

                    // Remove symbol from diagram
                    selectedItem.EditStyle.AllowDelete = true;
                    diagramFluidiDesign.Model.RemoveChild(selectedItem);

                    // Remove from Fluidics Designer object list
                    selSymbol.Dispose();
                    mobject_SymbolList.Remove(selItemName);
                }
                else
                {
                    string Msg = "Select a single component for deletion";
                    MessageBox.Show(Msg, "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }   // End sub

            /// <summary>
            /// Saves the diagram to a file (not currently used)
            /// </summary>
            /// <param name="OutFileName">Name of file to save</param>
            private void SaveDiagram(string outFileName)
            {
                // Save the graphical display
                FileStream outStream;

                try
                {
                    outStream = new FileStream(outFileName, FileMode.Create);
                }
                catch (Exception ex)
                {
                    outStream = null;       // Just being cautious
                    MessageBox.Show("Exception opening " + outFileName + ": " + ex.Message);
                }

                if (outStream != null)
                {
                    try
                    {
                        diagramFluidiDesign.SaveBinary(outStream);
                        MessageBox.Show("File saved");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception saving file: " + ex.Message);
                    }
                    finally
                    {
                        outStream.Close();
                    }
                }
            }   // End sub

            /// <summary>
            /// Saves the configuration data to an XML file
            /// </summary>
            /// <param name="fileNamePath">Fully qualified path to config file</param>
            private void SaveSymbolConfig(string fileNamePath)
            {
                // Save the object information in an XML document
                foreach (Node testNode in diagramFluidiDesign.Model.Nodes)
                {
                    // Does testNode have a corresponding IDeviceSymbol?
                    IDeviceSymbol testSymbol;
                    if (mobject_SymbolList.TryGetValue(testNode.Name, out testSymbol))
                    {
                        classConfigTools.CreateSymbolElement(testNode.Name);
                        string xPath = "//SymbolName[@Name='" + testNode.Name + "']";
                        classConfigTools.CreateElement(xPath,"PinPoint","PinPointX", testNode.PinPoint.X.ToString());
                        classConfigTools.CreateElement(xPath, "PinPoint", "PinPointY", testNode.PinPoint.Y.ToString());
                        classConfigTools.CreateElement(xPath, "SymbolType", "Type", testSymbol.GetType().ToString());
                        testSymbol.SaveSymbolSettings(testNode.Name);
                    }
                }
                // Write the XML config document to a file
                classConfigTools.WriteConfigFile(fileNamePath);


                if (Status != null)
                    Status(this, new classStatusEventArgs("Save Complete."));
            }   // End sub

            /// <summary>
            /// Loads the configuration data from an XML file
            /// (Overload to always show a completion message)
            /// </summary>
            /// <param name="fileNamePath">Fully qualified path to config file</param>
            private void LoadSymbolConfig(string fileNamePath)
            {
                LoadSymbolConfig(fileNamePath, true);
            }   // End sub

            /// <summary>
            /// Loads the configuration data from an XML file
            /// (Overload to allow option of showing a completion message)
            /// </summary>
            /// <param name="fileNamePath">Fully qualified path to config file</param>
            /// <param name="showCompletionMsg">TRUE to show a "Load completed message; FALSE to hide message</param>
            private void LoadSymbolConfig(string fileNamePath, bool showCompletionMsg)
            {
                string msg;

                // Verify no symbols on drawing
                if (diagramFluidiDesign.Model.Nodes.Count != 0)
                {
                    msg = "Cannot load config unless Fluidics Designer is empty";
                    MessageBox.Show(msg);
                    return;
                }

                // Ensure symbol list is empty
                mobject_SymbolList.Clear();

                // Load the file
                classConfigTools.LoadConfigDoc(fileNamePath);

                // Get a list of nodes representing each symbol in config file
                XmlNodeList symbolNodes = classConfigTools.GetSymbolNodes();
                if (symbolNodes == null)
                {
                    msg = "No symbols found in config file";
                    MessageBox.Show(msg);
                    classConfigTools.CloseConfigDoc();
                    return;
                }

                // Process each symbol
                int childIndx;
                IDeviceSymbol newDevSymbol                   = null;
                classSymbolPAL palSymbol                     = null;
                classSymbolDetectNetStart networkStartSymbol = null;
                string symbolName;
                foreach (XmlNode currNode in symbolNodes)
                {
                    childIndx = 0;
                    string symbolType = classConfigTools.GetAttribValue(currNode, "./SymbolType", "Type");
                    float pinPointX = float.Parse(classConfigTools.GetAttribValue(currNode,"./PinPoint[@PinPointX]","PinPointX"));
                    float pinPointY = float.Parse(classConfigTools.GetAttribValue(currNode,"./PinPoint[@PinPointY]","PinPointY"));
                    switch (symbolType)
                    {
                        case "LcmsNet.FluidicsDesigner.classSymbolValve4Port":
                            newDevSymbol = new classSymbolValve4Port(diagramFluidiDesign);
                            childIndx = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName              = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint         = new PointF(pinPointX,pinPointY);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint =   pinPoint;
                                newDevSymbol.GroupName  = symbolName;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolAgilentPump":
                            newDevSymbol = new classSymbolAgilentPump(diagramFluidiDesign);
                            childIndx = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint = new PointF(pinPointX,pinPointY);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint=pinPoint;
                                newDevSymbol.GroupName = symbolName;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolDetectCC":
                            newDevSymbol = new classSymbolDetectCC(diagramFluidiDesign);
                            childIndx = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint = new PointF(pinPointX,pinPointY);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint=pinPoint;
                                newDevSymbol.GroupName = symbolName;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolDetectNetStart":
                            //newDevSymbol = new classSymbolDetectNetStart(diagramFluidiDesign);
                            networkStartSymbol  = new classSymbolDetectNetStart(diagramFluidiDesign);
                            newDevSymbol        = networkStartSymbol;
                            childIndx           = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName              = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint         = new PointF(pinPointX,pinPointY);
                                newDevSymbol.GroupName  = symbolName;
                                networkStartSymbol.InstrumentMethodListReceived += new DelegateNameListReceived(OnInstrumentMethodListReceived);

                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint = pinPoint;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolDetectBrukerStart":
                            newDevSymbol = new classSymbolBrukerStart(diagramFluidiDesign);
                            childIndx = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint = new PointF(pinPointX, pinPointY);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint = pinPoint;
                                newDevSymbol.GroupName = symbolName;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolPAL":
                            //newDevSymbol = new classSymbolPAL(diagramFluidiDesign);
                            palSymbol    = new classSymbolPAL(diagramFluidiDesign);
                            newDevSymbol = palSymbol;
                            childIndx    = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName                     = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint                = new PointF(pinPointX,pinPointY);
                                newDevSymbol.GroupName         = symbolName;
                                palSymbol.PalTrayListReceived += new DelegateNameListReceived(OnPalTrayListReceived);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint=pinPoint;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolValve6Port":
                            newDevSymbol = new classSymbolValve6Port(diagramFluidiDesign);
                            childIndx = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint = new PointF(pinPointX,pinPointY);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint=pinPoint;
                                newDevSymbol.GroupName = symbolName;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolValve9Port":
                            newDevSymbol = new classSymbolValve9Port(diagramFluidiDesign);
                            childIndx = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint = new PointF(pinPointX, pinPointY);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint = pinPoint;
                                newDevSymbol.GroupName = symbolName;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        case "LcmsNet.FluidicsDesigner.classSymbolValve10Port":
                            newDevSymbol = new classSymbolValve10Port(diagramFluidiDesign);
                            childIndx = diagramFluidiDesign.Model.AppendChild(newDevSymbol.DwgGroup);
                            if (childIndx >= 0)
                            {
                                symbolName = diagramFluidiDesign.Model.Nodes[childIndx].Name;
                                PointF pinPoint = new PointF(pinPointX,pinPointY);
                                diagramFluidiDesign.Model.Nodes[childIndx].PinPoint=pinPoint;
                                newDevSymbol.GroupName = symbolName;
                                mobject_SymbolList.Add(symbolName, newDevSymbol);
                            }
                            break;
                        default:
                            msg = "Invalid symbol type in config file: " + symbolType;
                            MessageBox.Show(msg);
                            break;
                    }   // End switch

                    //TODO: The try/catch is temporary code until LoadDeviceSettings is implemented in the device classes
                    try
                    {
                        newDevSymbol.Device.LoadDeviceSettings((XmlElement)classConfigTools.GetDeviceData(currNode));
                                ///
                                /// This is a hack...There should be some way to set the device name after change.
                                ///
                                newDevSymbol.Device.Name = newDevSymbol.Device.Name;
                                newDevSymbol.SaveRequired += new DelegateSaveRequired(OnSaveRequired);

                    }
                    catch (Exception ex)
                    {
                        msg = "LoadSymbolConfig: Exception loading symbol of type " + symbolType;
                        classApplicationLogger.LogError(0, msg, ex);
                        msg = "Exception loading a symbol of type " + symbolType + " during configuration load.";
                        msg += " Complete configuration may not be loaded. See log for details";
                        MessageBox.Show(msg);
                    }
                } // End foreach

                classConfigTools.CloseConfigDoc();  // Close the configuration document

                if (showCompletionMsg) MessageBox.Show("All symbols loaded");
            }

            /// <summary>
            /// Loads a digram file
            /// </summary>
            /// <param name="InFileName">Name of file to load</param>
            private void LoadDiagram(string InFileName)
            {
                FileStream inStream;

                try
                {
                    inStream = new FileStream(InFileName, FileMode.Open);
                }
                catch (Exception ex)
                {
                    inStream = null;        // Just being cautious
                    MessageBox.Show("Exception opening " + InFileName + ": " + ex.Message);
                }

                if (inStream != null)
                {
                    try
                    {
                        diagramFluidiDesign.LoadBinary(inStream);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception opening file: " + ex.Message);
                    }
                    finally
                    {
                        inStream.Close();
                    }
                }
            }   // End sub

            /// <summary>
            /// Displays the properties for a control
            /// </summary>
            private void DisplayProperties()
            {
                if (!mbool_LeftButtonClicked)
                {
                    // Right mouse button was clicked
                    if (diagramFluidiDesign.Controller.SelectionList.Count != 1)
                    {
                        return;
                    }

                    Node SelNode = diagramFluidiDesign.Controller.SelectionList[0];
                    IDeviceSymbol SelSymbol;
                    mobject_SymbolList.TryGetValue(SelNode.Name, out SelSymbol);
                    if (SelSymbol != null)
                    {
                        Forms.formDeviceSettings displayForm = new Forms.formDeviceSettings();
                        displayForm.DeviceControl = SelSymbol.Device;

                        try
                        {
                            displayForm.Text = SelSymbol.Device.Device.Name + " Settings";
                            displayForm.StartPosition = FormStartPosition.CenterParent;
                            displayForm.ShowDialog();
                        }
                        catch (Exception Ex)
                        {
                            MessageBox.Show(Ex.Message);
                        }
                    }
                }
            }   // End sub

            /// <summary>
            /// Loads a default configuration file
            /// </summary>
            public void LoadDefaultConfig()
            {
                // If default config file doesn't exist, just return
                string defaultConfigFile = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH), DEFAULT_CONFIG_NAME);
                if (!File.Exists(defaultConfigFile)) return;

                // Load the settings from the default config file
                try
                {
                    LoadSymbolConfig(defaultConfigFile, false);
                }
                catch (Exception ex)
                {
                    //TODO: Handle this exeception
                }

            }   // End sub
        #endregion

        #region "Event handlers"
            /// <summary>
            /// Handles SaveRequired event
            /// </summary>
            /// <param name="device"></param>
            void OnSaveRequired(LcmsNetDataClasses.Devices.IDeviceControl device)
            {
                mbool_SaveRequired = true;
            }   // End sub

            /// <summary>
            /// Handler for node double-click event
            /// </summary>
            /// <param name="evtArgs"></param>
            private void OnNodeDoubleClickEvent(NodeMouseEventArgs evtArgs)
            {
                DisplayProperties();
            }   // End sub

            /// <summary>
            /// Handler for node right-click event
            /// </summary>
            /// <param name="evtArgs"></param>
            private void OnNodeRightClickEvent(NodeMouseEventArgs evtArgs)
            {
                DisplayProperties();
            }   // End sub

            /// <summary>
            /// Sets flag to indicate which button was clicked
            /// </summary>
            /// <param name="Sender"></param>
            /// <param name="evtArgs"></param>
            private void OnControlMouseDownEvent(object Sender, MouseEventArgs evtArgs)
            {
                if (evtArgs.Button == MouseButtons.Left)
                {
                    mbool_LeftButtonClicked = true;
                }
                else
                {
                    mbool_LeftButtonClicked = false;
                }
            }   // End sub

            /// <summary>
            /// Handles addition of 4-port, 2-position valve
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddValve4Port_Click(object sender, EventArgs e)
            {
                classSymbolValve4Port NewVlv = new classSymbolValve4Port(diagramFluidiDesign);
                NewVlv.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(NewVlv.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string vlvName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    NewVlv.GroupName = vlvName;
                    mobject_SymbolList.Add(vlvName, NewVlv);
                }
            }

            /// <summary>
            /// Handles addition of a 6-port, 2-position valve
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddValve6Port_Click(object sender, EventArgs e)
            {
                classSymbolValve6Port NewVlv = new classSymbolValve6Port(diagramFluidiDesign);
                NewVlv.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(NewVlv.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string vlvName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    NewVlv.GroupName = vlvName;
                    mobject_SymbolList.Add(vlvName, NewVlv);
                }
            }   // End sub

            /// <summary>
            /// Handles addition of a 9-port, multi-position valve
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddValve9Port_Click(object sender, EventArgs e)
            {
                classSymbolValve9Port NewVlv = new classSymbolValve9Port(diagramFluidiDesign);
                NewVlv.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(NewVlv.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string vlvName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    NewVlv.GroupName = vlvName;
                    mobject_SymbolList.Add(vlvName, NewVlv);
                }
            }   // End sub

            /// <summary>
            /// Handles addition of a 10-port, 2-position valve
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddValve10Port_Click(object sender, EventArgs e)
            {
                classSymbolValve10Port NewVlv = new classSymbolValve10Port(diagramFluidiDesign);
                NewVlv.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(NewVlv.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string vlvName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    NewVlv.GroupName = vlvName;
                    mobject_SymbolList.Add(vlvName, NewVlv);
                }
            }   // End sub

            /// <summary>
            /// Handles addition of a PAL
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddPAL_Click(object sender, EventArgs e)
            {
                classSymbolPAL newPal = new classSymbolPAL(diagramFluidiDesign);
                newPal.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(newPal.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string palName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    newPal.GroupName = palName;
                    mobject_SymbolList.Add(palName, newPal);
                    // Assign a hanlder for the tray list received event
                    newPal.PalTrayListReceived += new DelegateNameListReceived(OnPalTrayListReceived);
                }
            }

            /// <summary>
            /// Handles addition of an Agilent pump
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddAgilentPump_Click(object sender, EventArgs e)
            {
                classSymbolAgilentPump newPump = new classSymbolAgilentPump(diagramFluidiDesign);
                newPump.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(newPump.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string pumpName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    newPump.GroupName = pumpName;
                    mobject_SymbolList.Add(pumpName, newPump);
                }
            }   // End sub

            /// <summary>
            /// Hendles addition of a detector with contact closure trigger
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddDetContactClosure_Click(object sender, EventArgs e)
            {
                classSymbolDetectCC newDetectCC = new classSymbolDetectCC(diagramFluidiDesign);
                newDetectCC.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(newDetectCC.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string detectName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    newDetectCC.GroupName = detectName;
                    mobject_SymbolList.Add(detectName, newDetectCC);
                }
            }   // End sub

            /// <summary>
            /// Handles addition of a port
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddPort_Click(object sender, EventArgs e)
            {
                classSymbolConnectionPort newConnPort = new classSymbolConnectionPort(diagramFluidiDesign);
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(newConnPort.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string connName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    newConnPort.GroupName = connName;
                    mobject_SymbolList.Add(connName, newConnPort);
                }
            }   // End sub

            /// <summary>
            /// Hendles addition of a detector with network start trigger
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddDetNetStart_Click(object sender, EventArgs e)
            {
                classSymbolDetectNetStart newDetectNS = new classSymbolDetectNetStart(diagramFluidiDesign);
                newDetectNS.SaveRequired    += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired           = true;
                int ChildIndx                = diagramFluidiDesign.Model.AppendChild(newDetectNS.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string detectName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    newDetectNS.GroupName = detectName;

                    mobject_SymbolList.Add(detectName, newDetectNS);
                }
            }

            /// <summary>
            /// Hendles addition of a detector with Bruker start trigger
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemAddBrukerStart_Click(object sender, EventArgs e)
            {
                classSymbolBrukerStart newDetectorBS = new classSymbolBrukerStart(diagramFluidiDesign);
                newDetectorBS.SaveRequired += new DelegateSaveRequired(OnSaveRequired);
                mbool_SaveRequired = true;
                int ChildIndx = diagramFluidiDesign.Model.AppendChild(newDetectorBS.DwgGroup);
                if (ChildIndx >= 0)
                {
                    string detectName = diagramFluidiDesign.Model.Nodes[ChildIndx].Name;
                    newDetectorBS.GroupName = detectName;
                    mobject_SymbolList.Add(detectName, newDetectorBS);
                }
            }   // End sub
            /// <summary>
            /// Forwards the list received event from the PAL to any subscribed objects
            /// </summary>
            /// <param name="trayList">List of trays on the PAL</param>
            void OnPalTrayListReceived(List<string> trayList)
            {
                if (PalTrayListReceived != null)
                    PalTrayListReceived(trayList);
            }   // End sub
            /// <summary>
            /// Forwards the list received event from the PAL to any subscribed objects
            /// </summary>
            /// <param name="trayList">List of trays on the PAL</param>
            void OnInstrumentMethodListReceived(List<string> trayList)
            {
                if (InstrumentMethodListReceived != null)
                    InstrumentMethodListReceived(trayList);
            }   // End sub

            /// <summary>
            /// Handles deletion of a symbol
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemDelete_Click(object sender, EventArgs e)
            {
                DeleteSymbol();
            }   // End sub

            /// <summary>
            /// Connects two symbols with a line link
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemLinkLine_Click(object sender, EventArgs e)
            {
                if (!diagramFluidiDesign.ActivateTool("LineLinkTool"))
                {
                    string Msg = "Unable to activate line link tool";
                    MessageBox.Show(Msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }   // End sub

            /// <summary>
            /// Connects two symbols with an orthagonal link
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemLinkOrtho_Click(object sender, EventArgs e)
            {
                if (!diagramFluidiDesign.ActivateTool("OrthogonalLinkTool"))
                {
                    string Msg = "Unable to activate orthoganal link tool";
                    MessageBox.Show(Msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            /// <summary>
            /// Loads a file into the Fluidics Designer
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuItemFileLoad_Click(object sender, EventArgs e)
            {
                OpenFileDialog fileOpenDlg = new OpenFileDialog();
                fileOpenDlg.Filter="Config Files (*.xml)|*.xml|All files(*.*)|*.*";
                fileOpenDlg.FilterIndex = 0;
                if (fileOpenDlg.ShowDialog() == DialogResult.OK)
                {
                    LoadSymbolConfig(fileOpenDlg.FileName);
                }
            }   // End sub

            /// <summary>
            /// Saves the fluidics Designer to a file
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void menuItemFileSave_Click(object sender, EventArgs e)
            {
                SaveFileDialog fileSaveDlg = new SaveFileDialog();
                fileSaveDlg.Filter = "Config files (*.xml)|*.xml|All files (*.*)|*.*";
                fileSaveDlg.FilterIndex = 0;
                if (fileSaveDlg.ShowDialog() == DialogResult.OK)
                {
                    SaveSymbolConfig(fileSaveDlg.FileName);
                    mbool_SaveRequired = false;
                }
            }   // End sub

            /// <summary>
            /// Saves the current fluidics designer configuration as the default configuration
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void menuItemSaveAsDefault_Click(object sender, EventArgs e)
            {
                // Confirm save
                string msg = "Do you really want to save current configuration as default? (overwrites old default)";
                DialogResult result;
                result = MessageBox.Show(msg, "Confirm Save", MessageBoxButtons.OKCancel);
                if (result != DialogResult.OK) return;

                // Save the configuration
                string defaultConfigFile = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH), DEFAULT_CONFIG_NAME);
                SaveSymbolConfig(defaultConfigFile);
            }

            /// <summary>
         /// Displays a list of devices and their status.
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void devicesToolStripMenuItem_Click(object sender, EventArgs e)
         {
             LcmsNet.Devices.formDeviceManager deviceViewer = new LcmsNet.Devices.formDeviceManager();
             deviceViewer.Show();
         }
        /// <summary>
        /// Initializes the devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         private void initializeDevicesToolStripMenuItem_Click(object sender, EventArgs e)
         {
             try
             {
                 List<classDeviceErrorArgs> failedDevices = LcmsNet.Devices.classDeviceManager.Manager.InitializeDevices();
                 if (failedDevices != null && failedDevices.Count > 0)
                 {
                     formFailedDevicesDisplay display = new formFailedDevicesDisplay(failedDevices);
                     display.StartPosition = FormStartPosition.CenterScreen;
                     display.Show();
                 }
             }
             catch
             {
                 //TODO: Add logging
             }
         }


         /// <summary>
         /// Display the pump method data tools.
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void pumpDataToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if (mform_pumpDataDisplay != null)
                 mform_pumpDataDisplay.Show();
         }
        #endregion
    }   // End class
}   // End namespace
