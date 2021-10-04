using System;
using System.ComponentModel;

namespace LcmsNetSDK.Data
{
    /// <summary>
    /// Encapsulates data for an autosampler
    /// </summary>
    [Serializable]
    public class PalData : INotifyPropertyChangedExt
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PalData()
        {
            Method = CONST_METHOD_NAME;
            palTray = "";
            well = CONST_DEFAULT_VIAL_NUMBER;
            WellPlate = "";
        }

        /// <summary>
        /// Returns a new object reference to a cloned copy of this PAL data.
        /// </summary>
        /// <returns>A new object reference as a copy of this.</returns>
        public PalData Clone()
        {
            var newData = new PalData
            {
                PALTray = palTray,
                Method = Method,
                Well = well,
                WellPlate = WellPlate
            };

            return newData;
        }

        private const string CONST_METHOD_NAME = "std_01";

        /// <summary>
        /// Default sample vial number.  This should be invalid and force the user to update the sample information before running.
        /// </summary>
        public const int CONST_DEFAULT_VIAL_NUMBER = 0;

        /// <summary>
        /// Minimum wellplate number.
        /// </summary>
        public const int CONST_MIN_WELLPLATE = 1;

        /// <summary>
        /// Maximum wellplate number.
        /// </summary>
        public const int CONST_MAX_WELLPLATE = 1250;

        /// <summary>
        /// Name of the PAL tray to use.
        /// </summary>
        private string palTray;

        /// <summary>
        /// Vial index to use.
        /// </summary>
        private int well;

        /// <summary>
        /// Gets or sets the vial number to pull sample from.
        /// </summary>
        public int Well
        {
            get => well;
            set
            {
                if (value < CONST_MIN_WELLPLATE || CONST_MAX_WELLPLATE < value)
                {
                    // Say it changed, to force UI to refresh to the unchanged value
                    this.RaisePropertyChanged(nameof(Well));
                    return;
                }
                this.RaiseAndSetIfChanged(ref well, value, nameof(Well));
            }
        }

        /// <summary>
        /// Name of the PAL tray to use.
        /// </summary>
        public string PALTray
        {
            get => palTray;
            set => this.RaiseAndSetIfChanged(ref palTray, value, nameof(PALTray));
        }

        /// <summary>
        /// Name of the PAL method to run.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The Wellplate name that is stored in DMS.
        /// </summary>
        public string WellPlate { get; set; }

        //  /// <summary>

        //#region Well Plate - Vial Conversions
        //  /// Converts a given vial number to a well plate index.
        //  /// </summary>
        //  /// <param name="vialNumber">Number to convert to a well plate location.</param>
        //  /// <returns></returns>
        //  public static string ConvertVialToWellPlateLocation(int vialNumber)
        //  {
        //      return vialNumber.ToString();
        //  }
        //  #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

