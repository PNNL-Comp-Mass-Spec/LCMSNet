using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    public sealed class FluidicsWasteComponent : FluidicsDevice
    {
        public FluidicsWasteComponent()
        {
            const int size = 50;
            var offset = new Point(2, 2);

            // don't really need an "object" so we'll just create a 10x10 pixel rectangle and overlay a port
            AddRectangle(offset, new Size(size, size), Colors.Black, Brushes.White);
            AddPort(new Point(offset.X + size / 2, offset.Y + size / 2));
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
            //pumps don't have a state of this kind.
            get => -1;
            set
            {
                //do nothing
            }
        }
    }
}
