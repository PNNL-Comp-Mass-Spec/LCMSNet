using System;
using System.Collections.Generic;
using LcmsNetSDK.System;

namespace LcmsNet.Notification
{
    public class NotificationBroadcaster
    {
        static NotificationBroadcaster m_manager;

        public NotificationBroadcaster()
        {
            Notifiers = new List<INotifier>();
        }

        /// <summary>
        /// Grabs the list of all possible
        /// </summary>
        /// <returns></returns>
        public List<INotifier> Notifiers { get; }

        public static NotificationBroadcaster Manager => m_manager ?? (m_manager = new NotificationBroadcaster());

        public event EventHandler<NotifierChangedEventArgs> Added;
        public event EventHandler<NotifierChangedEventArgs> Removed;

        public void AddNotifier(INotifier notifier)
        {
            Added?.Invoke(this, new NotifierChangedEventArgs(notifier));

            Notifiers.Add(notifier);
        }

        public void RemoveNotifier(INotifier notifier)
        {
            Removed?.Invoke(this, new NotifierChangedEventArgs(notifier));

            if (Notifiers.Contains(notifier))
                Notifiers.Remove(notifier);
        }
    }

    public class NotifierChangedEventArgs : EventArgs
    {
        public NotifierChangedEventArgs(INotifier notifier)
        {
            Notifier = notifier;
        }

        public INotifier Notifier { get; }
    }
}
