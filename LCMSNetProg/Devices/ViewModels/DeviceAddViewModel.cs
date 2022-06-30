using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class DeviceAddViewModel : ReactiveObject
    {
        public DeviceAddViewModel()
        {
            addedPlugins.ToObservableChangeSet().Bind(out var addedBound).Subscribe();
            availablePlugins.ToObservableChangeSet().Bind(out var availableBound).Subscribe();
            AddedPlugins = addedBound;
            AvailablePlugins = availableBound;

            // Design-time data
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                var devices = new List<DevicePluginInformation>
                {
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Timer1", "Timers")),
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Timer2", "Timers")),
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Error1", "Errors")),
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Error2", "Errors")),
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Error3", "Errors")),
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Pump1", "Pumps")),
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Pump2", "Pumps")),
                    new DevicePluginInformation(typeof(TimerDevice), new DeviceControlAttribute(null, "Pump3", "Pumps"))
                };
                AddPluginInformation(devices);
                SelectedPlugin = devices[3];
                addedPlugins.Add(devices[2]);
                addedPlugins.Add(devices[7]);
                SelectedPlugins.Add(devices[2]);
            }

            AddCommand = ReactiveCommand.Create(AddSelectedNode, this.WhenAnyValue(x => x.SelectedPlugin, x => x.addedPlugins).Select(x => x.Item1 != null && !x.Item2.Any(y => y.DisplayName.Equals(x.Item1.DisplayName))));
            RemoveCommand = ReactiveCommand.Create(RemoveSelectedItems, this.WhenAnyValue(x => x.SelectedPlugins.Count).Select(x => x > 0));
        }

        private bool initializeOnAdd = false;
        private readonly ObservableCollectionExtended<DevicePluginInformation> addedPlugins = new ObservableCollectionExtended<DevicePluginInformation>();
        private readonly ObservableCollectionExtended<PluginCategoryData> availablePlugins = new ObservableCollectionExtended<PluginCategoryData>();
        private DevicePluginInformation selectedPlugin;

        /// <summary>
        /// Gets or sets the initialize on add flag.
        /// </summary>
        public bool InitializeOnAdd
        {
            get => initializeOnAdd;
            set => this.RaiseAndSetIfChanged(ref initializeOnAdd, value);
        }

        public ReadOnlyObservableCollection<DevicePluginInformation> AddedPlugins { get; }
        public ReadOnlyObservableCollection<PluginCategoryData> AvailablePlugins { get; }
        public ObservableCollectionExtended<DevicePluginInformation> SelectedPlugins { get; } = new ObservableCollectionExtended<DevicePluginInformation>();

        public DevicePluginInformation SelectedPlugin
        {
            get => selectedPlugin;
            set => this.RaiseAndSetIfChanged(ref selectedPlugin, value);
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

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

            using (availablePlugins.SuspendNotifications())
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
                using (SelectedPlugins.SuspendNotifications())
                using (addedPlugins.SuspendNotifications())
                {
                    foreach (var item in SelectedPlugins)
                    {
                        addedPlugins.Remove(item);
                    }
                }
            }
        }
    }

    public class PluginCategoryData
    {
        public string Category { get; }

        public ObservableCollectionExtended<DevicePluginInformation> Plugins { get; } = new ObservableCollectionExtended<DevicePluginInformation>();

        public PluginCategoryData(string categoryName)
        {
            Category = categoryName;
        }
    }
}
