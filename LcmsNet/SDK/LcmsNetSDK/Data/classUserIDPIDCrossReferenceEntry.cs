using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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