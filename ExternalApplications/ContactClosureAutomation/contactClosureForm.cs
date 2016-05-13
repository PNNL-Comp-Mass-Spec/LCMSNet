using System;
using System.Threading;
using System.Windows.Forms;

using LcmsNet.Devices;
using LcmsNet.Devices.ContactClosure;

namespace ContactClosureAutomation
{
    /// <summary>
    /// Simple form that triggers a contact closure.
    /// </summary>
    public partial class contactClosureForm : Form
    {
        #region Members
        /// <summary>
        /// Constant.
        /// </summary>
        private const int MILLISECONDS_PER_SECOND = 1000;
        /// <summary>
        /// Contact closure object - interface to hardware.
        /// </summary>
        private classContactClosure m_closure;
        /// <summary>
        /// Time length of trigger.
        /// </summary>
        private int                 m_holdTimeSeconds;
        /// <summary>
        /// Time between triggers.
        /// </summary>
        private int                 m_delayTimeSeconds;
        /// <summary>
        /// Thread that is being triggered on.
        /// </summary>
        private Thread              m_thread;
        /// <summary>
        /// Total number of triggers made.
        /// </summary>
        private int                 m_totalTriggersMade;
        /// <summary>
        /// Event to safely de-trigger the device.
        /// </summary>
        ManualResetEvent            m_abortEvent;
        /// <summary>
        /// Path to log file.
        /// </summary>
        private string              m_logPath;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public contactClosureForm()
        {
            InitializeComponent();

            m_logPath = "logfile.log";

            classDeviceManager.Manager  = new LcmsNet.Devices.classDeviceManager();
            m_abortEvent                = new ManualResetEvent(false);
            classLabjackU12 labjack     = new classLabjackU12();
            m_closure                   = new classContactClosure(enumLabjackU12OutputPorts.D0);
            m_closure.AbortEvent        = m_abortEvent;
            m_contactControl.Device     = m_closure;
            m_timer.Tick                 += new EventHandler(m_timer_Tick);
            FormClosing                  += new FormClosingEventHandler(contactClosureForm_FormClosing);
            classApplicationLogger.Error += new classApplicationLogger.DelegateErrorHandler(classApplicationLogger_Error);

            m_contactControl.Port = enumLabjackU12OutputPorts.D0;

            UpdateCycleTime();            
            LoadSettings();
        }


        #region Methods
        /// <summary>
        /// Updates the status label.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            m_statusLabel.Text = message;  
 
