using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LcmsNetData.Data
{
    [Serializable]
    public class UserIDPIDCrossReferenceEntry : INotifyPropertyChangedExt
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int userID;
        private string pid;

        public int UserID
        {
            get => userID;
            set => this.RaiseAndSetIfChanged(ref userID, value);
        }

        public string PID
        {
            get => pid;
            set => this.RaiseAndSetIfChanged(ref pid, value);
        }
    }
}