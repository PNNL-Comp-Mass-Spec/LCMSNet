using System;
using System.ComponentModel;

namespace LcmsNetSDK.Data
{
    [Serializable]
    public class classProposalUser
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

        public override string ToString()
        {
            return m_userID + ": " + (string.IsNullOrWhiteSpace(m_userName) ? "Undefined user" : m_userName);
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

        public string UserName
        {
            get { return m_userName; }
            set
            {
                if (m_userName != value)
                {
                    m_userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        private string m_userName;

        #endregion
    }
}