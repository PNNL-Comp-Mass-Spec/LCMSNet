
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
    /// <summary>
    /// Fluidics Designer class for 6-port, 2-position valve
    /// </summary>
    class classSymbolValve6Port : classSymbolValveBase, IDeviceSymbol
    {

        #region "Methods"
            public classSymbolValve6Port(Syncfusion.Windows.Forms.Diagram.Controls.Diagram TargetDiagram) :
                base(TargetDiagram)
            {
            }

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
                Ellipse port2 = new Ellipse(60, 38, 14, 14);
                port2.EnableCentralPort = false;
                port2.FillStyle.Color = Color.Yellow;
                port2.EditStyle.AllowDelete = false;
                port2.EditStyle.HidePinPoint = true;
                port2.EditStyle.AllowChangeHeight = false;
                port2.EditStyle.AllowChangeWidth = false;
                port2.EditStyle.HideRotationHandle = true;

                // Port 3
                Ellipse port3 = new Ellipse(60, 69, 14, 14);
                port3.EnableCentralPort = false;
                port3.FillStyle.Color = Color.Yellow;
                port3.EditStyle.AllowDelete = false;
                port3.EditStyle.HidePinPoint = true;
                port3.EditStyle.AllowChangeHeight = false;
                port3.EditStyle.AllowChangeWidth = false;
                port3.EditStyle.HideRotationHandle = true;

                // Port 4
                Ellipse port4 = new Ellipse(33, 84, 14, 14);
                port4.EnableCentralPort = false;
                port4.FillStyle.Color = Color.Yellow;
                port4.EditStyle.AllowDelete = false;
                port4.EditStyle.HidePinPoint = true;
                port4.EditStyle.AllowChangeHeight = false;
                port4.EditStyle.AllowChangeWidth = false;
                port4.EditStyle.HideRotationHandle = true;

                // Port 5
                Ellipse port5 = new Ellipse(7, 69, 14, 14);
                port5.EnableCentralPort = false;
                port5.FillStyle.Color = Color.Yellow;
                port5.EditStyle.AllowDelete = false;
                port5.EditStyle.HidePinPoint = true;
                port5.EditStyle.AllowChangeHeight = false;
                port5.EditStyle.AllowChangeWidth = false;
                port5.EditStyle.HideRotationHandle = true;

                // Port 6
                Ellipse port6 = new Ellipse(7, 38, 14, 14);
                port6.EnableCentralPort = false;
                port6.FillStyle.Color = Color.Yellow;
                port6.EditStyle.AllowDelete = false;
                port6.EditStyle.HidePinPoint = true;
                port6.EditStyle.AllowChangeHeight = false;
                port6.EditStyle.AllowChangeWidth = false;
                port6.EditStyle.HideRotationHandle = true;

                // Connection point 1
                ConnectionPoint cp1 = new ConnectionPoint();
                cp1.OffsetX = 40F;
                cp1.OffsetY = 29F;

                // Connection point 2
                ConnectionPoint cp2 = new ConnectionPoint();
                cp2.OffsetX = 67F;
                cp2.OffsetY = 45F;

                // Connection point 3
                ConnectionPoint cp3 = new ConnectionPoint();
                cp3.OffsetX = 67F;
                cp3.OffsetY = 76F;

                // Connection point 4
                ConnectionPoint cp4 = new ConnectionPoint();
                cp4.OffsetX = 40F;
                cp4.OffsetY = 91F;

                // Connection point 5
                ConnectionPoint cp5 = new ConnectionPoint();
                cp5.OffsetX = 14F;
                cp5.OffsetY = 76F;

                // Connection point 6
                ConnectionPoint cp6 = new ConnectionPoint();
                cp6.OffsetX = 14F;
                cp6.OffsetY = 45F;

                // Create the symbol
                newSymGrp.AppendChild(grpFrame);
                newSymGrp.AppendChild(vlvBody);
                newSymGrp.AppendChild(port1);
                newSymGrp.AppendChild(port2);
                newSymGrp.AppendChild(port3);
                newSymGrp.AppendChild(port4);
                newSymGrp.AppendChild(port5);
                newSymGrp.AppendChild(port6);
                newSymGrp.Ports.AddRange(new ConnectionPoint[] { cp1, cp2, cp3, cp4, cp5, cp6 });
                newSymGrp.AppendChild(captField);
                newSymGrp.AppendChild(posField);
                newSymGrp.PinPoint = new PointF(40F, 60F);

                // Assign the new symbol to its field
                mobj_Symbol = newSymGrp;
            }
        #endregion
    }   // Ens class
}
