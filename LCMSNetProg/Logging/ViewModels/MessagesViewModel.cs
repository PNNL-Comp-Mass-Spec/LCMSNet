using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using LcmsNetSDK.Logging;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Logging.ViewModels
{
    public class MessagesViewModel : ReactiveObject
    {
        // The maximum number of message entries in the messages or error messages lists
        private const int MaxMessagesEntryCount = 2000;
        // How many entries to remove when we hit the maximum
        private const int MaxMessagesRemoveCount = 30;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MessagesViewModel()
        {
            MessageLevel = ApplicationLogger.CONST_STATUS_LEVEL_USER;
            ErrorLevel = ApplicationLogger.CONST_STATUS_LEVEL_USER;

            messageList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var messageListBound).Subscribe();
            errorMessages.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var errorMessagesBound).Subscribe();
            MessageList = messageListBound;
            ErrorMessages = errorMessagesBound;

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
                messageList.Edit(list =>
                {
                    list.Insert(0, formatted);

                    if (list.Count > MaxMessagesEntryCount)
                    {
                        var removeStart = MaxMessagesEntryCount - MaxMessagesRemoveCount;
                        list.RemoveRange(removeStart, list.Count - removeStart);
                    }
                });
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
                InsertError(error);
            }
        }

        private void InsertError(ErrorLoggerArgs error)
        {
            ErrorPresent?.Invoke(this, EventArgs.Empty);
            // Use .Edit to make the whole set a single change (both synchronized, and single notification)
            errorMessages.Edit(sourceList =>
            {
                if (error.Exception != null)
                {
                    sourceList.Insert(0, FormatMessage(error.Exception.StackTrace));

                    var exMessages = new List<string>();
                    var ex = error.Exception;
                    while (ex != null)
                    {
                        exMessages.Add(ex.Message);
                        ex = ex.InnerException;
                    }

                    sourceList.Insert(0, FormatMessage(string.Join("\n", exMessages)));
                }

                sourceList.Insert(0, FormatMessage(error.Message));

                if (sourceList.Count > MaxMessagesEntryCount)
                {
                    var removeStart = MaxMessagesEntryCount - MaxMessagesRemoveCount;
                    sourceList.RemoveRange(removeStart, sourceList.Count - removeStart);
                }
            });
        }

        private void AcknowledgeErrors()
        {
            errorMessages.Clear();
            ErrorCleared?.Invoke(this, EventArgs.Empty);
        }

        private void ClearMessages()
        {
            messageList.Clear();
        }

        /// <summary>
        /// Level of messages to show.  Messages greater than are ignored.
        /// </summary>
        private int messageLevel;

        /// <summary>
        /// Level of errors to show.  Errors greater than are ignored.
        /// </summary>
        private int errorLevel;

        private readonly SourceList<string> messageList = new SourceList<string>();
        private readonly SourceList<string> errorMessages = new SourceList<string>();

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

        public ReadOnlyObservableCollection<string> MessageList { get; }
        public ReadOnlyObservableCollection<string> ErrorMessages { get; }

        public ReactiveCommand<Unit, Unit> AcknowledgeErrorsCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearMessagesCommand { get; }
    }
}
