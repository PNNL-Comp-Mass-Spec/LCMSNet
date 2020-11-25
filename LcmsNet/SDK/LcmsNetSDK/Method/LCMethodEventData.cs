﻿using System;
using System.Collections.Generic;
using System.Reflection;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.Method
{
    public class BreakEventArgs : EventArgs
    {
        public BreakEventArgs(bool stoppedHere)
        {
            IsStopped = stoppedHere;
        }

        public bool IsStopped { get; set; }
    }

    /// <summary>
    /// Class that holds the selected event and the value to pass for the parameters.
    /// </summary>
    public class LCMethodEventData
    {
        /// <summary>
        /// Constructor that takes a event, and the value to call it with.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="info">The info object about the method used by the event</param>
        /// <param name="attr"></param>
        /// <param name="parameter"></param>
        public LCMethodEventData(IDevice device,
            MethodInfo info,
            LCMethodEventAttribute attr,
            IReadOnlyList<LCMethodEventParameter> parameter)
        {
            Method = info;
            Parameters = parameter;
            MethodEventAttribute = attr;
            Device = device;
        }

        public event EventHandler<BreakEventArgs> BreakPointEvent;
        public event EventHandler Simulated;
        public event EventHandler SimulatingEvent;

        /// <summary>
        /// Builds the event by grabbing the values stored in the ILCEventParameter objects.
        /// </summary>
        public void BuildEvent()
        {
            foreach (var parameter in Parameters)
            {
                var vm = parameter.ViewModel;

                // Grab the ViewModels value to be used later on
                if (vm != null)
                {
                    parameter.Value = vm.ParameterValue;
                }
            }
        }

        public void Break()
        {
            BreakPointEvent?.Invoke(this, new BreakEventArgs(true));
        }

        public void PassBreakPoint()
        {
            BreakPointEvent?.Invoke(this, new BreakEventArgs(false));
        }

        public void IsDone()
        {
            Executing = false;
            Simulated?.Invoke(this, new EventArgs());
        }

        public void IsCurrent()
        {
            Executing = true;
            SimulatingEvent?.Invoke(this, new EventArgs());
        }

        public LCMethodEventData Clone()
        {
            var parameters = new List<LCMethodEventParameter>();
            foreach (var parameter in Parameters)
            {
                var vm = parameter.ViewModel?.CreateDuplicate();
                if (vm is ILCEventParameterWithDataProvider dpvm)
                {
                    Device.RegisterDataProvider(MethodEventAttribute.DataProvider, dpvm.FillData);
                }

                parameters.Add(new LCMethodEventParameter(parameter.Name, parameter.Value, vm, parameter.DataProviderName));
            }

            var copy = new LCMethodEventData(Device, Method, MethodEventAttribute, parameters);

            return copy;
        }

        /// <summary>
        /// Returns the name of the method/event in human readable form.
        /// </summary>
        /// <returns>Name of the method/event</returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(MethodEventAttribute.Name) ? "Undefined method/event" : MethodEventAttribute.Name;
        }

        #region Properties

        public IDevice Device { get; set; }

        /// <summary>
        /// Method that holds the given parameters.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Gets or sets the method attribute information for this method.
        /// </summary>
        public LCMethodEventAttribute MethodEventAttribute { get; set; }

        /// <summary>
        /// Gets the parameter values.
        /// </summary>
        public IReadOnlyList<LCMethodEventParameter> Parameters { get; }

        /// <summary>
        /// Gets or sets whether this event is part of the optimization step.
        /// </summary>
        public bool OptimizeWith { get; set; }

        public bool BreakPoint { get; set; }

        public bool Executing { get; set; }

        #endregion
    }
}