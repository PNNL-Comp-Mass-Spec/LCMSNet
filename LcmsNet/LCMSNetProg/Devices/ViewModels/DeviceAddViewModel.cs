using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class DeviceAddViewModel : ReactiveObject
    {
        public DeviceAddViewModel()
        {
            // Design-time data
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                var devices = new List<DevicePluginInformation>();
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Timer1", "Timers")));
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Timer2", "Timers")));
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Error1", "Errors")));
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Error2", "Errors")));
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Error3", "Errors")));
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Pump1", "Pumps")));
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Pump2", "Pumps")));
                devices.Add(new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Pump3", "Pumps")));
                AddPluginInformation(devices);
                SelectedPlugin = devices[3];
                addedPlugins.Add(devices[2]);
                addedPlugins.Add(devices[7]);
                SelectedPlugins.Add(devices[2]);
            }
            SetupCommands();
        }

        private bool initializeOnAdd = false;
        private readonly ReactiveList<DevicePluginInformation> addedPlugins = new ReactiveList<DevicePluginInformation>();
        private readonly ReactiveList<PluginCategoryData> availablePlugins = new ReactiveList<PluginCategoryData>();
        private readonly ReactiveList<DevicePluginInformation> selectedPlugins = new ReactiveList<DevicePluginInformation>();
        private DevicePluginInformation selectedPlugin;

        /// <summary>
        /// Gets or sets the initialize on add flag.
        /// </summary>
        public bool InitializeOnAdd
        {
            get { return initializeOnAdd; }
            set { this.RaiseAndSetIfChanged(ref initializeOnAdd, value); }
        }

        public IReadOnlyReactiveList<DevicePluginInformation> AddedPlugins => addedPlugins;
        public IReadOnlyReactiveList<PluginCategoryData> AvailablePlugins => availablePlugins;
        public ReactiveList<DevicePluginInformation> SelectedPlugins => selectedPlugins;

        public DevicePluginInformation SelectedPlugin
        {
            get { return selectedPlugin; }
            set { this.RaiseAndSetIfChanged(ref selectedPlugin, value); }
        }

        /// <summary>
        /// Adds the supplied plugins to the check box list.
        /// </summary>
        /// <param name="plugins"></param>
        public void AddPluginInformation(List<DevicePluginInformation> plugins)
        {
            var mapping = new Dictionary<string, PluginCategoryData>();
            foreach (var plugin in plugins)
            {
                if (!mapping.ContainsKey(plugin.DeviceAttribute.Category))
                {
                    mapping.Add(plugin.DeviceAttribute.Category, new PluginCategoryData(plugin.DeviceAttribute.Category));
                }
                mapping[plugin.DeviceAttribute.Category].Plugins.Add(plugin);
            }

            using (availablePlugins.SuppressChangeNotifications())
            {
                availablePlugins.Clear();
                availablePlugins.AddRange(mapping.Values);
            }
        }

        public List<DevicePluginInformation> GetSelectedPlugins()
        {
            return addedPlugins.ToList();
        }

        /// <summary>
        /// Adds the selected node to the list box of devices to be loaded.
        /// </summary>
        private void AddSelectedNode()
        {
            if (SelectedPlugin != null)
            {
                addedPlugins.Add(SelectedPlugin);
            }
        }

        private void RemoveSelectedItems()
        {
            if (SelectedPlugins.Count > 0)
            {
                using (SelectedPlugins.SuppressChangeNotifications())
                using (addedPlugins.SuppressChangeNotifications())
                {
                    addedPlugins.RemoveAll(SelectedPlugins);
                }
            }
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }

        private void SetupCommands()
        {
            AddCommand = ReactiveCommand.Create(() => AddSelectedNode(), this.WhenAnyValue(x => x.SelectedPlugin, x => x.addedPlugins).Select(x => x.Item1 != null && !x.Item2.Any(y => y.DisplayName.Equals(x.Item1.DisplayName))));
            RemoveCommand = ReactiveCommand.Create(() => RemoveSelectedItems(), this.WhenAnyValue(x => x.SelectedPlugins.Count).Select(x => x > 0));
        }
    }

    public class PluginCategoryData
    {
        public string Category { get; private set; }

        public ReactiveList<DevicePluginInformation> Plugins => plugins;

        private readonly ReactiveList<DevicePluginInformation> plugins = new ReactiveList<DevicePluginInformation>();

        public PluginCategoryData(string categoryName)
        {
            Category = categoryName;
        }
    }
}
