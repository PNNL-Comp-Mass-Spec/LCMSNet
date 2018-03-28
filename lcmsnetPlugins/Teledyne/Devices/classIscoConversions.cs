//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 04/19/2011
//
//*********************************************************************************************************

using System;

namespace LcmsNetPlugins.Teledyne.Devices
{
    /// <summary>
    /// Utility class for converting between units and setting display captions in ISCO pump classes
    /// </summary>
    public static class classIscoConversions
    {
        #region "Class variables"
        static enumIscoFlowUnits m_FlowUnits = enumIscoFlowUnits.ul_min;
        static enumIscoPressureUnits menu_PressUnits = enumIscoPressureUnits.psi;
        #endregion

        #region "Properties"
        public static enumIscoFlowUnits FlowUnits
        {
            get { return m_FlowUnits; }
            set { m_FlowUnits = value; }
        }

        public static enumIscoPressureUnits PressUnits
        {
            get { return menu_PressUnits; }
            set { menu_PressUnits = value; }
        }
        #endregion

        #region "Methods"
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
        public static double ConvertFlowFromString(string inpFlow, enumIscoFlowUnits units)
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
                case enumIscoFlowUnits.ml_hour:
                    retVal = tmpVal * 1000D * 60D;  // 1000 mL/L; 60 min/hr
                    break;
                case enumIscoFlowUnits.ml_min:
                    retVal = tmpVal * 1000D;    // 1000 mL/L
                    break;
                case enumIscoFlowUnits.ul_hr:
                    retVal = tmpVal * Math.Pow(10, 6) * 60D;    // 10E6 uL/L; 60 min/hr
                    break;
                case enumIscoFlowUnits.ul_min:
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
        public static double ConvertPressFromString(string inpPress, enumISCOModel model)
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
        public static double ConvertPressFromString(string inpPress, enumIscoPressureUnits units, enumISCOModel model)
        {
            if (model == enumISCOModel.Unknown)
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
                if (model == enumISCOModel.ISCO65D)
                {
                    tmpVal = tmpVal / 2.5D;
                }
                else if (model == enumISCOModel.ISCO100D)
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
                case enumIscoPressureUnits.atm:
                    retVal = tmpVal / 14.695;   // 1 atm/14.695 psi
                    break;
                case enumIscoPressureUnits.bar:
                    retVal = tmpVal / 14.504;   // 1 bar/14.504 psi
                    break;
                case enumIscoPressureUnits.kPa:
                    retVal = tmpVal * 6.895;    // 1 psi = 6.895 kPa
                    break;
                case enumIscoPressureUnits.psi:
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
        public static string GetFlowUnitsString(enumIscoFlowUnits units)
        {
            var retStr = "";
            switch (units)
            {
                case enumIscoFlowUnits.ml_hour:
                    retStr = "mL/hr";
                    break;
                case enumIscoFlowUnits.ml_min:
                    retStr = "mL/min";
                    break;
                case enumIscoFlowUnits.ul_hr:
                    retStr = "uL/hr";
                    break;
                case enumIscoFlowUnits.ul_min:
                    retStr = "uL/min";
                    break;
                case enumIscoFlowUnits.error:
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
        public static string GetPressUnitsString(enumIscoPressureUnits units)
        {
            var retStr = "";
            switch (units)
            {
                case enumIscoPressureUnits.atm:
                    retStr = "atm";
                    break;
                case enumIscoPressureUnits.bar:
                    retStr = "bar";
                    break;
                case enumIscoPressureUnits.kPa:
                    retStr = "kPa";
                    break;
                case enumIscoPressureUnits.psi:
                    retStr = "psi";
                    break;
                case enumIscoPressureUnits.error:
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
        public static enumIscoFlowUnits ConvertFlowStrToEnum(string inpStr)
        {
            enumIscoFlowUnits retVal;
            switch (inpStr.ToLower())
            {
                case "ml/min":
                    retVal = enumIscoFlowUnits.ml_min;
                    break;
                case "ml/hr":
                    retVal = enumIscoFlowUnits.ml_hour;
                    break;
                case "ul/min":
                    retVal = enumIscoFlowUnits.ul_min;
                    break;
                case "ul/hr":
                    retVal = enumIscoFlowUnits.ul_hr;
                    break;
                default:
                    retVal = enumIscoFlowUnits.error;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the pressure string returned from the pump to an enum
        /// </summary>
        /// <param name="inpStr">Input string from pump</param>
        /// <returns>Enum specified by input string</returns>
        public static enumIscoPressureUnits ConvertPressStrToEnum(string inpStr)
        {
            enumIscoPressureUnits retVal;
            switch (inpStr.ToLower())
            {
                case "atm":
                    retVal = enumIscoPressureUnits.atm;
                    break;
                case "bar":
                    retVal = enumIscoPressureUnits.bar;
                    break;
                case "kpa":
                    retVal = enumIscoPressureUnits.kPa;
                    break;
                case "psi":
                    retVal = enumIscoPressureUnits.psi;
                    break;
                default:
                    retVal = enumIscoPressureUnits.error;
                    break;
            }

            return retVal;
        }
        #endregion
    }
}
