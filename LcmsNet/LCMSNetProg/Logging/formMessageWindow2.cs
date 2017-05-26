using System;
using System.Windows.Forms;
using LcmsNet.Logging.ViewModels;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.Logging
{
    public partial class formMessageWindow2 : Form
    {
        private MessagesViewModel messagesViewModel;

        /// <summary>
        /// Constructor.
        /// </summary>
        public formMessageWindow2()
        {
            InitializeComponent();

            messagesViewModel = new MessagesViewModel();
            messagesView.DataContext = messagesViewModel;

            messagesViewModel.ErrorCleared += MessagesErrorCleared;
            messagesViewModel.ErrorPresent += MessagesErrorPresent;
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
        /// Displays the error message to the screen.
        /// </summary>
        /// <param name="level">Filter for displaying messages.</param>
        /// <param name="message">Message to show user.</param>
        public void ShowMessage(int level, classMessageLoggerArgs message)
        {
            messagesViewModel.ShowMessage(level, message);
        }

        private void MessagesErrorPresent(object sender, EventArgs e)
        {
            ErrorPresent?.Invoke(sender, e);
        }

        private void MessagesErrorCleared(object sender, EventArgs e)
        {
            ErrorCleared?.Invoke(sender, e);
        }

        /// <summary>
        /// Displays error messages to the user.
        /// </summary>
        /// <param name="level">Level of errors to display.</param>
        /// <param name="error">Error to display.</param>
        public void ShowErrors(int level, classErrorLoggerArgs error)
        {
            messagesViewModel.ShowErrors(level, error);
        }
    }
}
