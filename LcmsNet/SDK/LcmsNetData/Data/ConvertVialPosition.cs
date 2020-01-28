using System;
using System.Collections.Generic;

namespace LcmsNetData.Data
{
    /// <summary>
    /// Utilities for converting Vial (aka Well) position between LCMSNet and LCMS values
    /// </summary>
    public class ConvertVialPosition
    {
        #region "Methods"

        /// <summary>
        /// Converts a vial position in LCMS format (integer 1 - 96) to LCMSNet format (string A01 - H12)
        /// Supports vials beyond 97, where vial 312 is vial Z12, then vial 313 is AA01, 314 is AA02, etc.
        /// Highest vial number supported is 8424, which is ZZ12
        /// </summary>
        /// <param name="vialPosition">Vial position as integer</param>
        /// <param name="columnsPerRow">Columns per row (default is 12)</param>
        /// <returns>LCMSNet string-formatted vial position, or Z99 if an error</returns>
        /// <remarks>
        /// Vial 1 is position A01
        /// ...
        /// Vial 12 is position A12
        /// Vial 13 is position B01
        /// ...
        /// Vial 24 is position B12
        /// Vial 25 is position C01
        /// ...
        /// Vial 96 is position H12
        /// </remarks>
        public static string ConvertVialToString(int vialPosition, byte columnsPerRow = 12)
        {
            // Verify input position is in a valid range
            if ((vialPosition < 1) | (vialPosition > 8424))
            {
                return "Z99";
            }

            if (columnsPerRow < 1)
                columnsPerRow = 12;

            // To get ASCII code for first char, divide vial position by columnsPerRow and add result to 65 ("A")
            // When columnsPerRow is 12, This works for vials 1 through 312; beyond that we need to switch to AA01, AB01, etc.
            var firstCharCode = 65 + Math.Floor((vialPosition - 1) / (double)columnsPerRow);

            const char NO_PREFIX = '.';
            var prefix = NO_PREFIX;
            while (firstCharCode > 90)
            {
                if (prefix == NO_PREFIX)
                {
                    prefix = 'A';
                }
                else
                {
                    prefix = (char)(prefix + 1);
                }
                firstCharCode -= 26;
            }

            var firstChar = (char)firstCharCode;

            // Get the numeric part of the position; this will be a value between 1 and columnsPerRow
            var numericPart = 1 + (vialPosition - 1) % columnsPerRow;

            // Combine the results into the output string
            if (prefix == NO_PREFIX)
            {
                return firstChar + numericPart.ToString("00");
            }

            return prefix.ToString() + firstChar + numericPart.ToString("00");
        }

        /// <summary>
        /// Converts a vial position in LCMSNet format (string A1 - H12) to LCMS format (integer 1 - 96)
        /// Supports positions beyond H12, where position Z12 is vial 312, then position AA01 is vial 313, AA02 is 314, etc.
        /// Highest vial position supported is ZZ12, which is vial 8424
        /// </summary>
        /// <param name="vialPosition">Vial position as string</param>
        /// <param name="columnsPerRow">Columns per row (default is 12)</param>
        /// <returns>Vial position represented as an integer, or 9999 if the format of vialPosition is not recognized</returns>
        public static int ConvertVialToInt(string vialPosition, byte columnsPerRow = 12)
        {
            // Ensure valid string
            if (vialPosition.Length < 2 || vialPosition.Length > 4)
            {
                return 9999;
            }

            if (columnsPerRow < 1)
                columnsPerRow = 12;

            // Get first character(s)
            var intPos = vialPosition.IndexOfAny("0123456789".ToCharArray());
            if (intPos < 1 || intPos > 2)
            {
                return 9999;
            }

            var prefixAddon = 0;
            char columnLetter;
            var vialPositionUpper = vialPosition.ToUpper();

            if (intPos > 1)
            {
                // Position is of the form AB02 or AC10
                // Convert the first character to ascii, subtract 64 (1 less than "A"), then multiply by 26 * columnsPerRow
                var prefix = vialPositionUpper[0];
                prefixAddon = (26 * columnsPerRow) * (prefix - 64);
                columnLetter = vialPositionUpper[1];
            }
            else
            {
                // Position is of the form A9 or C11
                columnLetter = vialPositionUpper[0];
            }

            var positionInColumnText = vialPosition.Substring(intPos);
            int positionInColumn;
            if (!int.TryParse(positionInColumnText, out positionInColumn))
            {
                // Number portion is not numeric
                return 9999;
            }

            // Convert the column letter to ascii, subtract 65 ("A"), then multiply by columnsPerRow
            var columnAddon = columnsPerRow * (columnLetter - 65);

            var vialNum = prefixAddon + columnAddon + positionInColumn;

            return vialNum;
        }

        /// <summary>
        /// Use this function to view example column positions when columns per row is 12, 24, 36, ... 96
        /// Also validates the round trip conversion from vial number to vial position then back to vial number
        /// </summary>
        public static void VerifyLCMSNetVialPosition()
        {
            // Test vial num 1 through 8424
            const int MAX_VIAL_NUM = 27 * 26 * 12;

            for (byte columnsPerRow = 12; columnsPerRow <= 96; columnsPerRow += 12)
            {
                Console.WriteLine();
                Console.WriteLine("Testing {0} columns per row", columnsPerRow);

                var mapping = new Dictionary<int, string>();

                for (var vialNum = 1; vialNum <= MAX_VIAL_NUM; vialNum++)
                {
                    var position = ConvertVialToString(vialNum, columnsPerRow);

                    mapping.Add(vialNum, position);

                    Console.WriteLine("Vial {0} is position {1}", vialNum, position);

                    if (vialNum > 1248 && vialNum < 8400)
                        vialNum += 21;

                }

                Console.WriteLine();

                foreach (var vialInfo in mapping)
                {
                    var vialNum = ConvertVialToInt(vialInfo.Value, columnsPerRow);

                    if (vialNum == vialInfo.Key)
                        Console.WriteLine("Position {0} converts to vial {1}", vialInfo.Value, vialNum);
                    else
                        Console.WriteLine("Position {0} converts to vial {1}; mismatch!", vialInfo.Value, vialNum);

                }

                Console.WriteLine("Done testing {0} columns per row", columnsPerRow);
                Console.WriteLine();
            }

        }
        #endregion
    }
}