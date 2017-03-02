using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Devices;

namespace FluidicsPack
{
      [classDeviceControlAttribute(null,
                                   typeof(FluidicsTee),
                                   "Tee",
                                   "Fluidics Components")]
    public class Tee:FluidicsComponentBase
    {
        #region Methods
        public Tee()
        {
            Name = "Tee";
        }
        #endregion
    }
}
