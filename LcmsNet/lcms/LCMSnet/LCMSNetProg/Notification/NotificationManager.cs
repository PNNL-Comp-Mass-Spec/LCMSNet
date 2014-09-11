using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetSDK.Notifications;

namespace LcmsNet.Notification
{
    public class NotificationBroadcaster
    {
        static NotificationBroadcaster m_manager;                
        public event EventHandler<NotifierChangedEventArgs> Added;
        public event EventHandler<NotifierChangedEventArgs> Removed;

        public NotificationBroadcaster()
        {
            Notifiers = new List<INotifier>();
        }
        public void AddNotifier(INotifier notifier)
        {
            if (Added != null)
                Added(this, new NotifierChangedEventArgs(notifier));
            
            Notifiers.Add(notifier);
        }
        public void RemoveNotifier(INotifier notifier)
        {
            if (Removed != null)
                Removed(this, new NotifierChangedEventArgs(notifier));

            if (Notifiers.Contains(notifier))
                Notifiers.Remove(notifier);

        }
        /// <summary>
        /// Grabs the list of all possible 
        /// </summary>
        /// <returns></returns>
        public List<INotifier> Notifiers {get; private set;}

        public static NotificationBroadcaster Manager
        {
            get
            {
                if (m_manager == null)
                    m_manager = new NotificationBroadcaster();
                return m_manager;
            }
        }
    }

    public class NotifierChangedEventArgs: EventArgs
    {
        public NotifierChangedEventArgs(INotifier notifier)
        {
            Notifier = notifier;
        }

        public INotifier Notifier {get; private set;}
    }
}
