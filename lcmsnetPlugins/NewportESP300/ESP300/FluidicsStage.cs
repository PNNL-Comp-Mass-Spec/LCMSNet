using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.Newport.ESP300
{
    public sealed class FluidicsStage : FluidicsDevice
    {
        private classNewportStage m_obj;

        private const int STAGE_WIDTH = 100;
        private const int STAGE_HEIGHT = 100;

        public FluidicsStage()
        {
            var offset = new Point(6 + Port.PORT_DEFAULT_RADIUS * 2, 2);
            AddRectangle(offset, new Size(STAGE_WIDTH, STAGE_HEIGHT), Colors.Black, Brushes.White, true, null);
            AddPort(new Point(offset.X + STAGE_WIDTH + 14, offset.Y + STAGE_HEIGHT / 2));
            AddPort(new Point(offset.X + Loc.X - 14, offset.Y + Loc.Y + STAGE_HEIGHT / 2));
            m_info_controls_box = new Rect(offset, new Size(STAGE_WIDTH, STAGE_HEIGHT));
        }

        public override int CurrentState
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        protected override Rect UpdateControlBoxLocation()
        {
            return new Rect(Loc.X, Loc.Y, STAGE_WIDTH, STAGE_HEIGHT);
        }

        public override string StateString()
        {
            return m_obj.CurrentPos;
        }

        public override void ActivateState(int state)
        {
        }

        protected override void ClearDevice(IDevice device)
        {
            m_obj = null;
        }

        protected override void SetDevice(IDevice device)
        {
            m_obj = device as classNewportStage;
        }
    }
}
