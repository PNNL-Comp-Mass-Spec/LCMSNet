using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LcmsNetData;
using LcmsNetData.Data;

namespace LcmsNet.IO.DMS.Data
{
    [Serializable]
    public class ExperimentData : INotifyPropertyChangedExt
    {
        #region Initialization

        public ExperimentData()
        {
            //Campaign = null;
            //Comment = null;

            Created = DateTime.MinValue;
            Experiment = null;
            ID = 0;
            Organism = null;
            Reason = string.Empty;
            Request = 0;
            Researcher = null;
        }

        #endregion

        #region INotifyPropertyChangedExt

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Attributes

        private int id;
        private string experimentName;
        private string researcher;
        private string reason;

        private string organism;
        private DateTime created;
        private int request;
        //private string comment;
        //private string campaign;

        #endregion

        #region Properties

        //public string Campaign
        //{
        //    get => campaign;
        //    set => this.RaiseAndSetIfChanged(ref campaign, value);
        //}

        //public string Comment
        //{
        //    get => comment;
        //    set => this.RaiseAndSetIfChanged(ref comment, value);
        //}

        [PersistenceSetting(IsUniqueColumn = true)]
        public int ID
        {
            get => id;
            set => this.RaiseAndSetIfChanged(ref id, value);
        }

        [PersistenceSetting(IsUniqueColumn = true)]
        public string Experiment
        {
            get => experimentName;
            set => this.RaiseAndSetIfChanged(ref experimentName, value);
        }

        public string Researcher
        {
            get => researcher;
            set => this.RaiseAndSetIfChanged(ref researcher, value);
        }

        [PersistenceSetting(PropertyGetOverrideMethod = nameof(ReasonPropertyReadOverride))]
        public string Reason
        {
            get => reason;
            // Make sure value is not null
            set => this.RaiseAndSetIfChanged(ref reason, value ?? string.Empty);
        }

        public string Organism
        {
            get => organism;
            set => this.RaiseAndSetIfChanged(ref organism, value);
        }

        public DateTime Created
        {
            get => created;
            set => this.RaiseAndSetIfChanged(ref created, value);
        }

        [PersistenceSetting(IsUniqueColumn = true)]
        public int Request
        {
            get => request;
            set => this.RaiseAndSetIfChanged(ref request, value);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            var experiment = string.IsNullOrWhiteSpace(experimentName) ? "Undefined experiment" : experimentName;

            return id + ": " + experiment;
        }

        private string ReasonPropertyReadOverride()
        {
            return Reason?.Replace("'", "") ?? "";
        }

        #endregion
    }
}
