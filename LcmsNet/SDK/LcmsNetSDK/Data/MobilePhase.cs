using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        public string Comment
        {
            get { return comment; }
            set { this.RaiseAndSetIfChanged(ref comment, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

#if DotNET4
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

#else
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endif
    }
}