using System;

namespace XcaliburControl
{
    public readonly struct DeviceInfo
    {
        public string UI { get; }
        public string CFGUI { get; }
        public string StatusOCX { get; }
        public string VirDev { get; }
        public string Description { get; }
        public string ShortName { get; }
        public string RequiredDevice { get; }
        public string HelpFileName { get; }
        public string HelpFileLabel { get; }
        public string TuneHelpFileName { get; }
        public string TuneHelpFileLabel { get; }
        public string DirectControlOCX { get; }
        public string Version { get; }
        public string XcalVersion { get; }
        public int Type { get; }
        public string TypeString { get; }

        public DeviceInfo(string ui, string cfgUI, string statusOCX, string virDev, string description,
            string shortName, string requiredDevice, string helpFileName, string helpFileLabel, string tuneHelpFileName,
            string tuneHelpFileLabel, string directControlOCX, string version, string xcalVersion, int type,
            string typeString)
        {
            UI = ui;
            CFGUI = cfgUI;
            StatusOCX = statusOCX;
            VirDev = virDev;
            Description = description;
            ShortName = shortName;
            RequiredDevice = requiredDevice;
            HelpFileName = helpFileName;
            HelpFileLabel = helpFileLabel;
            TuneHelpFileName = tuneHelpFileName;
            TuneHelpFileLabel = tuneHelpFileLabel;
            DirectControlOCX = directControlOCX;
            Version = version;
            XcalVersion = xcalVersion;
            Type = type;
            TypeString = typeString;
        }

        public DeviceInfo(object[,] data, int i)
        {
            var j = 0;
            UI = data[i, j++].ToString();
            CFGUI = data[i, j++].ToString();
            StatusOCX = data[i, j++].ToString();
            VirDev = data[i, j++].ToString();
            Description = data[i, j++].ToString();
            ShortName = data[i, j++].ToString();
            RequiredDevice = data[i, j++].ToString();
            HelpFileName = data[i, j++].ToString();
            HelpFileLabel = data[i, j++].ToString();
            TuneHelpFileName = data[i, j++].ToString();
            TuneHelpFileLabel = data[i, j++].ToString();
            DirectControlOCX = data[i, j++].ToString();
            Version = data[i, j++].ToString();
            XcalVersion = data[i, j++].ToString();
            var type = data[i, j];
            try
            {
                Type = Convert.ToInt32(type);
                TypeString = XcaliburCOM.ConvertDeviceTypeToString(Type);
            }
            catch
            {
                Type = -1;
                TypeString = $"Not parseable type: {type}";
            }
        }

        public override string ToString()
        {
            return $"UI '{UI}', CFGUI '{CFGUI}', StatusOCX '{StatusOCX}', VirDev '{VirDev}', Description '{Description}', " +
                   $"ShortName '{ShortName}', RequiredDevice '{RequiredDevice}', HelpFileName '{HelpFileName}', " +
                   $"HelpFileLabel '{HelpFileLabel}', TuneHelpFileName '{TuneHelpFileName}', TuneHelpFileLabel '{TuneHelpFileLabel}', " +
                   $"DirectControlOCX '{DirectControlOCX}', Version '{Version}', XcalVersion '{XcalVersion}', Type '{Type}' ({TypeString})";
        }
    }
}
