﻿using System;
using System.Collections.Generic;
using System.Reflection;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNet.Method
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
    public class LCMethodEventData : ILCMethodEvent
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

        /// <inheritdoc />
        public void Break()
        {
            BreakPointEvent?.Invoke(this, new BreakEventArgs(true));
        }

        /// <inheritdoc />
        public void PassBreakPoint()
        {
            BreakPointEvent?.Invoke(this, new BreakEventArgs(false));
        }

        /// <inheritdoc />
        public void IsDone()
        {
            Executing = false;
            Simulated?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void IsCurrent()
        {
            Executing = true;
            SimulatingEvent?.Invoke(this, EventArgs.Empty);
        }

        public LCMethodEventData Clone()
        {
            var parameters = new List<LCMethodEventParameter>();
            foreach (var parameter in Parameters)
            {
                var vm = parameter.ViewModel?.CreateDuplicate();
                if (vm is ILCEventParameterWithDataProvider dpvm && MethodEventAttribute.DataProvider.IsSet)
                {
                    // This is a must for data provider updates if using a data provider, but ILCEventParameterWithDataProvider is also used for enum types and populated with enum values.
                    if (Device is IHasDataProvider dataProvider)
                    {
                        dataProvider.RegisterDataProvider(MethodEventAttribute.DataProvider.Key, dpvm.FillData);
                    }
                    else
                    {
                        ApplicationLogger.LogError(LogLevel.Error, $"LC Event {MethodEventAttribute.Name} has a data provider, but device {Device.GetType()} does not implement {nameof(IHasDataProvider)}");
                        continue;
                    }
                }

                parameters.Add(new LCMethodEventParameter(parameter.Name, parameter.Value, vm, parameter.DataProviderName));
            }

            var copy = new LCMethodEventData(Device, Method, MethodEventAttribute, parameters) { OptimizeWith = OptimizeWith, Comment = Comment };

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

        public IDevice Device { get; set; }

        /// <summary>
        /// Method that holds the given parameters.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Comment for the event
        /// </summary>
        public string Comment { get; set; }

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

        /// <inheritdoc />
        public bool BreakPoint { get; set; }

        /// <inheritdoc />
        public bool Executing { get; set; }
    }
}
