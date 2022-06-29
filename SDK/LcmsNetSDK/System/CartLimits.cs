using System;
using System.Globalization;

namespace LcmsNetSDK.System
{
    public static class CartLimits
    {
        /// <summary>
        /// The minimum sample volume for this system.
        /// </summary>
        public const double MinimumSampleVolume = 0.1;

        public static double MinimumVolume
        {
            get
            {
                var volume = LCMSSettings.GetParameter(LCMSSettings.PARAM_MINIMUMVOLUME, 0.0);
                return Math.Max(volume, MinimumSampleVolume);
            }
            set => LCMSSettings.SetParameter(LCMSSettings.PARAM_MINIMUMVOLUME, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
