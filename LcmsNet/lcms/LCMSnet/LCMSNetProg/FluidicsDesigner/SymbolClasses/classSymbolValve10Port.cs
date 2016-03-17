
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 07/30/2009
//
// Last modified 07/30/2009
//                      09/03/2009 (DAC) - Modified to obtain default caption from device control
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing;
using LcmsNet.Devices.Valves;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.FluidicsDesigner
{
    class classSymbolValve10Port : classSymbolValveBase, IDeviceSymbol
    {
        //*********************************************************************************************************
        // Fluidics Destigner class for 10-port, 2-position valve
        //**********************************************************************************************************

        #region "Methods"
        public classSymbolValve10Port(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram) :
                base(TargetDiagram)
            {
            }   // end sub

            protected override void CreateSymbolGroup()
            {
                base.CreateSymbolGroup();

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
                Ellipse port2 = new Ellipse(51, 28, 14, 14);
                port2.EnableCentralPort = false;
                port2.FillStyle.Color = Color.Yellow;
                port2.EditStyle.AllowDelete = false;
                port2.EditStyle.HidePinPoint = true;
                port2.EditStyle.AllowChangeHeight = false;
                port2.EditStyle.AllowChangeWidth = false;
                port2.EditStyle.HideRotationHandle = true;

                // Port 3
                Ellipse port3 = new Ellipse(62, 43, 14, 14);
                port3.EnableCentralPort = false;
                port3.FillStyle.Color = Color.Yellow;
                port3.EditStyle.AllowDelete = false;
                port3.EditStyle.HidePinPoint = true;
                port3.EditStyle.AllowChangeHeight = false;
                port3.EditStyle.AllowChangeWidth = false;
                port3.EditStyle.HideRotationHandle = true;

                // Port 4
                Ellipse port4 = new Ellipse(62, 63, 14, 14);
                port4.EnableCentralPort = false;
                port4.FillStyle.Color = Color.Yellow;
                port4.EditStyle.AllowDelete = false;
                port4.EditStyle.HidePinPoint = true;
                port4.EditStyle.AllowChangeHeight = false;
                port4.EditStyle.AllowChangeWidth = false;
                port4.EditStyle.HideRotationHandle = true;

                // Port 5
                Ellipse port5 = new Ellipse(51, 78, 14, 14);
                port5.EnableCentralPort = false;
                port5.FillStyle.Color = Color.Yellow;
                port5.EditStyle.AllowDelete = false;
                port5.EditStyle.HidePinPoint = true;
                port5.EditStyle.AllowChangeHeight = false;
                port5.EditStyle.AllowChangeWidth = false;
                port5.EditStyle.HideRotationHandle = true;

                // Port 6
                Ellipse port6 = new Ellipse(33, 84, 14, 14);
                port6.EnableCentralPort = false;
                port6.FillStyle.Color = Color.Yellow;
                port6.EditStyle.AllowDelete = false;
                port6.EditStyle.HidePinPoint = true;
                port6.EditStyle.AllowChangeHeight = false;
                port6.EditStyle.AllowChangeWidth = false;
                port6.EditStyle.HideRotationHandle = true;

                // Port 7
                Ellipse port7 = new Ellipse(15, 78, 14, 14);
                port7.EnableCentralPort = false;
                port7.FillStyle.Color = Color.Yellow;
                port7.EditStyle.AllowDelete = false;
                port7.EditStyle.HidePinPoint = true;
                port7.EditStyle.AllowChangeHeight = false;
                port7.EditStyle.AllowChangeWidth = false;
                port7.EditStyle.HideRotationHandle = true;

                // Port 8
                Ellipse port8 = new Ellipse(4, 63, 14, 14);
                port8.EnableCentralPort = false;
                port8.FillStyle.Color = Color.Yellow;
                port8.EditStyle.AllowDelete = false;
                port8.EditStyle.HidePinPoint = true;
                port8.EditStyle.AllowChangeHeight = false;
                port8.EditStyle.AllowChangeWidth = false;
                port8.EditStyle.HideRotationHandle = true;

                // Port 9
                Ellipse port9 = new Ellipse(4, 43, 14, 14);
                port9.EnableCentralPort = false;
                port9.FillStyle.Color = Color.Yellow;
                port9.EditStyle.AllowDelete = false;
                port9.EditStyle.HidePinPoint = true;
                port9.EditStyle.AllowChangeHeight = false;
                port9.EditStyle.AllowChangeWidth = false;
                port9.EditStyle.HideRotationHandle = true;

                // Port 10
                Ellipse port10 = new Ellipse(15, 28, 14, 14);
                port10.EnableCentralPort = false;
                port10.FillStyle.Color = Color.Yellow;
                port10.EditStyle.AllowDelete = false;
                port10.EditStyle.HidePinPoint = true;
                port10.EditStyle.AllowChangeHeight = false;
                port10.EditStyle.AllowChangeWidth = false;
                port10.EditStyle.HideRotationHandle = true;

                // Connection point 1
                ConnectionPoint cp1 = new ConnectionPoint();
                cp1.OffsetX = 40F;
                cp1.OffsetY = 29F;

                // Connection point 2
                ConnectionPoint cp2 = new ConnectionPoint();
                cp2.OffsetX = 58F;
                cp2.OffsetY = 35F;

                // Connection point 3
                ConnectionPoint cp3 = new ConnectionPoint();
                cp3.OffsetX = 69F;
                cp3.OffsetY = 50F;

                // Connection point 4
                ConnectionPoint cp4 = new ConnectionPoint();
                cp4.OffsetX = 69F;
                cp4.OffsetY = 70F;

                // Connection point 5
                ConnectionPoint cp5 = new ConnectionPoint();
                cp5.OffsetX = 58F;
                cp5.OffsetY = 85F;

                // Connection point 6
                ConnectionPoint cp6 = new ConnectionPoint();
                cp6.OffsetX = 40F;
                cp6.OffsetY = 91F;

                // Connection point 7
                ConnectionPoint cp7 = new ConnectionPoint();
                cp7.OffsetX = 22F;
                cp7.OffsetY = 85F;

                // Connection point 8
                ConnectionPoint cp8 = new ConnectionPoint();
                cp8.OffsetX = 11F;
                cp8.OffsetY = 70F;

                // Connection point 9
                ConnectionPoint cp9 = new ConnectionPoint();
                cp9.OffsetX = 11F;
                cp9.OffsetY = 50F;

                // Connection point 10
                ConnectionPoint cp10 = new ConnectionPoint();
                cp10.OffsetX = 22F;
                cp10.OffsetY = 35F;

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
                newSymGrp.AppendChild(port10);
                newSymGrp.Ports.AddRange(new ConnectionPoint[] { cp1, cp2, cp3, cp4, cp5, cp6, cp7, cp8, cp9, cp10 });
                newSymGrp.AppendChild(captField);
                newSymGrp.AppendChild(posField);
                newSymGrp.PinPoint = new PointF(40F, 60F);

                // Assign the new symbol to its field
                mobj_Symbol = newSymGrp;
            }
        #endregion
    }   // End class
}   // End namespace
