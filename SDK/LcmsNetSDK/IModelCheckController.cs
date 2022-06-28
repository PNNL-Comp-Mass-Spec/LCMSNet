using System;
using System.Collections.Generic;

namespace LcmsNetSDK
{
    public class ModelCheckControllerEventArgs : EventArgs
    {
        public ModelCheckControllerEventArgs()
        {
            ModelChecker = null;
        }

        public ModelCheckControllerEventArgs(IFluidicsModelChecker c)
        {
            ModelChecker = c;
        }

        public IFluidicsModelChecker ModelChecker { get; set; }
    }

    public interface IModelCheckController
    {
        List<IFluidicsModelChecker> GetModelCheckers();
        event EventHandler<ModelCheckControllerEventArgs> ModelCheckAdded;
        event EventHandler<ModelCheckControllerEventArgs> ModelCheckRemoved;
        event EventHandler<ModelStatusChangeEventArgs> ModelStatusChangeEvent;
    }
}
