using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    public sealed class FluidicsWasteComponentWpf : FluidicsDeviceWpf
    {
        public FluidicsWasteComponentWpf()
        {
            const int size = 50;

            // don't really need an "object" so we'll just create a 10x10 pixel rectangle and overlay a port
            AddPrimitive(new FluidicsRectangleWpf(new Point(0, 0), new Size(size, size), Colors.Black, Brushes.White, true, 1));
            AddPort(new Point(size / 2, size / 2));
            m_info_controls_box.Width = 50;
            Ports[0].Sink = true;
        }

        public override void ActivateState(int state)
        {
        }

        protected override void SetDevice(IDevice device)
        {
        }

        protected override void ClearDevice(IDevice device)
        {
        }

        public override string StateString()
        {
            return string.Empty;
        }

        public override int CurrentState
        {
            get
            {
                //pumps don't have a state of this kind.
                return -1;
            }
            set
            {
                //do nothing
            }
        }
    }
}
