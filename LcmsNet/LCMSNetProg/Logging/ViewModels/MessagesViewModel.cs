using System;
using System.Windows.Data;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK;
using ReactiveUI;

namespace LcmsNet.Logging.ViewModels
{
    public class MessagesViewModel : ReactiveObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MessagesViewModel()
        {
            MessageLevel = classApplicationLogger.CONST_STATUS_LEVEL_USER;
            ErrorLevel = classApplicationLogger.CONST_STATUS_LEVEL_USER;
            lockMessageList = new object();
            lockErrorList = new object();
            ErrorMessages = "";
            BindingOperations.EnableCollectionSynchronization(MessageList, lockMessageList);
            SetupCommands();
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
                DateTime.UtcNow.Subtract(TimeKeeper.Instance.TimeZone.BaseUtcOffset).ToLongDateString(),
                DateTime.UtcNow.Subtract(TimeKeeper.Instance.TimeZone.BaseUtcOffset).TimeOfDay,
                message);
        }

        /// <summary>
        /// Displays the error message to the screen.
        /// </summary>
        /// <param name="level">Filter for displaying messages.</param>
        /// <param name="message">Message to show user.</param>
        public void ShowMessage(int level, classMessageLoggerArgs message)
        {
            if (level <= messageLevel && message != null)
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
            lock (lockMessageList)
            {
                MessageList.Insert(0, message);
            }
        }

        /// <summary>
        /// Displays error messages to the user.
        /// </summary>
        /// <param name="level">Level of errors to display.</param>
        /// <param name="error">Error to display.</param>
        public void ShowErrors(int level, classErrorLoggerArgs error)
        {
            if (level <= errorLevel && error != null)
            {
                ErrorPresent?.Invoke(this, new EventArgs());
                var exceptions = "";
                lock (lockErrorList)
                {
                    if (error.Exception != null)
                    {
                        ErrorMessages = FormatMessage(error.Exception.StackTrace) + "\n" + ErrorMessages;

                        var ex = error.Exception;
                        while (ex != null)
                        {
                            exceptions += ex.Message + "\n";
                            ex = ex.InnerException;
                        }

                        ErrorMessages = FormatMessage(exceptions) + "\n" + ErrorMessages;
                    }

                    ErrorMessages = FormatMessage(error.Message) + "\n" + ErrorMessages;
                }
            }
        }

        private void AcknowledgeErrors()
        {
            ErrorMessages = "";
            ErrorCleared?.Invoke(this, new EventArgs());
        }

        private void ClearMessages()
        {
            using (MessageList.SuppressChangeNotifications())
            {
                MessageList.Clear();
            }
        }

        #region Commands

        public ReactiveCommand AcknowledgeErrorsCommand { get; private set; }
        public ReactiveCommand ClearMessagesCommand { get; private set; }

        private void SetupCommands()
        {
            AcknowledgeErrorsCommand = ReactiveCommand.Create(() => this.AcknowledgeErrors());
            ClearMessagesCommand = ReactiveCommand.Create(() => ClearMessages());
        }

        #endregion

        #region Members

        /// <summary>
        /// Level of messages to show.  Messages greater than are ignored.
        /// </summary>
        private int messageLevel;

        /// <summary>
        /// Level of errors to show.  Errors greater than are ignored.
        /// </summary>
        private int errorLevel;

        private readonly object lockMessageList;
        private readonly object lockErrorList;

        private string errorMessages = "";
        private readonly ReactiveList<string> messageList = new ReactiveList<string>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message filter level.  Messages greater than level are not shown.
        /// </summary>
        public int MessageLevel
        {
            get { return messageLevel; }
            set { this.RaiseAndSetIfChanged(ref messageLevel, value); }
        }

        /// <summary>
        /// Gets or sets the error filter level.  Errors greater than level are not shown.
        /// </summary>
        public int ErrorLevel
        {
            get { return errorLevel; }
            set { this.RaiseAndSetIfChanged(ref errorLevel, value); }
        }

        public string ErrorMessages
        {
            get { return errorMessages; }
            private set { this.RaiseAndSetIfChanged(ref errorMessages, value); }
        }

        public ReactiveList<string> MessageList => messageList;

        #endregion
    }
}
