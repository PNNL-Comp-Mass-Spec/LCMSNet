using System;
using LcmsNetDataClasses;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class ModelCheckReportViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the model check report view control that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ModelCheckReportViewModel()
        {
            Time = "TestTime";
            Type = "TestType";
            MessageType = "TestCategory";
            Event = "TestEvent";
            Device = "TestDevice";
            ProblemDevice = "TestProblemDevice";
        }

        public ModelCheckReportViewModel(ModelStatus status)
        {
            Time = status.Time;
            Type = status.Name;
            MessageType = status.Category.ToString();
            Event = status.Event;
            Device = status.EventDevice != null ? status.EventDevice.Name : string.Empty;
            ProblemDevice = status.ProblemDevice != null ? status.ProblemDevice.Name : string.Empty;
        }

        private string time;
        private string messageType;
        private string type;
        private string eventName;
        private string device;
        private string problemDevice;

        public string Time
        {
            get { return time; }
            set { this.RaiseAndSetIfChanged(ref time, value); }
        }

        public string MessageType
        {
            get { return messageType; }
            set { this.RaiseAndSetIfChanged(ref messageType, value); }
        }

        public string Type
        {
            get { return type; }
            set { this.RaiseAndSetIfChanged(ref type, value); }
        }

        public string Event
        {
            get { return eventName; }
            set { this.RaiseAndSetIfChanged(ref eventName, value); }
        }

        public string Device
        {
            get { return device; }
            set { this.RaiseAndSetIfChanged(ref device, value); }
        }

        public string ProblemDevice
        {
            get { return problemDevice; }
            set { this.RaiseAndSetIfChanged(ref problemDevice, value); }
        }
    }
}
