using System;
using System.Windows;
using XcaliburControl;

namespace LcmsNetPlugins.XcaliburLC
{
    public class XcaliburConfigViewModel : XcaliburConfigViewModelBase
    {
        [Obsolete("For WPF design-time use only", true)]
        public XcaliburConfigViewModel() : this(new XcaliburLCPump())
        { }

        public XcaliburConfigViewModel(XcaliburLCPump controller) : base(controller)
        { }

        public override bool AllowDirectoryPathUpdate => false;
        public override bool ShowMethodExport => true;

        protected override string BrowseForDirectory(string prompt, string startingDirectory, Window window)
        {
            return null;
        }

        public override void LoadSavedSettings()
        { }

        public override void SaveSettings()
        { }
    }
}
