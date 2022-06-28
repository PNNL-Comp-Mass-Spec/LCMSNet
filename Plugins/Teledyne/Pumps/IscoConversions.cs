using System;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Utility class for converting between units and setting display captions in ISCO pump classes
    /// </summary>
    public static class IscoConversions
    {
        static IscoFlowUnits m_FlowUnits = IscoFlowUnits.ul_min;
        static IscoPressureUnits menu_PressUnits = IscoPressureUnits.psi;

        public static IscoFlowUnits FlowUnits
        {
            get => m_FlowUnits;
            set => m_FlowUnits = value;
        }

        public static IscoPressureUnits PressUnits
        {
            get => menu_PressUnits;
            set => menu_PressUnits = value;
        }

        /// <summary>
        /// Converts flow represented by G&amp; return string to user units
        /// (Overload to use default units specifier stored in class)
        /// </summary>
        /// <param name="inpFlow">Input flow string</param>
        /// <returns>Flow in user units; -1000 on failure</returns>
        public static double ConvertFlowFromString(string inpFlow)
        {
            return ConvertFlowFromString(inpFlow, m_FlowUnits);
        }

        /// <summary>
        /// Converts flow represented by G&amp; return string to user units
        /// (Overload allowing user to specify units)
        /// </summary>
        /// <param name="inpFlow">Input flow string (liters/min * 10E10)</param>
        /// <param name="units">Units specifier</param>
        /// <returns>Flow in user units; -1000 on failure</returns>
        public static double ConvertFlowFromString(string inpFlow, IscoFlowUnits units)
        {
            // Convert input to double
            double tmpVal;
            try
            {
                // Convert to double
                tmpVal = double.Parse(inpFlow);
                // Convert to liters/minute
                tmpVal = tmpVal / Math.Pow(10, 10);
            }
            catch
            {
                return -1000;
            }

            // Convert to units pump is using
            double retVal;
            switch (units)
            {
                case IscoFlowUnits.ml_hour:
                    retVal = tmpVal * 1000D * 60D;  // 1000 mL/L; 60 min/hr
                    break;
                case IscoFlowUnits.ml_min:
                    retVal = tmpVal * 1000D;    // 1000 mL/L
                    break;
                case IscoFlowUnits.ul_hr:
                    retVal = tmpVal * Math.Pow(10, 6) * 60D;    // 10E6 uL/L; 60 min/hr
                    break;
                case IscoFlowUnits.ul_min:
                    retVal = tmpVal * Math.Pow(10, 6);  // 10E6 uL/L
                    break;
                default:
                    retVal = -1000D;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Converts pressure represented by G&amp; return string to user units
        /// (Overload to use default units specifier stored in class)
        /// </summary>
        /// <param name="inpPress">Input pressure string (PSI * 5)</param>
        /// <param name="model"></param>
        /// <returns>Pressure in user units; -1000 on failure</returns>
        public static double ConvertPressFromString(string inpPress, ISCOModel model)
        {
            return ConvertPressFromString(inpPress, menu_PressUnits, model);
        }

        /// <summary>
        /// Converts pressure represented by G&amp; return string to user units
        /// (Overload allowing user to specify units)
        /// </summary>
        /// <param name="inpPress">Input pressure string (PSI * 5)</param>
        /// <param name="units">Units specifier</param>
        /// <param name="model"></param>
        /// <returns>Pressure in user units; -1000 on failure</returns>
        public static double ConvertPressFromString(string inpPress, IscoPressureUnits units, ISCOModel model)
        {
            if (model == ISCOModel.Unknown)
            {
                //this is an error.
                return -1000;
            }
            double tmpVal;
            try
            {
                // Convert input string to double
                tmpVal = double.Parse(inpPress);

                // Convert to PSI, return value from pump is PSI * 2.5 if model is 65D, PSI * 5 if 100D
                if (model == ISCOModel.ISCO65D)
                {
                    tmpVal = tmpVal / 2.5D;
                }
                else if (model == ISCOModel.ISCO100D)
                {
                    tmpVal = tmpVal / 5D;
                }
            }
            catch
            {
                return -1000;
            }

            // Convert to units used by pump
            double retVal;
            switch (units)
            {
                case IscoPressureUnits.atm:
                    retVal = tmpVal / 14.695;   // 1 atm/14.695 psi
                    break;
                case IscoPressureUnits.bar:
                    retVal = tmpVal / 14.504;   // 1 bar/14.504 psi
                    break;
                case IscoPressureUnits.kPa:
                    retVal = tmpVal * 6.895;    // 1 psi = 6.895 kPa
                    break;
                case IscoPressureUnits.psi:
                    retVal = tmpVal;
                    break;
                default:
                    retVal = -1000;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Returns volume in liters from a string input
        /// </summary>
        /// <param name="inpVol">Input string representing level as Liters * 10E9</param>
        /// <returns>Volume in liters if successful; -999 otherwise</returns>
        public static double ConvertVolumeFromString(string inpVol)
        {
            try
            {
                var tmpVal = double.Parse(inpVol);
                return (tmpVal / Math.Pow(10, 6));  // Data from pump is in liters; this converts to mL
            }
            catch
            {
                return -999;
            }
        }

        /// <summary>
        /// Gets string representing current flow units setting
        /// (Overload using default stored in class)
        /// </summary>
        /// <returns>String specifying flow units; Empty string on error</returns>
        public static string GetFlowUnitsString()
        {
            return GetFlowUnitsString(m_FlowUnits);
        }

        /// <summary>
        /// Gets string representing current flow units setting
        /// (Overload allowing specification of flow units setting)
        /// </summary>
        /// <param name="units">Units specifier enum</param>
        /// <returns>String specifying flow units; Empty string on error</returns>
        public static string GetFlowUnitsString(IscoFlowUnits units)
        {
            var retStr = "";
            switch (units)
            {
                case IscoFlowUnits.ml_hour:
                    retStr = "mL/hr";
                    break;
                case IscoFlowUnits.ml_min:
                    retStr = "mL/min";
                    break;
                case IscoFlowUnits.ul_hr:
                    retStr = "uL/hr";
                    break;
                case IscoFlowUnits.ul_min:
                    retStr = "uL/min";
                    break;
                case IscoFlowUnits.error:
                    retStr = "Err";
                    break;
            }

            return retStr;
        }

        /// <summary>
        /// Gets string representing current pressure units setting
        /// (Overload using default stored in class)
        /// </summary>
        /// <returns>String specifying pressure units; Empty string on error</returns>
        public static string GetPressUnitsString()
        {
            return GetPressUnitsString(menu_PressUnits);
        }

        /// <summary>
        /// Gets string representing current pressure units setting
        /// (Overload allowing specification of pressure units setting)
        /// </summary>
        /// <param name="units">Units specifier enum</param>
        /// <returns>String specifying pressure units; Empty string on error</returns>
        public static string GetPressUnitsString(IscoPressureUnits units)
        {
            var retStr = "";
            switch (units)
            {
                case IscoPressureUnits.atm:
                    retStr = "atm";
                    break;
                case IscoPressureUnits.bar:
                    retStr = "bar";
                    break;
                case IscoPressureUnits.kPa:
                    retStr = "kPa";
                    break;
                case IscoPressureUnits.psi:
                    retStr = "psi";
                    break;
                case IscoPressureUnits.error:
                    retStr = "Err";
                    break;
            }

            return retStr;
        }

        /// <summary>
        /// Converts the flow string returned from the pump to an enum
        /// </summary>
        /// <param name="inpStr">Input string from pump</param>
        /// <returns>Enum specified by input string</returns>
        public static IscoFlowUnits ConvertFlowStrToEnum(string inpStr)
        {
            IscoFlowUnits retVal;
            switch (inpStr.ToLower())
            {
                case "ml/min":
                    retVal = IscoFlowUnits.ml_min;
                    break;
                case "ml/hr":
                    retVal = IscoFlowUnits.ml_hour;
                    break;
                case "ul/min":
                    retVal = IscoFlowUnits.ul_min;
                    break;
                case "ul/hr":
                    retVal = IscoFlowUnits.ul_hr;
                    break;
                default:
                    retVal = IscoFlowUnits.error;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the pressure string returned from the pump to an enum
        /// </summary>
        /// <param name="inpStr">Input string from pump</param>
        /// <returns>Enum specified by input string</returns>
        public static IscoPressureUnits ConvertPressStrToEnum(string inpStr)
        {
            IscoPressureUnits retVal;
            switch (inpStr.ToLower())
            {
                case "atm":
                    retVal = IscoPressureUnits.atm;
                    break;
                case "bar":
                    retVal = IscoPressureUnits.bar;
                    break;
                case "kpa":
                    retVal = IscoPressureUnits.kPa;
                    break;
                case "psi":
                    retVal = IscoPressureUnits.psi;
                    break;
                default:
                    retVal = IscoPressureUnits.error;
                    break;
            }

            return retVal;
        }
    }
}
