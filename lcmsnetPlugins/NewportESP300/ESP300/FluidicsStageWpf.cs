using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using LcmsNetDataClasses.Devices;

namespace Newport.ESP300
{
    public sealed class FluidicsStageWpf : FluidicsDeviceWpf
    {
        private classNewportStage m_obj;

        private const int STAGE_WIDTH = 100;
        private const int STAGE_HEIGHT = 100;

        public FluidicsStageWpf()
        {
            AddRectangle(new Point(0, 0), new Size(STAGE_WIDTH, STAGE_HEIGHT), Colors.Black, Brushes.White, true, null);
            AddPort(new Point(STAGE_WIDTH + 14, STAGE_HEIGHT / 2));
            AddPort(new Point(Loc.X - 14, Loc.Y + STAGE_HEIGHT / 2));
            m_info_controls_box = new Rect(new Point(0, 0), new Size(STAGE_WIDTH, STAGE_HEIGHT));
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
