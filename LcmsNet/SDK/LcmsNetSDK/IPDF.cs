using System.Collections.Generic;
using System.Windows.Media.Imaging;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK
{
    public interface IPDF
    {
        void WritePDF(string documentPath, string title, classSampleData sample, string numEnabledColumns,
            List<classColumnData> columnData,
            List<IDevice> devices, BitmapSource fluidicsImage);
    }
}