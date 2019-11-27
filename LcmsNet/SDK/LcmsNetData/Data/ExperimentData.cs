using System;
using System.ComponentModel;

namespace LcmsNetData.Data
{
    [Serializable]
    public class ExperimentData : INotifyPropertyChanged
    {
        #region Initialization

        public ExperimentData()
        {
            //this.Campaign     = null;
            //this.CellCulturies    = null;
            //this.Comment      = null;
            //this.Concentration    = null;

            //this.Container        = null;
            Created = null;
            //this.Enzyme           = null;
            Experiment = null;

            ID = null;
            //this.Labeling     = null;
            //this.Location     = null;
            //this.Notebook     = null;

            Organism = null;
            //this.Postdigest       = null;
            //this.Predigest        = null;
            Reason = null;

            Request = null;
            Researcher = null;
            //this.Well         = null;
            //this.Wellplate        = null;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Attributes

        private int? m_id;
        private string m_experiment;
        private string m_researcher;
        private string m_reason;

        private string m_organism;
        private DateTime? m_created;
        private int? m_request;
        //private string        m_comment;

        //private string        m_well;
        //private string        m_concentration;
        //private string        m_campaign;
        //private string        m_cellCultures;

        //private string        m_enzyme;
        //private string        m_notebook;
        //private string        m_labeling;
        //private string        m_predigest;

        //private string        m_postdigest;
        //private string        m_container;
        //private string        m_location;
        //private string        m_wellplate;

        #endregion

        #region Properties

        //public string Concentration
        //{
        //    get { return m_concentration; }
        //    set
        //    {
        //        if (m_concentration != value)
        //        {
        //            m_concentration = value;
        //            OnPropertyChanged("Concentration");
        //        }
        //    }
        //}

        //public string Campaign
        //{
        //    get { return m_campaign; }
        //    set
        //    {
        //        if (m_campaign != value)
        //        {
        //            m_campaign = value;
        //            OnPropertyChanged("Campaign");
        //        }
        //    }
        //}

        //public string CellCulturies
        //{
        //    get { return m_cellCultures; }
        //    set
        //    {
        //        if (m_cellCultures != value)
        //        {
        //            m_cellCultures = value;
        //            OnPropertyChanged("CellCulturies");
        //        }
        //    }
        //}

        //public string Enzyme
        //{
        //    get { return m_enzyme; }
        //    set
        //    {
        //        if (m_enzyme != value)
        //        {
        //            m_enzyme = value;
        //            OnPropertyChanged("Enzyme");
        //        }
        //    }
        //}

        //public string Notebook
        //{
        //    get { return m_notebook; }
        //    set
        //    {
        //        if (m_notebook != value)
        //        {
        //            m_notebook = value;
        //            OnPropertyChanged("Notebook");
        //        }
        //    }
        //}

        //public string Labeling
        //{
        //    get { return m_labeling; }
        //    set
        //    {
        //        if (m_labeling != value)
        //        {
        //            m_labeling = value;
        //            OnPropertyChanged("Labeling");
        //        }
        //    }
        //}

        //public string Predigest
        //{
        //    get { return m_predigest; }
        //    set
        //    {
        //        if (m_predigest != value)
        //        {
        //            m_predigest = value;
        //            OnPropertyChanged("Predigest");
        //        }
        //    }
        //}

        //public string Postdigest
        //{
        //    get { return m_postdigest; }
        //    set
        //    {
        //        if (m_postdigest != value)
        //        {
        //            m_postdigest = value;
        //            OnPropertyChanged("Postdigest");
        //        }
        //    }
        //}

        //public string Container
        //{
        //    get { return m_container; }
        //    set
        //    {
        //        if (m_container != value)
        //        {
        //            m_container = value;
        //            OnPropertyChanged("Container");
        //        }
        //    }
        //}

        //public string Location
        //{
        //    get { return m_location; }
        //    set
        //    {
        //        if (m_location != value)
        //        {
        //            m_location = value;
        //            OnPropertyChanged("Location");
        //        }
        //    }
        //}

        //public string Wellplate
        //{
        //    get { return m_wellplate; }
        //    set
        //    {
        //        if (m_wellplate != value)
        //        {
        //            m_wellplate = value;
        //            OnPropertyChanged("Wellplate");
        //        }
        //    }
        //}

        //public string Well
        //{
        //    get { return m_well; }
        //    set
        //    {
        //        if (m_well != value)
        //        {
        //            m_well = value;
        //            OnPropertyChanged("Well");
        //        }
        //    }
        //}

        //public string Comment
        //{
        //    get { return m_comment; }
        //    set
        //    {
        //        if (m_comment != value)
        //        {
        //            m_comment = value;
        //            OnPropertyChanged("Comment");
        //        }
        //    }
        //}

        public int? ID
        {
            get { return m_id; }
            set
            {
                if (m_id != value)
                {
                    m_id = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        public string Experiment
        {
            get { return m_experiment; }
            set
            {
                if (m_experiment != value)
                {
                    m_experiment = value;
                    OnPropertyChanged("Experiment");
                }
            }
        }

        public string Researcher
        {
            get { return m_researcher; }
            set
            {
                if (m_researcher != value)
                {
                    m_researcher = value;
                    OnPropertyChanged("Researcher");
                }
            }
        }

        [PersistenceSetting(PropertyGetOverrideMethod = nameof(ReasonPropertyReadOverride))]
        public string Reason
        {
            get { return m_reason; }
            set
            {
                if (m_reason != value)
                {
                    // Make sure value is not null
                    value = value ?? string.Empty;
                    m_reason = value;
                    OnPropertyChanged("Reason");
                }
            }
        }

        public string Organism
        {
            get { return m_organism; }
            set
            {
                if (m_organism != value)
                {
                    m_organism = value;
                    OnPropertyChanged("Organism");
                }
            }
        }

        [PersistenceSetting(PropertyGetOverrideMethod = nameof(CreatedPropertyReadOverride))]
        public DateTime? Created
        {
            get { return m_created; }
            set
            {
                if (m_created != value)
                {
                    m_created = value;
                    OnPropertyChanged("Created");
                }
            }
        }

        public int? Request
        {
            get { return m_request; }
            set
            {
                if (m_request != value)
                {
                    m_request = value;
                    OnPropertyChanged("Request");
                }
            }
        }

        #endregion

        #region Methods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            var experiment = string.IsNullOrWhiteSpace(m_experiment) ? "Undefined experiment" : m_experiment;

            if (m_id.HasValue)
                return m_id + ": " + experiment;

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
