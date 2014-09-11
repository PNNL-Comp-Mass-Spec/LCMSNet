using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Data
{
	[Serializable]
	public class classProposalUser
		: classDataClassBase, INotifyPropertyChanged
	{
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
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


		#region Methods
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
