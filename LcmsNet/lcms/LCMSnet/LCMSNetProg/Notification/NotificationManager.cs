using System;
using System.Collections.Generic;
using LcmsNetSDK.Notifications;

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

        public static NotificationBroadcaster Manager
        {
            get
            {
                if (m_manager == null)
                    m_manager = new NotificationBroadcaster();
                return m_manager;
            }
        }

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

        public INotifier Notifier { get; private set; }
    }
}