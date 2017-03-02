using System.Collections.Generic;
using System.Drawing;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;

namespace LcmsNetSDK
{
    public interface IPDF
    {
        void WritePDF(string documentPath, string title, classSampleData sample, string numEnabledColumns,
            List<classColumnData> columnData,
            List<IDevice> devices, Bitmap fluidicsImage);
    }
}