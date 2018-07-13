using System.ComponentModel;
using LcmsNetData;

namespace LcmsNetSDK.Data
{
    public class MobilePhase : INotifyPropertyChangedExt
    {
        public MobilePhase()
        {
        }

        public MobilePhase(string name, string comment)
        {
            Name = name;
            Comment = comment;
        }

        private string name;
        private string comment;

        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value, nameof(Name)); }
        }

        public string Comment
        {
            get { return comment; }
            set { this.RaiseAndSetIfChanged(ref comment, value, nameof(Comment)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}