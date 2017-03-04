using System;
using System.ComponentModel;

namespace LcmsNetDataClasses
{
    [Serializable]
    public class classLCColumn
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
            return string.IsNullOrWhiteSpace(m_lcColumn) ? "Undefined column" : m_lcColumn;
        }

        #endregion

        #region Initialization

        #endregion

        #region Properties

        public string LCColumn
        {
            get { return m_lcColumn; }
            set
            {
                if (m_lcColumn != value)
                {
                    m_lcColumn = value;
                    OnPropertyChanged("LCColumn");
                }
            }
        }

        private string m_lcColumn;

        public string State
        {
            get { return m_state; }
            set
            {
                if (m_state != value)
                {
                    m_state = value;
                    OnPropertyChanged("State");
                }
            }
        }

        private string m_state;

        #endregion
    }
}