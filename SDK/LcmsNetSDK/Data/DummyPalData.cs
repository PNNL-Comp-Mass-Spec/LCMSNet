using LcmsNetSDK.System;

namespace LcmsNetSDK.Data
{
    public class DummyPalData : IPalData
    {
        public DummyPalData()
        {
            Method = "std_01";
            PALTray = "";
            Well = CartLimits.CONST_DEFAULT_VIAL_NUMBER;
        }

        public int Well { get; set; }
        public string PALTray { get; set; }
        public string Method { get; set; }
    }
}
