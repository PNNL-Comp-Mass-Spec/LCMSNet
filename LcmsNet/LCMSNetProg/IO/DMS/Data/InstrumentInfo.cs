using System;
using System.ComponentModel;
using LcmsNetData;
using LcmsNetData.Data;

namespace LcmsNet.IO.DMS.Data
{
    /// <summary>
    /// Class to hold data about the instrument connected to the LC cart
    /// </summary>
    ///
    [Serializable]
    public class InstrumentInfo : IEquatable<InstrumentInfo>, INotifyPropertyChangedExt, ICloneable
    {
        private string captureMethod;

        #region "Properties"

        /// <summary>
        /// Instrument name as used in DMS
        /// </summary>
        [PersistenceSetting(IsUniqueColumn = true)]
        public string DMSName { get; set; }

        /// <summary>
        /// User-friendly name used for pick lists
        /// </summary>
        /// <remarks>Instrument name, then a space, then the instrument description</remarks>
        [PersistenceSetting(IsUniqueColumn = true)]
        public string CommonName { get; set; }

        /// <summary>
        /// Instrument grouping in DMS (a rough 'instrument class/type' specification)
        /// </summary>
        public string InstrumentGroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the method used to capture data from this instrument
        /// </summary>
        public string CaptureMethod
        {
            get => captureMethod;
            set => this.RaiseAndSetIfChanged(ref captureMethod, value, nameof(CaptureMethod));
        }

        /// <summary>
        /// Gets or sets the instrument status
        /// </summary>
        /// <remarks>Status will be active, inactive, or offline</remarks>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the name of the computer that holds the data for the instrument
        /// </summary>
        /// <remarks>May contain a domain suffix, for example VPro02.bionet</remarks>
        [PersistenceSetting(IsUniqueColumn = true)]
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the name of the shared folder for retrieving the instrument data
        /// </summary>
        /// <remarks>Typically a single folder name, like ProteomicsData\ but might contain subfolders, e.g. UserData\Nikola\AMOLF\</remarks>
        public string SharePath { get; set; }

        #endregion

        /// <summary>
        /// Clone - make a deep copy
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var newInstrumentInfo = new InstrumentInfo();

            newInstrumentInfo.DMSName = DMSName;
            newInstrumentInfo.CommonName = CommonName;
            newInstrumentInfo.InstrumentGroup = InstrumentGroup;
            newInstrumentInfo.CaptureMethod = CaptureMethod;
            newInstrumentInfo.Status = Status;
            newInstrumentInfo.HostName = HostName;
            newInstrumentInfo.SharePath = SharePath;

            return newInstrumentInfo;
        }

        #region Methods

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(DMSName))
                return DMSName;

            if (!string.IsNullOrWhiteSpace(CommonName))
                return CommonName;

            return "Undefined instrument";
        }

        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(InstrumentInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(DMSName, other.DMSName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InstrumentInfo) obj);
        }

        public override int GetHashCode()
        {
            return (DMSName != null ? DMSName.GetHashCode() : 0);
        }
    }
}
