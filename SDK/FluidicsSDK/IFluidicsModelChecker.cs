﻿using System.Collections.Generic;
using LcmsNetSDK;
using LcmsNetSDK.System;

namespace FluidicsSDK
{
    /// <summary>
    /// interface for checking validity of fluidics models
    /// </summary>
    public interface IFluidicsModelChecker : INotifier, INotifyPropertyChangedExt
    {
        /// <summary>
        /// gets or sets if this specific model check is enabled during the simulation.
        /// </summary>
        bool IsEnabled { get; set; }

        ModelStatusCategory Category { get; set; }

        /// <summary>
        /// CheckModel does the actual checking of the fluidics model.
        /// </summary>
        /// <returns> An IEnumerable of ModelStatus objects representing model status changes
        /// </returns>
        IEnumerable<ModelStatus> CheckModel();
    }
}
