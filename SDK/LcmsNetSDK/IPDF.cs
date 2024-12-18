﻿using System.Collections.Generic;
using System.Windows.Media.Imaging;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK
{
    public interface IPDF
    {
        void WritePDF(string documentPath, string title, ISampleInfo sample, string numEnabledColumns,
            IReadOnlyList<IColumn> columnData,
            IReadOnlyList<IDevice> devices, BitmapSource fluidicsImage);
    }
}
