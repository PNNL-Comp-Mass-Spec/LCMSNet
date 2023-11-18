using System;

namespace LcmsNetPlugins.ZaberStage
{
    /// <summary>
    /// Stores position values (in millimeters)
    /// </summary>
    public readonly struct Position
    {
        public string ID { get; }
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double XRetract { get; }
        public double YRetract { get; }
        public double ZRetract { get; }

        private readonly bool hasRetractValue;

        public Position(string id, double x = 0, double y = 0, double z = 0, double xRetract = 0, double yRetract = 0, double zRetract = 0)
        {
            ID = id;
            X = x;
            Y = y;
            Z = z;
            XRetract = xRetract;
            YRetract = yRetract;
            ZRetract = zRetract;
            hasRetractValue = XRetract != 0 || YRetract != 0 || ZRetract != 0;
        }

        public Position(string positionEncoded)
        {
            if (string.IsNullOrWhiteSpace(positionEncoded))
            {
                ID = "bad";
                X = 0;
                Y = 0;
                Z = 0;
                XRetract = 0;
                YRetract = 0;
                ZRetract = 0;
                hasRetractValue = false;
                return;
            }

            var split = positionEncoded.Split(',');
            if (split.Length != 7)
            {
                throw new NotSupportedException();
            }

            ID = split[0];
            X = double.Parse(split[1]);
            Y = double.Parse(split[2]);
            Z = double.Parse(split[3]);
            XRetract = double.Parse(split[4]);
            YRetract = double.Parse(split[5]);
            ZRetract = double.Parse(split[6]);
            hasRetractValue = XRetract != 0 || YRetract != 0 || ZRetract != 0;
        }

        public string GetPositionEncoded()
        {
            return $"{ID},{X},{Y},{Z},{XRetract},{YRetract},{ZRetract}";
        }

        public override string ToString()
        {
            if (hasRetractValue)
            {
                return $"X: {X}, Y: {Y}, Z: {Z}, Xd: {XRetract}, Yd: {YRetract}, Zd: {ZRetract}";
            }

            return $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }
}
