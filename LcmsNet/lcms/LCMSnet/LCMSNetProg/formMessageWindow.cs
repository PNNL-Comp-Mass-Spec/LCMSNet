using System;
using System.Windows.Forms;
using System.ComponentModel;
using LcmsNetDataClasses.Logging;

namespace LcmsNet
{
    /// <summary>
    /// Form that displays errors and messages to the user.
    /// </summary>
    public partial class formMessageWindow : Form
    {
        public delegate void DelegateShowErrors(int level, classErrorLoggerArgs args);

        bool m_errorLabelSelected = false;
        bool m_messageLabelSelected = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public formMessageWindow()
        {
            InitializeComponent();
            m_messageLevel = classApplicationLogger.CONST_STATUS_LEVEL_USER;
            m_errorLevel = classApplicationLogger.CONST_STATUS_LEVEL_USER;
            mobj_lockMessages = new object();
            mobj_lockErrors = new object();
            SelectErrorTab();
        }

        /// <summary>
        /// Fired when an error is found by the system.
        /// </summary>
        public event EventHandler ErrorPresent;

        /// <summary>
        /// Fired when the user says the errors are cleared.
        /// </summary>
        public event EventHandler ErrorCleared;

        /// <summary>
        /// Formats the input string message with a date and time string.
        /// </summary>
        /// <param name="message">Message to format.</param>
        /// <returns>Formatted string message to display.</returns>
        private string FormatMessage(string message)
        {
            return string.Format("{0} {1}: {2}",
                DateTime.UtcNow.Subtract(LcmsNetSDK.TimeKeeper.Instance.TimeZone.BaseUtcOffset).ToLongDateString(),
                DateTime.UtcNow.Subtract(LcmsNetSDK.TimeKeeper.Instance.TimeZone.BaseUtcOffset).TimeOfDay.ToString(),
                message);
        }

        /// <summary>
        /// Displays the error message to the screen.
        /// </summary>
        /// <param name="level">Filter for displaying messages.</param>
        /// <param name="message">Message to show user.</param>
        public void ShowMessage(int level, classMessageLoggerArgs message)
        {
            if (level <= m_messageLevel && message != null)
            {
                InsertMessage(FormatMessage(message.Message));
                //mlistBox_messages.Items.Insert(0, FormatMessage(message.Message));
                //mlistBox_messages.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Updates message window using a delegate to avoid cross-thread problems
        /// </summary>
        /// <param name="message"></param>
        private void InsertMessage(string message)
        {
            if (mlistBox_messages.InvokeRequired)
            {
                var d = new delegateInsertMessage(InsertMessage);
                mlistBox_messages.Invoke(d, new object[] {message});
            }
            else
            {
                lock (mobj_lockMessages)
                {
                    mlistBox_messages.Items.Insert(0, message);
                    mlistBox_messages.SelectedIndex = 0;
                }
            }
        }

        private void ShowErrorsDelegated(int level, classErrorLoggerArgs args)
        {
            if (level <= m_errorLevel && args != null)
            {
                if (ErrorPresent != null)
                {
                    ErrorPresent(this, new EventArgs());
                }
                var exceptions = "";
                lock (mobj_lockErrors)
                {
                    if (args.Exception != null)
                    {
                        m_errorMessages.Text = FormatMessage(args.Exception.StackTrace) + "\n" + m_errorMessages.Text;

                        var ex = args.Exception;
                        while (ex != null)
                        {
                            exceptions += ex.Message + "\n";
                            ex = ex.InnerException;
                        }

                        m_errorMessages.Text = FormatMessage(exceptions) + "\n" + m_errorMessages.Text;
                    }

                    m_errorMessages.Text = FormatMessage(args.Message) + "\n" + m_errorMessages.Text;
                }
            }
        }

        /// <summary>
        /// Displays error messages to the user.
        /// </summary>
        /// <param name="level">Level of errors to display.</param>
        /// <param name="error">Error to display.</param>
        public void ShowErrors(int level, classErrorLoggerArgs error)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateShowErrors(ShowErrorsDelegated), new object[] {level, error});
            }
            else
            {
                ShowErrorsDelegated(level, error);
            }
        }

