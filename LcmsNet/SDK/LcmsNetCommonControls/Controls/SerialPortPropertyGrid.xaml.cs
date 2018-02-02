using System.IO.Ports;
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace LcmsNetCommonControls.Controls
{
    /// <summary>
    /// Interaction logic for SerialPortPropertyGrid.xaml
    /// </summary>
    public partial class SerialPortPropertyGrid : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SerialPortPropertyGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// List of serial port names
        /// </summary>
        public static readonly ReactiveList<string> SerialPortNamesList;

        static SerialPortPropertyGrid()
        {
            SerialPortNamesList = new ReactiveList<string>(SerialPort.GetPortNames());
        }

        /// <summary>
        /// Updates the serial port name list
        /// </summary>
        public static void UpdateSerialPortNames()
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                using (SerialPortNamesList.SuppressChangeNotifications())
                {
                    SerialPortNamesList.Clear();
                    SerialPortNamesList.AddRange(SerialPort.GetPortNames());
                }
            });
        }

        /// <summary>
        /// List of valid serial port names.
        /// </summary>
        public IReadOnlyReactiveList<string> SerialPortNames => SerialPortNamesList;

        private void PropertyGrid_OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            RaiseEvent(new PropertyValueChangedEventArgs(PropertyValueChangedEvent, sender, e.OldValue, e.NewValue));
        }

        /// <summary>
        /// Routed event for PropertyValueChanged
        /// </summary>
        public static readonly RoutedEvent PropertyValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(PropertyValueChanged), RoutingStrategy.Bubble, typeof(PropertyValueChangedEventHandler), typeof(SerialPortPropertyGrid));

        /// <summary>
        /// Event fired when a property value is changed
        /// </summary>
        public event PropertyValueChangedEventHandler PropertyValueChanged
        {
            add => AddHandler(PropertyValueChangedEvent, value);
            remove => RemoveHandler(PropertyValueChangedEvent, value);
        }
    }
}
