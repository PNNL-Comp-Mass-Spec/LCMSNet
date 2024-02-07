namespace ZaberStageControl
{
    public class XYAxis2Stage : StageBase
    {
        public XYAxis2Stage() : base(new string[] { "X", "Y" }, "XY_Stage")
        {
        }

        public StageControl XAxis => StagesUsed[0];
        public StageControl YAxis => StagesUsed[1];

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
    }
}
