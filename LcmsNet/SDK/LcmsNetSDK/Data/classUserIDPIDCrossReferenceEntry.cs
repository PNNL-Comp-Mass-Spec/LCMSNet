using System;
using System.ComponentModel;

namespace LcmsNetDataClasses.Data
{
    [Serializable]
    public class classUserIDPIDCrossReferenceEntry
        : classDataClassBase, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Properties

        public int UserID
        {
            get { return m_userID; }
            set
            {
                if (m_userID != value)
                {
                    m_userID = value;
                    OnPropertyChanged("UserID");
                }
            }
        }

        private int m_userID;

        public string PID
        {
            get { return m_pid; }
            set
            {
                if (m_pid != value)
                {
                    m_pid = value;
                    OnPropertyChanged("PID");
                }
            }
        }

        private string m_pid;

        #endregion
    }
}