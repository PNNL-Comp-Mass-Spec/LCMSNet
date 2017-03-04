/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 12/16/2013
 *
 ********************************************************************************************************/

using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using System.Drawing;

namespace FluidicsSDK.Devices
{
    public sealed class FluidicsDetector:FluidicsDevice
    {
        private const int WIDTH = 100;
        private const int HEIGHT = 50;
        private readonly Size m_size = new Size(WIDTH, HEIGHT);

        public FluidicsDetector()
        {
            var myRectangle = new FluidicsRectangle(new Point(0,0), m_size, Color.Black, Brushes.White);
            AddPrimitive(myRectangle);
            m_deviceName = "Detector";
        }

        protected override void ClearDevice(LcmsNetDataClasses.Devices.IDevice device)
        {
            // do nothing
        }

        protected override void SetDevice(LcmsNetDataClasses.Devices.IDevice device)
        {
            var detector = device as IFluidicsClosure;
            if (detector != null)
            {
                ClosureType = detector.GetClosureType();
            }
        }

        public override void ActivateState(int state)
        {
            //do nothing
        }

        public override string StateString()
        {
            return ClosureType;
        }
        public override int CurrentState
        {
            get
            {
                return -1;
            }
            set
            {
                //do nothing
            }
        }


        // property for type of closure (bruker, network start, etc)
        public string ClosureType
        {
            get;
            set;
        }
    }
}
