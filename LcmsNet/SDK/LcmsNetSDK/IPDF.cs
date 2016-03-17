using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using System.Drawing;

namespace LcmsNetSDK
{
    public interface IPDF
    {
        void WritePDF(string documentPath, string title, classSampleData sample, string numEnabledColumns,
            List<LcmsNetDataClasses.Configuration.classColumnData> columnData,
            List<IDevice> devices, Bitmap fluidicsImage);
    }
}