using System.Collections.Generic;
using System.Windows.Media.Imaging;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK
{
    public interface IPDF
    {
        void WritePDF(string documentPath, string title, SampleData sample, string numEnabledColumns,
            List<ColumnData> columnData,
            List<IDevice> devices, BitmapSource fluidicsImage);
    }
}