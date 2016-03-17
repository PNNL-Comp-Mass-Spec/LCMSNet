using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetSDK;

namespace LcmsNetDataClasses
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