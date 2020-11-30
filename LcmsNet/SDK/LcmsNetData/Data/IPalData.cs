using System;

namespace LcmsNetData.Data
{
    /// <summary>
    /// Interface for storing PAL data. Only needed (available in LcmsNetData) for populating this data when reading samples from DMS, for LCMSNet.
    /// </summary>
    public interface IPalData : ICloneable, INotifyPropertyChangedExt
    {
        /// <summary>
        /// Gets or sets the vial number to pull sample from.
        /// </summary>
        int Well { get; set; }

        /// <summary>
        /// Name of the PAL tray to use.
        /// </summary>
        string PALTray { get; set; }

        /// <summary>
        /// Name of the PAL method to run.
        /// </summary>
        string Method { get; set; }

        /// <summary>
        /// The Wellplate name that is stored in DMS.
        /// </summary>
        string WellPlate { get; set; }
    }
}
