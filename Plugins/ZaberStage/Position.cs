using System;

namespace LcmsNetPlugins.ZaberStage
{
    public readonly struct Position
    {
        public string ID { get; }
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double XRetract { get; }
        public double YRetract { get; }
        public double ZRetract { get; }

        public Position(string id, double x = 0, double y = 0, double z = 0, double xRetract = 0, double yRetract = 0, double zRetract = 0)
        {
            ID = id;
            X = x;
            Y = y;
            Z = z;
            XRetract = xRetract;
            YRetract = yRetract;
            ZRetract = zRetract;
        }

        public Position(string positionEncoded)
        {
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
        }

        public string GetPositionEncoded()
        {
            return $"{ID},{X},{Y},{Z},{XRetract},{YRetract},{ZRetract}";
        }
    }
}
