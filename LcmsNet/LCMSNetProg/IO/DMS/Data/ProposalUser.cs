using System;
using System.ComponentModel;
using LcmsNetData;

namespace LcmsNet.IO.DMS.Data
{
    [Serializable]
    public class ProposalUser : IEquatable<ProposalUser>, INotifyPropertyChangedExt
    {
        private int userID;
        private string userName;

        public ProposalUser(int userId, string userName)
        {
            UserID = userId;
            UserName = userName;
        }

        public ProposalUser()
        {
        }

        public int UserID
        {
            get => userID;
            set => this.RaiseAndSetIfChanged(ref userID, value);
        }

        public string UserName
        {
            get => userName;
            set => this.RaiseAndSetIfChanged(ref userName, value);
        }

        public override string ToString()
        {
            return userID + ": " + (string.IsNullOrWhiteSpace(userName) ? "Undefined user" : userName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(ProposalUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return userID == other.userID && userName == other.userName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProposalUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (userID * 397) ^ (userName != null ? userName.GetHashCode() : 0);
            }
        }
    }
}