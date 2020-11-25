﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Windows.Data;
using LcmsNetData.Logging;
using LcmsNetData.System;
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
            MessageLevel = ApplicationLogger.CONST_STATUS_LEVEL_USER;
            ErrorLevel = ApplicationLogger.CONST_STATUS_LEVEL_USER;
            lockMessageList = new object();
            lockErrorList = new object();
            BindingOperations.EnableCollectionSynchronization(MessageList, lockMessageList);

            AcknowledgeErrorsCommand = ReactiveCommand.Create(AcknowledgeErrors);
            ClearMessagesCommand = ReactiveCommand.Create(ClearMessages);
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
            return $"{TimeKeeper.Instance.Now:MM/dd/yyyy HH:mm:ss.fff}: {message}";
        }

        /// <summary>
        /// Displays the error message to the screen.
        /// </summary>
        /// <param name="level">Filter for displaying messages.</param>
        /// <param name="message">Message to show user.</param>
        public void LogMessage(int level, MessageLoggerArgs message)
        {
            if (level <= messageLevel && message != null)
            {
                var formatted = FormatMessage(message.Message);
                RxApp.MainThreadScheduler.Schedule(x => InsertMessage(formatted));
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
                messageList.Insert(0, message);
            }
        }

        /// <summary>
        /// Displays error messages to the user.
        /// </summary>
        /// <param name="level">Level of errors to display.</param>
        /// <param name="error">Error to display.</param>
        public void LogError(int level, ErrorLoggerArgs error)
        {
            if (level <= errorLevel && error != null)
            {
                RxApp.MainThreadScheduler.Schedule(x => InsertError(error));
            }
        }

        private void InsertError(ErrorLoggerArgs error)
        {
            ErrorPresent?.Invoke(this, new EventArgs());
            lock (lockErrorList)
            {
                if (error.Exception != null)
                {
                    errorMessages.Insert(0, FormatMessage(error.Exception.StackTrace));

                    var exMessages = new List<string>();
                    var ex = error.Exception;
                    while (ex != null)
                    {
                        exMessages.Add(ex.Message);
                        ex = ex.InnerException;
                    }

                    errorMessages.Insert(0, FormatMessage(string.Join("\n", exMessages)));
                }

                errorMessages.Insert(0, FormatMessage(error.Message));
            }
        }

        private void AcknowledgeErrors()
        {
            using (errorMessages.SuppressChangeNotifications())
            {
                errorMessages.Clear();
            }
            ErrorCleared?.Invoke(this, new EventArgs());
        }

        private void ClearMessages()
        {
            using (messageList.SuppressChangeNotifications())
            {
                messageList.Clear();
            }
        }

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

        private readonly ReactiveList<string> messageList = new ReactiveList<string>();
        private readonly ReactiveList<string> errorMessages = new ReactiveList<string>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message filter level.  Messages greater than level are not shown.
        /// </summary>
        public int MessageLevel
        {
            get => messageLevel;
            set => this.RaiseAndSetIfChanged(ref messageLevel, value);
        }

        /// <summary>
        /// Gets or sets the error filter level.  Errors greater than level are not shown.
        /// </summary>
        public int ErrorLevel
        {
            get => errorLevel;
            set => this.RaiseAndSetIfChanged(ref errorLevel, value);
        }

        /// <summary>
        /// Error message importance level
        /// </summary>
        public LogLevel ErrorLogLevel
        {
            get => ApplicationLogger.ConvertIntToLogLevel(ErrorLevel);
            set => ErrorLevel = (int)value;
        }

        /// <summary>
        /// Status message importance level
        /// </summary>
        public LogLevel MessageLogLevel
        {
            get => ApplicationLogger.ConvertIntToLogLevel(MessageLevel);
            set => MessageLevel = (int)value;
        }

        public IReadOnlyReactiveList<string> MessageList => messageList;
        public IReadOnlyReactiveList<string> ErrorMessages => errorMessages;

        public ReactiveCommand<Unit, Unit> AcknowledgeErrorsCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearMessagesCommand { get; }

        #endregion
    }
}