            try
            {
                using (System.IO.TextWriter writer = System.IO.File.AppendText(m_logPath))
                {
                    string newMessage = string.Format("{0}\t{1}", DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)), message);
                    writer.WriteLine(newMessage);
                }
            }catch
            {
                //pass.
            }
        }
        /// <summary>
        /// Updates status label with error.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="args"></param>
        void classApplicationLogger_Error(int errorLevel, LcmsNetDataClasses.Logging.classErrorLoggerArgs args)
        {
            UpdateStatus(args.Message);
        }
        /// <summary>
        /// Make sure the thread dies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void contactClosureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Kill();
            SaveSettings();
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadSettings()
        {
            m_delayTime.Value       = Properties.Settings.Default.DelayTime;
            m_holdOpenTime.Value    = Properties.Settings.Default.TriggerTime; 
            m_totalTriggers.Value   = Properties.Settings.Default.NumTriggers;
            try
            {
                m_contactControl.Port = (enumLabjackU12OutputPorts)Enum.Parse(typeof(enumLabjackU12OutputPorts),  Properties.Settings.Default.Port.ToString());
            }
            catch
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void SaveSettings()
        {
            Properties.Settings.Default.DelayTime   = Convert.ToInt32(m_delayTime.Value);
            Properties.Settings.Default.TriggerTime = Convert.ToInt32(m_holdOpenTime.Value);
            Properties.Settings.Default.NumTriggers = Convert.ToInt32(m_totalTriggers.Value);
            Properties.Settings.Default.Port        = m_contactControl.Port;
            Properties.Settings.Default.Save();
        }
        /// <summary>
        /// Timer between delays and triggers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_timer_Tick(object sender, EventArgs e)
        {

            m_totalTriggersMade++;


            int totalTriggersToMake = Convert.ToInt32(m_totalTriggers.Value);
            // We are done!
            if (m_totalTriggersMade > totalTriggersToMake)
            {
                SetIsEnabled(false);
                UpdateStatus("Complete.");
                return;
            }

            StartTrigger();            
        }
        /// <summary>
        /// Starts a trigger thread.
        /// </summary>
        private void StartTrigger()
        {
            string triggers  = string.Format(" - {0}/{1} triggers", (m_totalTriggersMade), 
                                                                 Convert.ToInt32(m_totalTriggers.Value));

            UpdateStatus("Triggering " + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString() + triggers);
            ThreadStart start       = new ThreadStart(TriggerHold);
            Thread shortLivedThread = new Thread(start);
            m_thread                = shortLivedThread;
            shortLivedThread.Start();
        }
        private delegate void DelegateTriggerThreadException(string error, Exception ex);

        private void UpdateStatus(string error, Exception ex)
        {
            UpdateStatus(error + " " + ex.Message);
        }

        /// <summary>
        /// Short lived trigger thread point.
        /// </summary>
        private void TriggerHold()
        {
            try
            {
                m_closure.Trigger(Convert.ToDouble(m_holdTimeSeconds + 5), m_holdTimeSeconds);
            }
            catch(Exception ex)
            {
                BeginInvoke(new DelegateTriggerThreadException(UpdateStatus), new object[] { "Could not trigger.", ex });
            }
        }
        /// <summary>
        /// Enables timers etc.
        /// </summary>
        /// <param name="enabled"></param>
        private void SetIsEnabled(bool enabled)
        {
            m_totalTriggersMade      = 1;
            m_runButton.Enabled      = (enabled == false);
            m_stopButton.Enabled     = (enabled);
            m_holdTimeSeconds        = Convert.ToInt32(m_holdOpenTime.Value);
            m_delayTimeSeconds       = Convert.ToInt32(m_delayTime.Value);

            // Don't let the change stuff here.
            m_contactControl.Enabled = (enabled == false);
            m_holdOpenTime.Enabled   = (enabled == false);
            m_delayTime.Enabled      = (enabled == false);
            m_totalTriggers.Enabled  = (enabled == false);

            // This is key, we only need to trigger the cycle every delay + hold and then the trigger contact closure just does the stuff for us.
            m_timer.Interval         = m_holdTimeSeconds * MILLISECONDS_PER_SECOND + m_delayTimeSeconds * MILLISECONDS_PER_SECOND;
            m_timer.Enabled          = enabled;

            if (enabled)
            {
                m_abortEvent.Reset();
                UpdateStatus(string.Format("Started - {0} Triggers {1} length with {2} delay between triggers", m_totalTriggers.Value, m_holdTimeSeconds, m_delayTimeSeconds));
                StartTrigger();
            }
        }
        /// <summary>
        /// Starts the triggering.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_runButton_Click(object sender, EventArgs e)
        {
            SetIsEnabled(true);
        }
        /// <summary>
        /// Stops the triggering.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_stopButton_Click(object sender, EventArgs e)
        {
            Kill();
        }
        /// <summary>
        /// Kills all processing.
        /// </summary>
        private void Kill()
        {
            UpdateStatus("Stopped.");
            SetIsEnabled(false);
            if (m_thread != null)
            {
                try
                {
                    m_abortEvent.Set();
                    Thread.Sleep(50);
                    m_abortEvent.Reset();
                    m_thread.Abort();
                }
                catch (ThreadAbortException)
                {
                    //pass
                }
                finally
                {
                    m_thread = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateCycleTime()
        {            
            int totalTime    = Convert.ToInt32(m_delayTime.Value) + Convert.ToInt32(m_holdOpenTime.Value);
            m_cycleTime.Text = totalTime.ToString() + " seconds"; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_delayTime_ValueChanged(object sender, EventArgs e)
        {
            UpdateCycleTime();
        }
        /// <summary>
        /// Updates the cycle time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_holdOpenTime_ValueChanged(object sender, EventArgs e)
        {
            UpdateCycleTime();
        }
        #endregion

        private void m_totalTriggers_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
