using System;
using System.Windows.Forms;
using FluidicsSDK;
using FluidicsSimulator;
using LcmsNetSDK;

namespace LcmsNet.Simulator
{
    public partial class SimConfigControl : UserControl
    {
        const int TRANSPARENCY_OF_FLUIDICS = 255;
        const float SCALE_OF_FLUIDICS = 1;
        private static SimConfigControl m_instance;

        private readonly classFluidicsModerator m_mod;

        private bool m_tacked;

        private SimConfigControl()
        {
            InitializeComponent();
            m_mod = classFluidicsModerator.Moderator;
            m_mod.ModelChanged += new classFluidicsModerator.ModelChange(ModelChangedHandler);

            m_tacked = true;
            FluidicsSimulator.FluidicsSimulator.GetInstance.EventSimulated += EventSimulated_Handler;

            var sinkCheck = new FluidicsSDK.ModelCheckers.NoSinksModelCheck();
            sinkCheck.IsEnabled = false;

            var sourceCheck = new FluidicsSDK.ModelCheckers.MultipleSourcesModelCheck();
            sourceCheck.IsEnabled = false;

            var cycleCheck = new FluidicsSDK.ModelCheckers.FluidicsCycleCheck();
            cycleCheck.IsEnabled = false;

            var testCheck = new FluidicsSDK.ModelCheckers.TestModelCheck();
            testCheck.IsEnabled = false;

            FluidicsSimulator.FluidicsSimulator.GetInstance.AddModelCheck(sinkCheck as IFluidicsModelChecker);
            FluidicsSimulator.FluidicsSimulator.GetInstance.AddModelCheck(sourceCheck as IFluidicsModelChecker);
            FluidicsSimulator.FluidicsSimulator.GetInstance.AddModelCheck(cycleCheck as IFluidicsModelChecker);
            FluidicsSimulator.FluidicsSimulator.GetInstance.AddModelCheck(testCheck as IFluidicsModelChecker);
            //controlConfig.UpdateImage();
        }

        public static SimConfigControl GetInstance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SimConfigControl();
                }
                return m_instance;
            }
        }

        public bool Tacked
        {
            get { return m_tacked; }
            private set
            {
                m_tacked = value;
                Tack?.Invoke(this, new TackEventArgs(m_tacked));
            }
        }

        public event EventHandler<TackEventArgs> Tack;

        private void EventSimulated_Handler(object sender, SimulatedEventArgs e)
        {
            lblElapsed.Text = "+" + e.SimulatedTimeElapsed.ToString(@"%d\.hh\:mm\:ss");
        }

        public void UpdateImage()
        {
            controlConfig.UpdateImage();
        }

        /// <summary>
        /// event handler for when a fluidics model changes.
        /// </summary>
        private void ModelChangedHandler()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(changeHandler));
            }
            else
            {
                changeHandler(this, new EventArgs());
            }
        }

        void changeHandler(object sender, EventArgs e)
        {
            controlConfig.UpdateImage();
            Refresh();
        }

        public void TackOnRequest()
        {
            m_tacked = true;
        }

        private void btnTack_Click(object sender, EventArgs e)
        {
            Tacked = !Tacked;
        }
    }
}