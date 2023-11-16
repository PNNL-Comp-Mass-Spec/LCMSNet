namespace LcmsNetPlugins.ZaberStage
{
    /// <summary>
    /// Jog speeds; values are used for array indexing
    /// </summary>
    public enum JogSpeed
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    /// <summary>
    /// Move direction; values are '-1' and '1' so they can be as multipliers
    /// </summary>
    public enum MoveDirection
    {
        Decrease = -1,
        Increase = 1
    }

    public static class EnumExtensions
    {
        public static double Convert(this MoveDirection direction, JogSpeed speed, double[] jogSpeeds, bool inverted = false)
        {
            var s = (int)speed;
            var d = (int)direction * (inverted ? -1 : 1);
            if (s < jogSpeeds.Length)
            {
                return d * jogSpeeds[s];
            }

            return d * jogSpeeds[jogSpeeds.Length - 1];
        }

        public static double Convert(this MoveDirection direction, double length, bool inverted = false)
        {
            var d = (int)direction * (inverted ? -1 : 1);
            return d * length;
        }
    }
}
