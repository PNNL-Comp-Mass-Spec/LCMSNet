/*********************************************************************************************************
 * Written by Christopher Walters for the US Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014, Battelle Memorial Institute
 *
 *  Last modified 06/19/2014
 *
/*********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;
using System.Drawing;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.Pumps
{
    public class classPumpIscoGlyph:FluidicsDevice
    {
        private const int CONST_WIDTH = 300;
        private const int CONST_HEIGHT = 25;
        private classPumpIsco m_device;

        public override event EventHandler<FluidicsDevChangeEventArgs> DeviceChanged;

        public classPumpIscoGlyph()
        {
            base.AddRectangle(new Point(0, 0), new Size(CONST_WIDTH, CONST_HEIGHT), Color.Black, Brushes.White, true);
            base.AddPort(new Point((int)(CONST_WIDTH * 0.25), -10));
            base.AddPort(new Point((int)(CONST_WIDTH * 0.5), -10));
            base.AddPort(new Point((int)(CONST_WIDTH * 0.75), -10));
            foreach(var port in Ports)
            {
                port.Source = true;
            }
            m_info_controls_box = new Rectangle(0, 0, 300, 50);
        }

        protected override void SetDevice(IDevice device)
        {
            m_device = device as classPumpIsco;
            m_device.RefreshComplete +=m_device_RefreshComplete;
        }

        private void m_device_RefreshComplete()
        {
            DeviceChanged?.Invoke(this, new FluidicsDevChangeEventArgs("ISCO refreshed"));
        }

        public override string StateString()
        {       
            var formatStr = "{0, -7}{1,6}{2,-7}{3, -7}{4,6}{5, -7}{6,-7}{7,6}{8,-7}\n";
            var pressUnits = classIscoConversions.GetPressUnitsString();
            var flowUnits = classIscoConversions.GetFlowUnitsString();
            var volUnits = "mL";
            var pumpA = string.Format(formatStr, new object[9] { "PresA:", m_device.PumpData[0].Pressure, pressUnits + "         ",  " PresB:", m_device.PumpData[1].Pressure, pressUnits, "        PresC:", m_device.PumpData[2].Pressure, pressUnits });
            var pumpB = string.Format(formatStr, new object[9] { "FlowA:", m_device.PumpData[0].Flow, flowUnits, "   FlowB:", m_device.PumpData[1].Flow, flowUnits, "   FlowC:", m_device.PumpData[2].Flow, flowUnits });
            var pumpC = string.Format(formatStr, new object[9] { "VolA   :", m_device.PumpData[0].Volume, volUnits + "      ", "    VolB  :",  m_device.PumpData[1].Volume, volUnits, "       VolC  :", m_device.PumpData[2].Volume, volUnits });
            return pumpA + pumpB + pumpC;
        }

        protected override void ClearDevice(IDevice device)
        {
            m_device.RefreshComplete -= m_device_RefreshComplete;
            m_device = null;
        }

        public override void ActivateState(int state)
        {            
            // no state to care about here.
        }

        protected override Rectangle UpdateControlBoxLocation()
        {
            return new Rectangle(m_primitives[0].Loc.X, m_primitives[0].Loc.Y + m_primitives[0].Size.Height, m_info_controls_box.Size.Width, m_info_controls_box.Size.Height);
        }

        public override int CurrentState
        {
            get
            {
                return -1;
            }
            set
            {           
     
            }
        }
    }
}
