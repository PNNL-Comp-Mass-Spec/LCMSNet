namespace ZaberStageControl
{
    public class XYZAxis3Stage : StageBase
    {
        public XYZAxis3Stage() : base(new string[] { "X", "Y", "Z" }, "XYZ_Stage")
        {
        }

        public StageControl XAxis => StagesUsed[0];
        public StageControl YAxis => StagesUsed[1];
        public StageControl ZAxis => StagesUsed[2];

        public string XAxisConfig
        {
            get => XAxis.GetAxisConfigString();
            set => XAxis.ParseAxisConfigString(value);
        }

        public string YAxisConfig
        {
            get => YAxis.GetAxisConfigString();
            set => YAxis.ParseAxisConfigString(value);
        }

        public string ZAxisConfig
        {
            get => ZAxis.GetAxisConfigString();
            set => ZAxis.ParseAxisConfigString(value);
        }
    }
}
