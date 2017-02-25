/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 11/27/2013
 * 
 * Last Modified 11/27/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using System.Drawing;
using FluidicsSDK.Devices;

namespace FluidicsSDK.Devices
{
    class FluidicsSampler: FluidicsDevice, IFluidicsDevice
    {

        Point DEFAULT_POINT = new Point(0, 0);

        public FluidicsSampler()
            : base()
        {            
            DefineGraphcs(DEFAULT_POINT);
        }

        public FluidicsSampler(Point loc)
        {
            DefineGraphcs(loc);               
        }

        /// <summary>
        /// defines the graphics that make up the PAL primitive
        /// </summary>
        /// <param name="basePoint">location for the "main" graphic to start at</param>
        public void DefineGraphcs(Point basePoint)
        {
            //define body
            //base rectangle
            base.AddRectangle(basePoint, new Size(180, 40), Color.Black, Brushes.White, true);
            // offset from base rectangle (5, 25)
            base.AddRectangle(new Point(basePoint.X + 5, basePoint.Y + 25), new Size(170, 8), Color.Black, Brushes.White, true);

            //define sampler arm
            // offset from base rectangle(25, -35)
            Point samplerArmBase = new Point(basePoint.X + 25, basePoint.Y - 35);
            base.AddRectangle(samplerArmBase, new Size(35, 110), Color.Black, Brushes.White, true);
            // offset from sampler arm outer rectangle(5, 45)
            base.AddRectangle(new Point(samplerArmBase.X + 5, samplerArmBase.Y + 45), new Size(25, 62), Color.Black, Brushes.Gray, true);

            //define sample cabinet + drawers
            //offset from base rectangle (90, 41)
            Point cabinetBase = new Point(basePoint.X + 90, basePoint.Y + 41);
            base.AddRectangle(cabinetBase, new Size(80, 110), Color.Black, Brushes.White, true);
            //offset from cabinet outer rectangle (5, 8)
            Point firstDrawerBase = new Point(cabinetBase.X + 5, cabinetBase.Y + 8);
            base.AddRectangle(firstDrawerBase, new Size(70, 25), Color.Black, Brushes.White, true);
            //offset from first drawer(0, 35)
            Point secondDrawerBase = new Point(firstDrawerBase.X, firstDrawerBase.Y + 35);
            base.AddRectangle(secondDrawerBase, new Size(70, 25), Color.Black, Brushes.White, true);
            //offset from second drawer(0, 35)
            base.AddRectangle(new Point(secondDrawerBase.X, secondDrawerBase.Y + 35), new Size(70, 25), Color.Black, Brushes.White, true);
        }

        /// <summary>
        /// IDevice required 
        /// </summary>
        /// <param name="device"></param>
        protected override void ClearDevice(LcmsNetDataClasses.Devices.IDevice device)
        {
            IDevice = null;
        }

        /// <summary>
        /// IDevice required
        /// </summary>
        /// <param name="device"></param>
        protected override void SetDevice(LcmsNetDataClasses.Devices.IDevice device)
        {
            IDevice = device;
        }

        /// <summary>
        /// fluidics device required method
        /// </summary>
        /// <param name="state"></param>
        public override void ActivateState(int state)
        {
            //do nothing with this
        }        
  
        /// <summary>
        /// fluidics device required method
        /// </summary>
        /// <returns></returns>
        public override string StateString()
        {
            return string.Empty;
        }

        
        /// <summary>
        /// fluidics device required method
        /// </summary>
        public override int CurrentState
        {
            //PAL doesn't have a state as valves do
            get
            {
                return -1;
            }
            set
            {
                // do nothing
            }
        }

        public override SizeF Size
        {
            get
            {
                // get the primitve with the highest value for location + size in both width and height
                int width = m_primitives.Max(x => x.Loc.X + x.Size.Width);
                int height = m_primitives.Max(x => x.Loc.Y + x.Size.Height);
                //subtract the smallest x and y from respective measures
                width -= m_primitives.Min(x => x.Loc.X);
                height -= m_primitives.Min(x => x.Loc.Y);
                // return new size, as width - smallest x = total width of the device
                // and height - smallest y = total height of the device.
                return new Size(width, height);
            }
            set
            {
            }
        }
        #region IFluidicsDevice Members

        public event EventHandler DeviceSaveRequired
        {
            add { }
            remove { }
        }

        #endregion
    }
}
