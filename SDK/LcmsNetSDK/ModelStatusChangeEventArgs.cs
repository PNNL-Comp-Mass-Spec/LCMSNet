using System;
using System.Collections.Generic;

namespace LcmsNetSDK
{
    /// <summary>
    /// Event args for ModelCheck event
    /// </summary>
    public class ModelStatusChangeEventArgs : EventArgs
    {
        public ModelStatusChangeEventArgs()
        {
        }

        public ModelStatusChangeEventArgs(List<ModelStatus> status)
        {
            StatusList = status;
        }

        public List<ModelStatus> StatusList { get; set; }
    }
}
