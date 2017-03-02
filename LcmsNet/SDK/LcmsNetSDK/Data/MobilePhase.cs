namespace LcmsNetSDK.Data
{
    public class MobilePhase
    {
        public MobilePhase()
        {
        }

        public MobilePhase(string name, string comment)
        {
            Name = name;
            Comment = comment;
        }

        public string Name { get; set; }
        public string Comment { get; set; }
    }
}