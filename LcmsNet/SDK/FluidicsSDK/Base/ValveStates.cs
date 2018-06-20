using System;

namespace FluidicsSDK.Base
{
    /// <summary>
    /// state enum for sixteen port multiposition valves
    /// </summary>
    public enum FifteenPositionState
    {
        P1=1, P2=2, P3=3, P4=4, P5=5, P6=6, P7=7, P8=8, P9=9, P10=10, P11=11, P12=12, P13=13, P14=14, P15=15, Unknown=-1
    };


    public enum EightPositionState
    {
        P1 = 1, P2 = 2, P3 = 3, P4 = 4, P5 = 5, P6 = 6, P7 = 7, P8 = 8, Unknown = -1
    };

    public enum TenPositionState
    {
        P1 = 1, P2 = 2, P3 = 3, P4 = 4, P5 = 5, P6 = 6, P7 = 7, P8 = 8, P9 = 9, P10 = 10, Unknown = -1
    };

    /// <summary>
    /// state enum for two position valves
    /// </summary>
    public enum TwoPositionState
    {
        PositionA = 0, PositionB = 1, Unknown = -1
    };

    /// <summary>
    /// extensions that turn enums into custom strings for display or other use.
    /// </summary>
    public static class StateExtensions
    {
        /// <summary>
        /// convert multiposition state to a human-readable string for display.
        /// </summary>
        /// <param name="s">the state enum to convert to string</param>
        /// <returns>a string representation of the state</returns>
        public static string ToCustomString(this FifteenPositionState s)
        {
            string customString;
            switch (s)
            {
                case FifteenPositionState.P1:
                    customString = "1";
                    break;
                case FifteenPositionState.P2:
                    customString = "2";
                    break;
                case FifteenPositionState.P3:
                    customString = "3";
                    break;
                case FifteenPositionState.P4:
                    customString = "4";
                    break;
                case FifteenPositionState.P5:
                    customString = "5";
                    break;
                case FifteenPositionState.P6:
                    customString = "6";
                    break;
                case FifteenPositionState.P7:
                    customString = "7";
                    break;
                case FifteenPositionState.P8:
                    customString = "8";
                    break;
                case FifteenPositionState.P9:
                    customString = "9";
                    break;
                case FifteenPositionState.P10:
                    customString = "10";
                    break;
                case FifteenPositionState.P11:
                    customString = "11";
                    break;
                case FifteenPositionState.P12:
                    customString = "12";
                    break;
                case FifteenPositionState.P13:
                    customString = "13";
                    break;
                case FifteenPositionState.P14:
                    customString = "14";
                    break;
                case FifteenPositionState.P15:
                    customString = "15";
                    break;
                case FifteenPositionState.Unknown:
                    customString = "Unknown";
                    break;
                default:
                    // this should be 100% impossible to get to...
                    throw new ArgumentException("Error converting state to string");
            }
            return customString;
        }

        public static string ToCustomString(this EightPositionState s)
        {
            string customString;
            switch (s)
            {
                case EightPositionState.P1:
                    customString = "1";
                    break;
                case EightPositionState.P2:
                    customString = "2";
                    break;
                case EightPositionState.P3:
                    customString = "3";
                    break;
                case EightPositionState.P4:
                    customString = "4";
                    break;
                case EightPositionState.P5:
                    customString = "5";
                    break;
                case EightPositionState.P6:
                    customString = "6";
                    break;
                case EightPositionState.P7:
                    customString = "7";
                    break;
                case EightPositionState.P8:
                    customString = "8";
                    break;
                case EightPositionState.Unknown:
                    customString = "Unknown";
                    break;
                default:
                    // this should be 100% impossible to get to...
                    throw new ArgumentException("Error converting state to string");
            }
            return customString;
        }

        /// <summary>
        /// convert multiposition state to a human-readable string for display.
        /// </summary>
        /// <param name="s">the state enum to convert to string</param>
        /// <returns>a string representation of the state</returns>
        public static string ToCustomString(this TenPositionState s)
        {
            string customString;
            switch (s)
            {
                case TenPositionState.P1:
                    customString = "1";
                    break;
                case TenPositionState.P2:
                    customString = "2";
                    break;
                case TenPositionState.P3:
                    customString = "3";
                    break;
                case TenPositionState.P4:
                    customString = "4";
                    break;
                case TenPositionState.P5:
                    customString = "5";
                    break;
                case TenPositionState.P6:
                    customString = "6";
                    break;
                case TenPositionState.P7:
                    customString = "7";
                    break;
                case TenPositionState.P8:
                    customString = "8";
                    break;
                case TenPositionState.P9:
                    customString = "9";
                    break;
                case TenPositionState.P10:
                    customString = "10";
                    break;
                case TenPositionState.Unknown:
                    customString = "Unknown";
                    break;
                default:
                    // this should be 100% impossible to get to...
                    throw new ArgumentException("Error converting state to string");
            }
            return customString;
        }

        /// <summary>
        /// convert twoposition state to human-readable string for display
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToCustomString(this TwoPositionState s)
        {
            string customString;
            switch (s)
            {
                case TwoPositionState.PositionA:
                    customString = "A";
                    break;
                case TwoPositionState.PositionB:
                    customString = "B";
                    break;
                case TwoPositionState.Unknown:
                    customString = "Unknown";
                    break;
                default:
                    // this should be 100% impossible to get to...
                    throw new ArgumentException("Error converting state to string");
            }
            return customString;
        }
    }
}
