using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using LcmsNet.Configuration;
using LcmsNet.Data;
using LcmsNet.IO;
using LcmsNetSDK;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    /// <summary>
    /// Control that represents a specific stage in the LC-Experiment.
    /// </summary>
    public class LCMethodStageViewModel : ReactiveObject
    {
        /// <summary>
        /// Constant defining where the LC-Methods are stored.
        /// </summary>
        private const string CONST_METHOD_FOLDER_PATH = "LCMethods";

        /// <summary>
        /// Maps a method name to itself so that we can update the ui from the method manager.
        /// </summary>
        private readonly Dictionary<string, string> methodMap;

        private bool triggeredUpdate;

        /// <summary>
        /// Maps the check box clicked to a specific column data.
        /// </summary>
        private readonly Dictionary<string, ColumnData> checkBoxToColumnDataMap;

        /// <summary>
        /// Constructor for holding the list of events.
        /// </summary>
        public LCMethodStageViewModel()
        {
            methodMap = new Dictionary<string, string>();
            MethodFolderPath = CONST_METHOD_FOLDER_PATH;

            // Build the button to method dictionary.
            checkBoxToColumnDataMap = new Dictionary<string, ColumnData>();
            UpdateConfiguration();
            LCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
            LCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;
            LCMethodManager.Manager.MethodUpdated += Manager_MethodUpdated;

            savedMethodsComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var savedMethodsComboBoxOptionsBound).Subscribe();
            columnComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var columnComboBoxOptionsBound).Subscribe();
            var resortTrigger = lcMethodEvents.Connect().WhenValueChanged(x => x.EventNumber).Select(_ => Unit.Default);
            lcMethodEvents.Connect().Sort(SortExpressionComparer<LCMethodEventViewModel>.Ascending(x => x.EventNumber), resort: resortTrigger).ObserveOn(RxApp.MainThreadScheduler).Bind(out var lcMethodEventsBound).Subscribe();
            SavedMethodsComboBoxOptions = savedMethodsComboBoxOptionsBound;
            ColumnComboBoxOptions = columnComboBoxOptionsBound;
            LCMethodEvents = lcMethodEventsBound;
            LoadMethodCommand = ReactiveCommand.Create(LoadMethods);
            SaveMethodCommand = ReactiveCommand.Create(SaveSelectedMethod, this.WhenAnyValue(x => x.CanSave));
            SaveAllMethodsCommand = ReactiveCommand.Create(SaveMethods, this.WhenAnyValue(x => x.CanSave));
            BuildMethodCommand = ReactiveCommand.Create(BuildSelectedMethod, this.WhenAnyValue(x => x.CanBuild));
            UpdateMethodCommand = ReactiveCommand.Create(UpdateSelectedMethod, this.WhenAnyValue(x => x.CanUpdate));
            AddEventCommand = ReactiveCommand.Create(AddEvent);
            RemoveEventCommand = ReactiveCommand.Create(DeleteEvent);
            MoveEventUpCommand = ReactiveCommand.Create(MoveEventUp);
            MoveEventDownCommand = ReactiveCommand.Create(MoveEventDown);
            SelectAllCommand = ReactiveCommand.Create(() => SetAllEventSelect(true));
            DeselectAllCommand = ReactiveCommand.Create(() => SetAllEventSelect(false));

            this.WhenAnyValue(x => x.SelectedColumn, x => x.AllowPreOverlap, x => x.AllowPostOverlap).Subscribe(x => this.OnEventChanged());
            this.WhenAnyValue(x => x.SelectedSavedMethod).Subscribe(x => this.SelectedSavedMethodChanged());
            this.WhenAnyValue(x => x.MethodName).Subscribe(x => this.MethodNameChanged());
            this.WhenAnyValue(x => x.MethodComment).Subscribe(x => this.MethodCommentChanged());
            showComment = this.WhenAnyValue(x => x.SharedUISettings.CommentsEnabled, x => x.MethodComment)
                .Select(x => x.Item1 == true || (x.Item1 == null && !string.IsNullOrWhiteSpace(x.Item2)))
                .ToProperty(this, x => x.ShowComment);
        }

        private bool IgnoreUpdates { get; set; }

        private string methodName = "";
        private readonly ObservableAsPropertyHelper<bool> showComment;
        private string methodComment = "";
        private bool allowPreOverlap;
        private bool allowPostOverlap;
        private string selectedSavedMethod = "";
        private string selectedColumn = "1";
        private bool canSave;
        private bool canBuild;
        private bool canUpdate;
        private readonly SourceList<string> savedMethodsComboBoxOptions = new SourceList<string>();
        private readonly SourceList<string> columnComboBoxOptions = new SourceList<string>();
        private readonly SourceList<LCMethodEventViewModel> lcMethodEvents = new SourceList<LCMethodEventViewModel>();

        public string MethodName
        {
            get => methodName;
            set => this.RaiseAndSetIfChanged(ref methodName, value);
        }

        public LCMethodStageSharedSettings SharedUISettings { get; } = new LCMethodStageSharedSettings();

        public bool ShowComment => showComment.Value;

        public string MethodComment
        {
            get => methodComment;
            set => this.RaiseAndSetIfChanged(ref methodComment, value);
        }

        public bool AllowPreOverlap
        {
            get => allowPreOverlap;
            set => this.RaiseAndSetIfChanged(ref allowPreOverlap, value);
        }

        public bool AllowPostOverlap
        {
            get => allowPostOverlap;
            set => this.RaiseAndSetIfChanged(ref allowPostOverlap, value);
        }

        public string SelectedSavedMethod
        {
            get => selectedSavedMethod;
            set => this.RaiseAndSetIfChanged(ref selectedSavedMethod, value);
        }

        public string SelectedColumn
        {
            get => selectedColumn;
            set => this.RaiseAndSetIfChanged(ref selectedColumn, value);
        }

        public bool CanSave
        {
            get => canSave;
            private set => this.RaiseAndSetIfChanged(ref canSave, value);
        }

        public bool CanBuild
        {
            get => canBuild;
            private set => this.RaiseAndSetIfChanged(ref canBuild, value);
        }

        public bool CanUpdate
        {
            get => canUpdate;
            private set => this.RaiseAndSetIfChanged(ref canUpdate, value);
        }

        public List<LCEvent> LCEvents
        {
            get
            {
                // Grab the selected method items from the user interfaces
                var data = new List<LCMethodEventData>();
                foreach (var lcEvent in lcMethodEvents.Items.OrderBy(x => x.EventNumber))
                {
                    data.Add(lcEvent.SelectedMethod);
                }

                // Then return a list of events for this stage that are
                // assigned with the correct timing data.
                return LCMethodOptimizer.ConstructEvents(data);
            }
        }

        public ReadOnlyObservableCollection<string> SavedMethodsComboBoxOptions { get; }
        public ReadOnlyObservableCollection<string> ColumnComboBoxOptions { get; }
        public ReadOnlyObservableCollection<LCMethodEventViewModel> LCMethodEvents { get; }

        /// <summary>
        /// Gets or sets what folder path the methods are stored in.
        /// </summary>
        public string MethodFolderPath { get; set; }

        public ReactiveCommand<Unit, Unit> LoadMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveAllMethodsCommand { get; }
        public ReactiveCommand<Unit, Unit> BuildMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> AddEventCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveEventCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveEventUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveEventDownCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }
        public ReactiveCommand<Unit, Unit> DeselectAllCommand { get; }

        private void Manager_MethodUpdated(object sender, ILCMethod ilcMethod)
        {
            if (!(ilcMethod is LCMethod method))
                return;

            //only stages that already have this method loaded should reload it.
            if (MethodName == method.Name && !triggeredUpdate)
            {
                LoadMethod(method);
            }
            else
            {
                triggeredUpdate = false;
            }
        }

        private void Manager_MethodRemoved(object sender, ILCMethod ilcMethod)
        {
            if (ilcMethod is LCMethod method && methodMap.ContainsKey(method.Name))
            {
                methodMap.Remove(method.Name);
                savedMethodsComboBoxOptions.Remove(method.Name);
            }
        }

        private void Manager_MethodAdded(object sender, ILCMethod ilcMethod)
        {
            if (ilcMethod is LCMethod method && !methodMap.ContainsKey(method.Name))
            {
                methodMap.Add(method.Name, method.Name);
                savedMethodsComboBoxOptions.Add(method.Name);
            }
        }

        /// <summary>
        /// Finds the associated method from the list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private LCMethod FindMethods(string name)
        {
            return LCMethodManager.Manager.GetLCMethodByName(name) as LCMethod;
        }

        /// <summary>
        /// Loads the method into the editor.
        /// </summary>
        /// <param name="method"></param>
        private void LoadMethod(LCMethod method)
        {
            if (method != null)
            {
                LoadMethodEvents(method);
                MethodName = method.Name;
                MethodComment = method.Comment;
            }
        }

        /// <summary>
        /// Loads the events from the selected method.
        /// </summary>
        private void SelectedSavedMethodChanged()
        {
            var name = SelectedSavedMethod;
            var method = FindMethods(name);
            IgnoreUpdates = true;
            LoadMethod(method);
            MethodName = name;
            MethodComment = method?.Comment;
            IgnoreUpdates = false;

            CanBuild = false;
            CanUpdate = MethodExists();
        }

        /// <summary>
        /// Updates the button text when the user types a method name.
        /// </summary>
        private void MethodNameChanged()
        {
            if (SelectedSavedMethod.Equals(MethodName))
            {
                return;
            }

            var text = MethodName;
            var found = false;
            foreach (var o in SavedMethodsComboBoxOptions)
            {
                if (o.Equals(text))
                {
                    found = true;
                    SelectedSavedMethod = o;
                    var method = FindMethods(text);
                    LoadMethod(method);
                    break;
                }
            }

            if (found)
            {
                IgnoreUpdates = true;
                UpdateUserInterface(false);

                OnEventChanged();
                IgnoreUpdates = false;
            }
            else
            {
                UpdateUserInterface(false);
                OnEventChanged();
            }
        }

        /// <summary>
        /// Updates the button text when the user types a method name.
        /// </summary>
        private void MethodCommentChanged()
        {
            UpdateUserInterface(true);
        }

        /// <summary>
        /// Updates the user interface if a method exists and is loaded into the User interface.
        /// </summary>
        /// <param name="needsAnUpdate"></param>
        private void UpdateUserInterface(bool needsAnUpdate)
        {
            if (!IgnoreUpdates)
            {
                CanBuild = !needsAnUpdate;
                CanUpdate = needsAnUpdate;
            }
        }

        public int GetColumn()
        {
            var column = -1;

            if (string.IsNullOrWhiteSpace(SelectedColumn))
            {
                return column;
            }

            var key = SelectedColumn;
            if (checkBoxToColumnDataMap.ContainsKey(key))
            {
                column = checkBoxToColumnDataMap[key].ID;
            }
            return column;
        }


        /// <summary>
        /// Updates the configuration data and the user interface.
        /// </summary>
        public void UpdateConfiguration()
        {
            checkBoxToColumnDataMap.Clear();
            columnComboBoxOptions.Clear();

            if (CartConfiguration.Columns == null)
                return;

            foreach (var column in CartConfiguration.Columns.Where(x => x.Status != ColumnStatus.Disabled))
            {
                var id = (column.ID + 1).ToString();
                checkBoxToColumnDataMap.Add(id, column);
                columnComboBoxOptions.Add(id);

                if (string.IsNullOrWhiteSpace(SelectedColumn))
                {
                    SelectedColumn = id;
                }
            }

            if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, true))
            {
                const string specialColName = "Special/All";
                columnComboBoxOptions.Add(specialColName);

                if (string.IsNullOrWhiteSpace(SelectedColumn))
                {
                    SelectedColumn = specialColName;
                }
            }
        }

        /// <summary>
        /// Builds the LC Method.
        /// </summary>
        /// <returns>A LC-Method if events are defined.  Null if events are not.</returns>
        public LCMethod BuildMethod()
        {
            var method = LCMethodBuilder.BuildMethod(LCEvents);
            if (method == null)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Cannot create the LC-method from an empty event list.  You need to add events to the method.");
                return null;
            }

            var column = GetColumn();
            if (column < 0)
            {
                method.IsSpecialMethod = true;
            }
            else
            {
                method.IsSpecialMethod = false;
            }
            method.Column = column;
            method.AllowPreOverlap = AllowPreOverlap;
            method.AllowPostOverlap = AllowPostOverlap;
            return method;
        }

        /// <summary>
        /// Builds the method based on the user interface input.
        /// </summary>
        private void Build()
        {
            // Construct the method
            var method = BuildMethod();
            if (method == null)
                return;

            method.Name = MethodName;
            method.Comment = MethodComment;

            // Renders the method built
            //currentMethod = method;

            // Register the method
            LCMethodManager.Manager.AddOrUpdateMethod(method);
            OnEventChanged();
        }

        private void UpdateSelectedMethod()
        {
            triggeredUpdate = true;
            Build();
            CanUpdate = false;
            CanSave = true;

            ApplicationLogger.LogMessage(0, "LC-Methods updated internally.");
        }

        /// <summary>
        /// Handles when the user wants to build the method.  Builds it for them
        /// and fires an event to build the method for preview.
        /// </summary>
        private void BuildSelectedMethod()
        {
            if (SelectedColumn == null)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_USER, "Please select a column before building the method.");
                return;
            }
            Build();

            CanUpdate = true;
            CanBuild = false;
            CanSave = true;
            ApplicationLogger.LogMessage(0, "Method built.");
        }

        private void SaveSelectedMethod()
        {
            try
            {
                SaveMethod();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not save the current method.  " + ex.Message, ex);
            }
        }

        private bool MethodExists()
        {
            if (!string.IsNullOrEmpty(MethodName))
            {
                var text = MethodName;

                foreach (var o in SavedMethodsComboBoxOptions)
                {
                    if (o.Equals(text))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a list of selected events.
        /// </summary>
        /// <returns></returns>
        private List<LCMethodEventViewModel> GetSelectedEvents()
        {
            return LCMethodEvents.Where(x => x.IsSelected).ToList();
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            UpdateUserInterface(MethodExists());
        }

        /// <summary>
        /// Adds a new event to the list of events to run.
        /// </summary>
        private LCMethodEventViewModel RegisterNewEventEventHandlers(LCMethodEventViewModel deviceEvent)
        {
            // Adds a new event into the user interface and records the new event in the event list

            // Make a locking event.
            deviceEvent.Lock += deviceEvent_Lock;
            deviceEvent.EventChanged += deviceEvent_EventChanged;

            ApplicationLogger.LogMessage(6, "Control event added - " + deviceEvent.ToString());

            return deviceEvent;
        }

        /// <summary>
        /// Loads the method to the user interface.
        /// </summary>
        /// <param name="method"></param>
        private void LoadMethodEvents(LCMethod method)
        {
            if (method == null)
            {
                lcMethodEvents.Clear();
            }

            var eventNumber = 1;
            var eventsList = new List<LCMethodEventViewModel>(method.Events.Count);
            foreach (var lcEvent in method.Events)
            {
                var parameters = new List<LCMethodEventParameter>();
                for (var i = 0; i < lcEvent.Parameters.Length; i++)
                {
                    var parameter = lcEvent.Parameters[i];
                    var name = lcEvent.ParameterNames[i];
                    ILCEventParameter vm = null;

                    if (lcEvent.MethodAttribute.DataProviderIndex == i)
                    {
                        // Figure out what index to adjust the data provider for.
                        var combo = new EventParameterEnumViewModel();

                        // Register the event to automatically get new data when the data provider has new stuff.
                        lcEvent.Device.RegisterDataProvider(lcEvent.MethodAttribute.DataProvider, combo.FillData);
                        vm = combo;
                    }
                    else
                    {
                        if (parameter != null)
                        {
                            vm = LCMethodEventViewModel.GetEventParametersFromType(parameter.GetType());
                        }
                    }

                    parameters.Add(new LCMethodEventParameter(name, parameter, vm, lcEvent.MethodAttribute.DataProvider));
                }

                var data = new LCMethodEventData(lcEvent.Device, lcEvent.Method, lcEvent.MethodAttribute, parameters)
                    { OptimizeWith = lcEvent.OptimizeWith, Comment = lcEvent.Comment };

                // Construct an event.  We send false as locked because it's not a locking event.
                var eventVm = new LCMethodEventViewModel(data, false, SharedUISettings);
                eventVm.SetBreakPoint(lcEvent.MethodData.BreakPoint);
                eventVm.EventNumber = eventNumber++;
                eventsList.Add(eventVm);
            }

            lcMethodEvents.Edit(list =>
            {
                list.Clear();
                list.AddRange(eventsList.Select(RegisterNewEventEventHandlers));
            });

            // Then set all the check boxes accordingly.
            if (method.Column < 0)
            {
                SelectedColumn = ColumnComboBoxOptions[ColumnComboBoxOptions.Count - 1];
            }
            else
            {
                var column = CartConfiguration.Columns[method.Column];
                if (column != null)
                {
                    foreach (var item in ColumnComboBoxOptions)
                    {
                        var id = (column.ID + 1).ToString();
                        if (item.ToString() == id)
                        {
                            SelectedColumn = item;
                        }
                    }
                }
            }

            AllowPreOverlap = method.AllowPreOverlap;
            AllowPostOverlap = method.AllowPostOverlap;
        }

        /// <summary>
        /// Handles creating an unlock event when the event is locked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="enabled"></param>
        /// <param name="method"></param>
        void deviceEvent_Lock(object sender, bool enabled, LCMethodEventData method)
        {
            if (!enabled)
                return;

            var senderEvent = sender as LCMethodEventViewModel;
            var newEvent = new LCMethodEventViewModel(method, true, SharedUISettings);
            lcMethodEvents.Add(RegisterNewEventEventHandlers(newEvent));
        }

        /// <summary>
        /// Alerts listeners that the event has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void deviceEvent_EventChanged(object sender, EventArgs e)
        {
            OnEventChanged();
        }

        /// <summary>
        /// Deletes the events selected.
        /// </summary>
        private void DeleteSelectedEvents()
        {
            // Find out what events to remove
            var listEvents = GetSelectedEvents();

            // Remove the events from the list
            foreach (var deviceEvent in listEvents)
            {
                var data = deviceEvent.SelectedMethod;
                var device = deviceEvent.SelectedDevice;

                foreach (var parameter in data.Parameters)
                {
                    if (!(parameter.ViewModel is EventParameterEnumViewModel combo))
                        continue;

                    var key = parameter.DataProviderName;

                    if (key == null)
                        continue;

                    try
                    {
                        device.UnRegisterDataProvider(key, combo.FillData);
                    }
                    catch (Exception ex)
                    {
                        ApplicationLogger.LogError(0, "There was an error when removing the device event. " + ex.Message, ex);
                    }
                }

                lcMethodEvents.Remove(deviceEvent);
                ApplicationLogger.LogMessage(0, $"A control event for the device {deviceEvent.SelectedDevice.Name} was  removed from method {data.Method.Name}.");
            }

            // Fix numbering after deleting all events.
            var counter = 1;
            foreach (var item in lcMethodEvents.Items.OrderBy(x => x.EventNumber))
            {
                item.EventNumber = counter++;
            }
        }

        /// <summary>
        /// Moves the selected events up in the list
        /// </summary>
        private void MoveSelectedEventsUp()
        {
            var listEvents = GetSelectedEvents();

            if (listEvents.Count <= 0)
                return;

            var orderedEvents = LCMethodEvents.OrderBy(x => x.EventNumber).ToList();
            for (var i = 0; i < orderedEvents.Count - 1; i++)
            {
                // Only moves a selected item down if it is possible.
                if (!orderedEvents[i].IsSelected && orderedEvents[i + 1].IsSelected)
                {
                    // Swap items
                    var temp = orderedEvents[i];
                    orderedEvents[i] = orderedEvents[i + 1];
                    orderedEvents[i + 1] = temp;
                }
            }

            // Update the event numbers according to the ordering in orderedEvents
            var counter = 1;
            foreach (var eventItem in orderedEvents)
            {
                eventItem.EventNumber = counter++;
            }

            ApplicationLogger.LogMessage(0, "Control event moved up.");
        }

        /// <summary>
        /// Moves the selected events up in the list
        /// </summary>
        private void MoveSelectedEventsDown()
        {
            var listEvents = GetSelectedEvents();

            if (listEvents.Count <= 0)
                return;

            var orderedEvents = LCMethodEvents.OrderBy(x => x.EventNumber).ToList();
            for (var i = orderedEvents.Count - 1; i > 0; i--)
            {
                // Only moves a selected item down if it is possible.
                if (!orderedEvents[i].IsSelected && orderedEvents[i - 1].IsSelected)
                {
                    // Swap items
                    var temp = orderedEvents[i];
                    orderedEvents[i] = orderedEvents[i - 1];
                    orderedEvents[i - 1] = temp;
                }
            }

            // Update the event numbers according to the ordering in orderedEvents
            var counter = 1;
            foreach (var eventItem in orderedEvents)
            {
                eventItem.EventNumber = counter++;
            }

            ApplicationLogger.LogMessage(0, "Control event moved down.");
        }

        /// <summary>
        /// Adds a new event.
        /// </summary>
        private void AddEvent()
        {
            lcMethodEvents.Add(RegisterNewEventEventHandlers(new LCMethodEventViewModel(lcMethodEvents.Count + 1, SharedUISettings)));
            OnEventChanged();
        }

        /// <summary>
        /// Removes the checked events.
        /// </summary>
        private void DeleteEvent()
        {
            DeleteSelectedEvents();
            OnEventChanged();
        }

        /// <summary>
        /// Moves the LC event down in the list.
        /// </summary>
        private void MoveEventDown()
        {
            MoveSelectedEventsDown();
            OnEventChanged();
        }

        /// <summary>
        /// Moves the LC event up in the list.
        /// </summary>
        private void MoveEventUp()
        {
            MoveSelectedEventsUp();
            OnEventChanged();
        }

        private void SetAllEventSelect(bool selected)
        {
            foreach (var lcEvent in LCMethodEvents)
            {
                lcEvent.IsSelected = selected;
            }
        }

        /// <summary>
        /// Saves the given method to file.
        /// </summary>
        /// <param name="method"></param>
        public bool SaveMethod(LCMethod method)
        {
            if (method == null)
                return false;

            // Construct the path
            var path = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), LCMethodXmlFile.CONST_LC_METHOD_FOLDER);
            path = Path.Combine(path, method.Name + LCMethodXmlFile.CONST_LC_METHOD_EXTENSION);

            // Write the method out!
            ApplicationLogger.LogMessage(0, "Writing method to file " + path);
            return LCMethodXmlFile.WriteMethod(path, method);
        }

        /// <summary>
        /// Saves the current method selected in the method combo box.
        /// </summary>
        public void SaveMethod()
        {
            if (!string.IsNullOrWhiteSpace(SelectedSavedMethod))
            {
                var method = FindMethods(SelectedSavedMethod);

                if (method == null)
                    return;

                SaveMethod(method);

                MessageBox.Show("Saved method " + method.Name);
            }
        }

        /// <summary>
        /// Saves all of the methods.
        /// </summary>
        public void SaveMethods()
        {
            foreach (var name in SavedMethodsComboBoxOptions)
            {
                var method = FindMethods(name);
                SaveMethod(method);
            }

            CanSave = false;
            ApplicationLogger.LogMessage(0, "LC-Methods Saved.");
        }

        /// <summary>
        /// Opens a method.
        /// </summary>
        public void OpenMethod(string path)
        {
            var errors = new List<Exception>();
            var method = LCMethodXmlFile.ReadMethod(path, errors);

            if (method != null)
            {
                LCMethodManager.Manager.AddOrUpdateMethod(method);
            }
        }

        /// <summary>
        /// Loads method stored in the method folder directory.
        /// </summary>
        public void LoadMethods()
        {
            var methodPath = PersistDataPaths.GetDirectoryLoadPathCheckFiles(MethodFolderPath, "*.xml");
            var methods = Directory.GetFiles(methodPath, "*.xml");
            foreach (var method in methods)
            {
                ApplicationLogger.LogMessage(0, "Loading method " + method);
                OpenMethod(method);
            }

            ApplicationLogger.LogMessage(0, "Methods loaded.");
        }
    }
}
