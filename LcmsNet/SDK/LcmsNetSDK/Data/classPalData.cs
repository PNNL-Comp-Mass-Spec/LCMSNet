//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/21/2009
//
// Updates:
// - 03/10/2010 (DAC) - Added wellplate field, changed vial field to well
//*********************************************************************************************************

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LcmsNetSDK;

namespace LcmsNetDataClasses.Data
{
    [Serializable]
    public class classPalData : classDataClassBase, ICloneable, INotifyPropertyChangedExt
    {
        #region "Constructors"

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classPalData()
        {
            Method = CONST_METHOD_NAME;
            palTray = "";
            well = CONST_DEFAULT_VIAL_NUMBER;
            WellPlate = "";
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Returns a new object reference to a cloned copy of this PAL data.
        /// </summary>
        /// <returns>A new object reference as a copy of this.</returns>
        public object Clone()
        {
            var newData = new classPalData
            {
                PALTray = palTray,
                Method = Method,
                Well = well,
                WellPlate = WellPlate
            };

            return newData;
        }

        #endregion

        //*********************************************************************************************************
        //Class that encapsulates the PAL data.
        //**********************************************************************************************************

        #region "Constants"

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

        #endregion

        #region "Class variables"

        /// <summary>
        /// Name of the PAL tray to use.
        /// </summary>
        private string palTray;

        /// <summary>
        /// Vial index to use.
        /// </summary>
        private int well;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the vial number to pull sample from.
        /// </summary>
        public int Well
        {
            get { return well; }
            set
            {
                if (value < CONST_MIN_WELLPLATE || CONST_MAX_WELLPLATE < value)
                {
                    // Say it changed, to force UI to refresh to the unchanged value
                    this.OnPropertyChanged();
                    return;
                }
                this.RaiseAndSetIfChanged(ref well, value);
            }
        }

        /// <summary>
        /// Name of the PAL tray to use.
        /// </summary>
        public string PALTray
        {
            get { return palTray; }
            set { this.RaiseAndSetIfChanged(ref palTray, value); }
        }

        /// <summary>
        /// Name of the PAL method to run.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The Wellplate name that is stored in DMS.
        /// </summary>
        public string WellPlate { get; set; }

        #endregion

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

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
