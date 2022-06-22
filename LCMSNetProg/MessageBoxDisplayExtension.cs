using System.Windows;
using System.Windows.Threading;

namespace LcmsNet
{
    /// <summary>
    /// Extension method for Window involving message box
    /// </summary>
    public static class MessageBoxDisplayExtension
    {
        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon; and that accepts a default message box result, complies with the specified options, and returns a result.
        /// </summary>
        /// <param name="window">Window for display context</param>
        /// <param name="messageBoxText">Text to display</param>
        /// <param name="caption">Title bar caption</param>
        /// <param name="button">Which button(s) to display</param>
        /// <param name="icon">Title bar icon</param>
        /// <param name="defaultResult">default result of the message box</param>
        /// <param name="options">message box options</param>
        /// <returns>Result specifying what button the user clicked</returns>
        public static MessageBoxResult ShowMessage(this Window window, string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None,
            MessageBoxResult defaultResult = MessageBoxResult.None, MessageBoxOptions options = MessageBoxOptions.None)
        {
            if (!window.Dispatcher.CheckAccess())
            {
                return window.Dispatcher.Invoke(() => ShowMessage(window, messageBoxText, caption, button, icon, defaultResult, options), DispatcherPriority.Normal);
            }

            return MessageBox.Show(window, messageBoxText, caption, button, icon, defaultResult, options);
        }
    }
}
