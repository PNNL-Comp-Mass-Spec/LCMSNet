using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using ReactiveUI;

namespace FailureInjector.Drivers
{
    public class NotificationDriverViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private NotificationDriver m_driver;

        public NotificationDriverViewModel()
        {
            InjectFailureCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => InjectFailure()));
        }

        public ReactiveCommand<Unit, Unit> InjectFailureCommand { get; private set; }

        private void InjectFailure()
        {
            m_driver.InjectFailure();
        }

        public void RegisterDevice(IDevice device)
        {
            m_driver = device as NotificationDriver;
            SetBaseDevice(m_driver);
        }
        #region IDeviceControl Members

        public IDevice Device
        {
            get
            {
                return m_driver;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        public UserControl GetDefaultView()
        {
            return new NotificationDriverView();
        }

        public void ShowProps()
        {

        }
        #endregion
    }
}
