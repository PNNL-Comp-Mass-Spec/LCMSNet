namespace ZaberStageControl
{
    public class XAxis1Stage : StageBase
    {
        public XAxis1Stage() : base(new string[] { "X" }, "X_Stage")
        {
        }

        public StageControl XAxis => StagesUsed[0];

        public string XAxisConfig
        {
            get => XAxis.GetAxisConfigString();
            set => XAxis.ParseAxisConfigString(value);
        }
    }
}
