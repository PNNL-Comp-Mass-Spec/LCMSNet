using System;
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
            LCMethodEventParameter parameter)
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
            for (var i = 0; i < Parameters.ViewModels.Count; i++)
            {
                var vm = Parameters.ViewModels[i];

                // Grab the ViewModels value to be used later on
                if (vm != null)
                {
                    Parameters.Values[i] = vm.ParameterValue;
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
            var parameters = new LCMethodEventParameter();
            parameters.Names.AddRange(Parameters.Names);
            parameters.Values.AddRange(Parameters.Values);
            parameters.DataProviderNames.AddRange(Parameters.DataProviderNames);
            foreach (var parameterVm in Parameters.ViewModels)
            {
                var vm = parameterVm?.CreateDuplicate();
                if (vm is ILCEventParameterWithDataProvider dpvm)
                {
                    Device.RegisterDataProvider(MethodEventAttribute.DataProvider, dpvm.FillData);
                }

                parameters.ViewModels.Add(vm);
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
        /// Gets or sets the parameter values.
        /// </summary>
        public LCMethodEventParameter Parameters { get; set; }

        /// <summary>
        /// Gets or sets whether this event is part of the optimization step.
        /// </summary>
        public bool OptimizeWith { get; set; }

        public bool BreakPoint { get; set; }

        public bool Executing { get; set; }

        #endregion
    }
}