        private void mbutton_acknowledgeErrors_Click(object sender, EventArgs e)
        {
            m_errorMessages.Clear();
            if (ErrorCleared != null)
            {
                ErrorCleared(this, new EventArgs());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (mobj_lockMessages)
            {
                mlistBox_messages.Items.Clear();
            }
        }

        private void mlabel_errors_Click(object sender, EventArgs e)
        {
            SelectErrorTab();
        }

        private void mlabel_messages_Click(object sender, EventArgs e)
        {
            SelectMessageTab();
        }

        private void SelectErrorTab()
        {
            customTabControl1.SelectedTab = mtab_errors;
            //mlabel_errors.BackColor             = System.Drawing.Color.White;
            mlabel_errors.ForeColor = System.Drawing.Color.Black;
            mpanel_errorIndicator.BackColor = System.Drawing.Color.DarkGray;

            //mlabel_messages.BackColor           = System.Drawing.Color.LightGray;
            mlabel_messages.ForeColor = System.Drawing.Color.Gray;
            mpanel_messageIndicator.BackColor = System.Drawing.Color.White;
            m_errorLabelSelected = true;
            m_messageLabelSelected = false;
        }

        private void SelectMessageTab()
        {
            customTabControl1.SelectedTab = mtab_messages;
            //mlabel_errors.BackColor             = System.Drawing.Color.LightGray;
            mlabel_errors.ForeColor = System.Drawing.Color.Gray;
            mpanel_errorIndicator.BackColor = System.Drawing.Color.White;

            //mlabel_messages.BackColor           = System.Drawing.Color.White;
            mlabel_messages.ForeColor = System.Drawing.Color.Black;
            mpanel_messageIndicator.BackColor = System.Drawing.Color.DarkGray;
            m_messageLabelSelected = true;
            m_errorLabelSelected = false;
        }


        private void mlabel_errors_MouseLeave(object sender, EventArgs e)
        {
            if (!m_errorLabelSelected)
            {
                mlabel_errors.ForeColor = System.Drawing.Color.Gray;
                mpanel_errorIndicator.BackColor = System.Drawing.Color.White;
            }
        }

        private void mlabel_errors_MouseEnter(object sender, EventArgs e)
        {
            if (!m_errorLabelSelected)
            {
                mlabel_errors.ForeColor = System.Drawing.Color.LightGray;
                mpanel_errorIndicator.BackColor = System.Drawing.Color.DarkGray;
            }
        }

        private void mlabel_messages_MouseLeave(object sender, EventArgs e)
        {
            if (!m_messageLabelSelected)
            {
                mlabel_messages.ForeColor = System.Drawing.Color.Gray;
                mpanel_messageIndicator.BackColor = System.Drawing.Color.White;
            }
        }

        private void mlabel_messages_MouseEnter(object sender, EventArgs e)
        {
            if (!m_messageLabelSelected)
            {
                mlabel_messages.ForeColor = System.Drawing.Color.LightGray;
                mpanel_messageIndicator.BackColor = System.Drawing.Color.DarkGray;
            }
        }

        #region "Delegates"

        /// <summary>
        /// Delegate used for updating message window without cross-thread problems
        /// </summary>
        /// <param name="message"></param>
        private delegate void delegateInsertMessage(string message);

        #endregion

        #region Members

        /// <summary>
        /// Level of messages to show.  Messages greater than are ignored.
        /// </summary>
        private int m_messageLevel;

        /// <summary>
        /// Level of errors to show.  Errors greater than are ignored.
        /// </summary>
        private int m_errorLevel;

        private readonly object mobj_lockMessages;
        private readonly object mobj_lockErrors;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message filter level.  Messages greater than level are not shown.
        /// </summary>
        public int MessageLevel
        {
            get { return m_messageLevel; }
            set { m_messageLevel = value; }
        }

        /// <summary>
        /// Gets or sets the error filter level.  Errors greater than level are not shown.
        /// </summary>
        public int ErrorLevel
        {
            get { return m_errorLevel; }
            set { m_errorLevel = value; }
        }

        #endregion
    }
}