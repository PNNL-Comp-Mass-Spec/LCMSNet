using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;

namespace FluidicsSDK.Devices
{
    public sealed class FluidicsDetector : FluidicsDevice
    {
        private const int WIDTH = 100;
        private const int HEIGHT = 50;
        private readonly Size m_size = new Size(WIDTH, HEIGHT);

        public FluidicsDetector()
        {
            var myRectangle = new FluidicsRectangle(new Point(0, 0), m_size, Colors.Black, Brushes.White);
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
