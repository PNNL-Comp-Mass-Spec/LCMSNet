using System;
using System.Windows;

namespace XcaliburControl
{
    internal class XcaliburConfigDevViewModel : XcaliburConfigViewModelBase
    {
        [Obsolete("For WPF design-time use only", true)]
        public XcaliburConfigDevViewModel() : base(new XcaliburController())
        { }

        public override bool AllowDirectoryPathUpdate { get; } = true;
        public override bool ShowMethodExport { get; } = true;
        protected override string BrowseForDirectory(string prompt, string startingDirectory, Window window)
        {
            return "";
        }

        public override void LoadSavedSettings()
        {
        }

        public override void SaveSettings()
        {
        }
    }
}
