using System.ComponentModel;

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
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string Comment
        {
            get => comment;
            set => this.RaiseAndSetIfChanged(ref comment, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
