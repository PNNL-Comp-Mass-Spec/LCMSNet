using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using LcmsNetDataClasses.Devices;
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
                var devices = new List<classDevicePluginInformation>();
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Timer1", "Timers")));
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Timer2", "Timers")));
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Error1", "Errors")));
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Error2", "Errors")));
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Error3", "Errors")));
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Pump1", "Pumps")));
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Pump2", "Pumps")));
                devices.Add(new classDevicePluginInformation(typeof(classTimerDevice), new classDeviceControlAttribute(null, "Pump3", "Pumps")));
                AddPluginInformation(devices);
                SelectedPlugin = devices[3];
                AddedPlugins.Add(devices[2]);
                AddedPlugins.Add(devices[7]);
                SelectedPlugins.Add(devices[2]);
            }
            SetupCommands();
        }

        private bool initializeOnAdd = false;
        private readonly ReactiveList<classDevicePluginInformation> addedPlugins = new ReactiveList<classDevicePluginInformation>();
        private readonly ReactiveList<PluginCategoryData> availablePlugins = new ReactiveList<PluginCategoryData>();
        private readonly ReactiveList<classDevicePluginInformation> selectedPlugins = new ReactiveList<classDevicePluginInformation>();
        private classDevicePluginInformation selectedPlugin;

        /// <summary>
        /// Gets or sets the initialize on add flag.
        /// </summary>
        public bool InitializeOnAdd
        {
            get { return initializeOnAdd; }
            set { this.RaiseAndSetIfChanged(ref initializeOnAdd, value); }
        }

        public ReactiveList<classDevicePluginInformation> AddedPlugins => addedPlugins;
        public ReactiveList<PluginCategoryData> AvailablePlugins => availablePlugins;
        public ReactiveList<classDevicePluginInformation> SelectedPlugins => selectedPlugins;

        public classDevicePluginInformation SelectedPlugin
        {
            get { return selectedPlugin; }
            set { this.RaiseAndSetIfChanged(ref selectedPlugin, value); }
        }

        /// <summary>
        /// Adds the supplied plugins to the check box list.
        /// </summary>
        /// <param name="plugins"></param>
        public void AddPluginInformation(List<classDevicePluginInformation> plugins)
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

            using (AvailablePlugins.SuppressChangeNotifications())
            {
                AvailablePlugins.Clear();
                AvailablePlugins.AddRange(mapping.Values);
            }
        }

        public List<classDevicePluginInformation> GetSelectedPlugins()
        {
            return AddedPlugins.ToList();
        }

        /// <summary>
        /// Adds the selected node to the list box of devices to be loaded.
        /// </summary>
        private void AddSelectedNode()
        {
            if (SelectedPlugin != null)
            {
                AddedPlugins.Add(SelectedPlugin);
            }
        }

        private void RemoveSelectedItems()
        {
            if (SelectedPlugins.Count > 0)
            {
                using (SelectedPlugins.SuppressChangeNotifications())
                using (AddedPlugins.SuppressChangeNotifications())
                {
                    AddedPlugins.RemoveAll(SelectedPlugins);
                }
            }
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }

        private void SetupCommands()
        {
            AddCommand = ReactiveCommand.Create(() => AddSelectedNode(), this.WhenAnyValue(x => x.SelectedPlugin, x => x.AddedPlugins).Select(x => x.Item1 != null && !Enumerable.Any<classDevicePluginInformation>(x.Item2, y => y.DisplayName.Equals(x.Item1.DisplayName))));
            RemoveCommand = ReactiveCommand.Create(() => RemoveSelectedItems(), this.WhenAnyValue(x => x.SelectedPlugins.Count).Select(x => x > 0));
        }
    }

    public class PluginCategoryData
    {
        public string Category { get; private set; }

        public ReactiveList<classDevicePluginInformation> Plugins => plugins;

        private readonly ReactiveList<classDevicePluginInformation> plugins = new ReactiveList<classDevicePluginInformation>();

        public PluginCategoryData(string categoryName)
        {
            Category = categoryName;
        }
    }
}
