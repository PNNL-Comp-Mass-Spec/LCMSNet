using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LcmsNetData.Data
{
    [Serializable]
    public class ExperimentData : INotifyPropertyChangedExt
    {
        #region Initialization

        public ExperimentData()
        {
            //Campaign = null;
            //Comment = null;

            Created = null;
            Experiment = null;
            ID = null;
            Organism = null;
            Reason = string.Empty;
            Request = null;
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

        private int? id;
        private string experimentName;
        private string researcher;
        private string reason;

        private string organism;
        private DateTime? created;
        private int? request;
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

        public int? ID
        {
            get => id;
            set => this.RaiseAndSetIfChanged(ref id, value);
        }

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

        [PersistenceSetting(PropertyGetOverrideMethod = nameof(CreatedPropertyReadOverride))]
        public DateTime? Created
        {
            get => created;
            set => this.RaiseAndSetIfChanged(ref created, value);
        }

        public int? Request
        {
            get => request;
            set => this.RaiseAndSetIfChanged(ref request, value);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            var experiment = string.IsNullOrWhiteSpace(experimentName) ? "Undefined experiment" : experimentName;

            if (id.HasValue)
                return id + ": " + experiment;

            return experiment;
        }

        private string ReasonPropertyReadOverride()
        {
            return Reason?.Replace("'", "") ?? "";
        }

        private DateTime CreatedPropertyReadOverride()
        {
            return Created ?? DateTime.MinValue;
        }

        #endregion
    }
}
