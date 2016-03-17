//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 12/01/2009
//
// Last modified 12/01/2009
//*********************************************************************************************************

using System.Text.RegularExpressions;

namespace LcmsNet.SampleQueue.IO
{
    class classConvertVialPosition
    {
        //*********************************************************************************************************
        // Utilities for converting Vial (aka Well) position between LCMSNet and LCMS values
        //**********************************************************************************************************

        #region "Methods"

        /// <summary>
        /// Converts a vial position in LCMS format (integer 1 - 96) to LCMSNet format (string A1 - H12)
        /// </summary>
        /// <param name="vialPosition">Vial position as integer</param>
        /// <returns>LCMSNet string-formated vial position</returns>
        public static string ConvertVialToString(int vialPosition)
        {
            // Verify input position is in a valid range
            if ((vialPosition < 1) | (vialPosition > 96))
            {
                return "Z99";
            }

            // To get ASCII code for first char, divide vial position by 12 and add result to 65 ("A")
            char firstChar = (char) (65 + (vialPosition / 12));

            // Get the numeric part of the position via a MOD operation
            int numericPart = vialPosition % 12;
            // Positions go from 1 to 12, so if the result of the mod operation is 0, add 12
            if (numericPart == 0)
            {
                numericPart = numericPart + 12;
                firstChar--;
            }

            // Combine the results into the output string
            return firstChar.ToString() + numericPart.ToString("00");
        }

        /// <summary>
        /// Converts a vial position in LCMSNet format (string A1 - H12) to LCMS format (integer 1 - 96)
        /// </summary>
        /// <param name="vialPosition">Vial position as string</param>
        /// <returns>Vial position represented as an integer</returns>
        public static int ConvertVialToInt(string vialPosition)
        {
            MatchCollection mc = null;
            char[] tmpCharArray;

            // Ensure valid string
            if (vialPosition.Length < 2)
            {
                return 99;
            }
            if (vialPosition.Length > 3)
            {
                return 99;
            }

            string tmpPosition = vialPosition.ToUpper();

            // Get first character
            mc = Regex.Matches(tmpPosition, "^[A-E]");
            if (mc.Count != 1)
            {
                return 99;
            }
            string firstChar = mc[0].ToString();
            tmpCharArray = firstChar.ToCharArray();

            // Get numeric part
            mc = Regex.Matches(tmpPosition, "[0-9]+");
            if (mc.Count != 1)
            {
                return 99;
            }

            int numericPart = int.Parse(mc[0].ToString());

            // Verify valid numeric part
            if ((numericPart < 1) | (numericPart > 12))
            {
                return 99;
            }

            // Assemble the integer. Start by subtracting 65 from the ASCII value for the first character
            int tmpOutput = (int) tmpCharArray[0] - 65;
            // Multiply the result by 12
            tmpOutput = tmpOutput * 12;
            // Add the numeric part and return
            return tmpOutput + numericPart;
        }

        #endregion
    }
} // End namespace