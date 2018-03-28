using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Data;
using LcmsNetSDK;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
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
        private readonly Dictionary<string, classColumnData> checkBoxToColumnDataMap;

        /// <summary>
        /// List of controls that manage events.
        /// </summary>
        private readonly List<LCMethodEventViewModel> eventsList;

        /// <summary>
        /// Constructor for holding the list of events.
        /// </summary>
        public LCMethodStageViewModel()
        {
            eventsList = new List<LCMethodEventViewModel>();
            methodMap = new Dictionary<string, string>();
            MethodFolderPath = CONST_METHOD_FOLDER_PATH;

            // Build the button to method dictionary.
            checkBoxToColumnDataMap = new Dictionary<string, classColumnData>();
            UpdateConfiguration();
            //TODO: In Code-behind?: MethodName.LostFocus += MethodName_LostFocus;
            classLCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
            classLCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;
            classLCMethodManager.Manager.MethodUpdated += Manager_MethodUpdated;

            BindingOperations.EnableCollectionSynchronization(SavedMethodsComboBoxOptions, savedMethodsComboBoxOptionsLock);
            BindingOperations.EnableCollectionSynchronization(ColumnComboBoxOptions, columnComboBoxOptionsLock);
            BindingOperations.EnableCollectionSynchronization(LCMethodEvents, lcMethodEventsLock);

            SetupCommands();

            this.WhenAnyValue(x => x.SelectedColumn, x => x.AllowPreOverlap, x => x.AllowPostOverlap).Subscribe(x => this.OnEventChanged());
            this.WhenAnyValue(x => x.SelectedSavedMethod).Subscribe(x => this.SelectedSavedMethodChanged());
            this.WhenAnyValue(x => x.MethodName).Subscribe(x => this.MethodNameChanged());
        }

        private bool IgnoreUpdates { get; set; }

        private string methodName = "";
        private bool allowPreOverlap;
        private bool allowPostOverlap;
        private string selectedSavedMethod = "";
        private string selectedColumn = "";
        private bool canSave;
        private bool canBuild;
        private bool canUpdate;
        private readonly object savedMethodsComboBoxOptionsLock = new object();
        private readonly object columnComboBoxOptionsLock = new object();
        private readonly object lcMethodEventsLock = new object();
        private readonly ReactiveList<string> savedMethodsComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<string> columnComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<LCMethodEventViewModel> lcMethodEvents = new ReactiveList<LCMethodEventViewModel>();

        #region Properties

        public string MethodName
        {
            get { return methodName; }
            set { this.RaiseAndSetIfChanged(ref methodName, value); }
        }

        public bool AllowPreOverlap
        {
            get { return allowPreOverlap; }
            set { this.RaiseAndSetIfChanged(ref allowPreOverlap, value); }
        }

        public bool AllowPostOverlap
        {
            get { return allowPostOverlap; }
            set { this.RaiseAndSetIfChanged(ref allowPostOverlap, value); }
        }

        public string SelectedSavedMethod
        {
            get { return selectedSavedMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedSavedMethod, value); }
        }

        public string SelectedColumn
        {
            get { return selectedColumn; }
            set { this.RaiseAndSetIfChanged(ref selectedColumn, value); }
        }

        public bool CanSave
        {
            get { return canSave; }
            private set { this.RaiseAndSetIfChanged(ref canSave, value); }
        }

        public bool CanBuild
        {
            get { return canBuild; }
            private set { this.RaiseAndSetIfChanged(ref canBuild, value); }
        }

        public bool CanUpdate
        {
            get { return canUpdate; }
            private set { this.RaiseAndSetIfChanged(ref canUpdate, value); }
        }

        public List<classLCEvent> LCEvents
        {
            get
            {
                // Grab the selected method items from the user interfaces
                var data = new List<classLCMethodData>();
                foreach (var lcEvent in eventsList)
                {
                    data.Add(lcEvent.SelectedMethod);
                }

                // Then return a list of events for this stage that are
                // assigned with the correct timing data.
                return classLCMethodOptimizer.ConstructEvents(data);
            }
        }

        public IReadOnlyReactiveList<string> SavedMethodsComboBoxOptions => savedMethodsComboBoxOptions;
        public IReadOnlyReactiveList<string> ColumnComboBoxOptions => columnComboBoxOptions;
        public IReadOnlyReactiveList<LCMethodEventViewModel> LCMethodEvents => lcMethodEvents;

        #endregion

        /// <summary>
        /// Gets or sets what folder path the methods are stored in.
        /// </summary>
        public string MethodFolderPath { get; set; }

        bool Manager_MethodUpdated(object sender, classLCMethod method)
        {
            //only stages that already have this method loaded should reload it.
            if (MethodName == method.Name && !triggeredUpdate)
            {
                LoadMethod(method);
            }
            else
            {
                triggeredUpdate = false;
            }
            return true;
        }

        public bool Manager_MethodRemoved(object sender, classLCMethod method)
        {
            if (method != null)
            {
                if (methodMap.ContainsKey(method.Name))
                {
                    methodMap.Remove(method.Name);
                    ComboBoxSavedItemsRemoveItem(method.Name);
                }
            }
            return true;
        }

        public bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            if (method != null)
            {
                if (!methodMap.ContainsKey(method.Name))
                {
                    methodMap.Add(method.Name, method.Name);
                    ComboBoxSavedItemsAddItem(method.Name);
                }
            }
            return true;
        }

        public void ComboBoxSavedItemsRemoveItem(string name)
        {
            savedMethodsComboBoxOptions.Remove(name);
        }

        public void ComboBoxSavedItemsAddItem(string name)
        {
            savedMethodsComboBoxOptions.Add(name);
        }

        /// <summary>
        /// Finds the associated method from the list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private classLCMethod FindMethods(string name)
        {
            classLCMethod method = null;
            if (classLCMethodManager.Manager.Methods.ContainsKey(name))
            {
                method = classLCMethodManager.Manager.Methods[name];
            }
            return method;
        }

        /// <summary>
        /// Loads the method into the editor.
        /// </summary>
        /// <param name="method"></param>
        private void LoadMethod(classLCMethod method)
        {
            if (method != null)
            {
                LoadMethod(method, true);
                MethodName = method.Name;
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
            OnEditMethod();
            IgnoreUpdates = false;

            CanBuild = false;
            CanUpdate = MethodExists();
        }

        private void OnEditMethod()
        {
            UpdatingMethod?.Invoke(this, new classMethodEditingEventArgs(MethodName));
        }

        /// <summary>
        /// Updates the button text when the user types a method name.
        /// </summary>
        private void MethodNameChanged()
        {
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
                    OnEditMethod();
                    break;
                }
            }

            if (found)
            {
                IgnoreUpdates = true;
                UpdateUserInterface(false);

                OnEventChanged();
                OnEditMethod();
                IgnoreUpdates = false;
            }
            else
            {
                UpdateUserInterface(false);
                OnEventChanged();
                OnEditMethod();
            }
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

            if (SelectedColumn == null)
            {
                return column;
            }

            var key = SelectedColumn.ToString();
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

            if (classCartConfiguration.Columns == null)
                return;

            foreach (var column in classCartConfiguration.Columns)
            {
                var id = (column.ID + 1).ToString();
                checkBoxToColumnDataMap.Add(id, column);
                columnComboBoxOptions.Add(id);
            }
            columnComboBoxOptions.Add("Special/All");
        }

        public bool IsColumnSelected()
        {
            if (SelectedColumn == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// returns index of the event in the event list, used to number the events on screen.
        /// </summary>
        /// <param name="thisEvent"></param>
        /// <returns>an integer</returns>
        public int EventIndex(LCMethodEventViewModel thisEvent)
        {
            return eventsList.IndexOf(thisEvent);
        }

        /// <summary>
        /// Builds the LC Method.
        /// </summary>
        /// <returns>A LC-Method if events are defined.  Null if events are not.</returns>
        public classLCMethod BuildMethod()
        {
            var method = classLCMethodBuilder.BuildMethod(LCEvents);
            if (method == null)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
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

            // Renders the method built
            //currentMethod = method;

            // Register the method
            classLCMethodManager.Manager.AddMethod(method);
            OnEventChanged();
        }

        private void UpdateSelectedMethod()
        {
            triggeredUpdate = true;
            Build();
            CanUpdate = false;
            CanSave = true;

            classApplicationLogger.LogMessage(0, "LC-Methods updated internally.");
        }

        /// <summary>
        /// Handles when the user wants to build the method.  Builds it for them
        /// and fires an event to build the method for preview.
        /// </summary>
        private void BuildSelectedMethod()
        {
            var columnSelected = IsColumnSelected();
            if (!columnSelected)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER, "Please select a column before building the method.");
                return;
            }
            Build();

            CanUpdate = true;
            CanBuild = false;
            CanSave = true;
            classApplicationLogger.LogMessage(0, "Method built.");
        }

        private void SaveSelectedMethod()
        {
            try
            {
                SaveMethod();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not save the current method.  " + ex.Message, ex);
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
        /// Fired when editing a method.
        /// </summary>
        public event EventHandler<classMethodEditingEventArgs> UpdatingMethod;

        public class classMethodEditingEventArgs : EventArgs
        {
            public classMethodEditingEventArgs(string methodName)
            {
                Name = methodName;
            }

            public string Name { get; private set; }
        }

        #region Event List Control for Adding, Deleting, Moving, and Rendering LC-Events

        /// <summary>
        /// Re-renderes the event list.
        /// </summary>
        private void RenderEventList()
        {
            using (lcMethodEvents.SuppressChangeNotifications())
            {
                lcMethodEvents.Clear();
                lcMethodEvents.AddRange(eventsList);
            }
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
        /// Adds a new event to the list of events to run.
        /// </summary>
        private void AddNewEvent()
        {
            var deviceEvent = new LCMethodEventViewModel(eventsList.Count + 1);
            AddNewEvent(deviceEvent);
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
        private void AddNewEvent(LCMethodEventViewModel deviceEvent)
        {
            // Adds a new event into the user interface and records the new event in the event list

            // Make a locking event.
            deviceEvent.Lock += deviceEvent_Lock;
            deviceEvent.EventChanged += deviceEvent_EventChanged;

            var eventData = "";
            if (deviceEvent.SelectedMethod?.MethodAttribute != null)
            {
                eventData = string.Format("{0} - {1}", deviceEvent.SelectedMethod.Device.Name, deviceEvent.SelectedMethod.MethodAttribute.Name, deviceEvent.SelectedMethod.Parameters);
            }
            classApplicationLogger.LogMessage(0, "Control event added - " + eventData);
            eventsList.Add(deviceEvent);
            RenderEventList();
        }

        /// <summary>
        /// Loads the method to the user interface.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="clearOld"></param>
        public void LoadMethod(classLCMethod method, bool clearOld)
        {
            if (clearOld)
                eventsList.Clear();

            foreach (var lcEvent in method.Events)
            {
                var parameters = new classLCMethodEventParameter();
                for (var i = 0; i < lcEvent.Parameters.Length; i++)

                {
                    var parameter = lcEvent.Parameters[i];
                    var name = lcEvent.ParameterNames[i];
                    ILCEventParameter control = null;

                    if (lcEvent.MethodAttribute.DataProviderIndex == i)
                    {
                        // Figure out what index to adjust the data provider for.
                        var combo = new EventParameterViewModel(EventParameterViewModel.ParameterTypeEnum.Enum);

                        // Register the event to automatically get new data when the data provider has new stuff.
                        lcEvent.Device.RegisterDataProvider(lcEvent.MethodAttribute.DataProvider, combo.FillData);
                        control = combo;
                    }
                    else
                    {
                        if (parameter != null)
                        {
                            control = LCMethodEventViewModel.GetEventParametersFromType(parameter.GetType());
                        }
                    }

                    parameters.AddParameter(parameter, control, name, lcEvent.MethodAttribute.DataProvider);
                }

                var data = new classLCMethodData(lcEvent.Device, lcEvent.Method, lcEvent.MethodAttribute, parameters)
                    { OptimizeWith = lcEvent.OptimizeWith };

                // Construct an event.  We send false as locked because its not a locking event.
                var controlEvent = new LCMethodEventViewModel(data, false);
                controlEvent.SetBreakPoint(lcEvent.BreakPoint);
                AddNewEvent(controlEvent);
                controlEvent.UpdateEventNum(EventIndex(controlEvent) + 1);
            }
            // Then set all the check boxes accordingly.
            if (method.Column < 0)
            {
                SelectedColumn = ColumnComboBoxOptions[ColumnComboBoxOptions.Count - 1];
            }
            else
            {
                var column = classCartConfiguration.Columns[method.Column];
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
        void deviceEvent_Lock(object sender, bool enabled, classLCMethodData method)
        {
            if (!enabled)
                return;

            var senderEvent = sender as LCMethodEventViewModel;
            var newEvent = new LCMethodEventViewModel(method, true);
            AddNewEvent(newEvent);
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
                var indexOfLastDeleted = eventsList.IndexOf(deviceEvent);
                var data = deviceEvent.SelectedMethod;
                var device = deviceEvent.SelectedDevice;
                var parameter = data.Parameters;

                for (var i = 0; i < parameter.Names.Count; i++)
                {
                    var combo = parameter.Controls[i] as EventParameterViewModel;

                    if (combo == null)
                        continue;

                    var key = parameter.DataProviderNames[i];

                    if (key == null)
                        continue;

                    try
                    {
                        device.UnRegisterDataProvider(key, combo.FillData);
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(0, "There was an error when removing the device event. " + ex.Message, ex);
                    }
                }
                for (var i = indexOfLastDeleted; i < eventsList.Count; i++)
                {
                    eventsList[i].UpdateEventNum(eventsList.IndexOf(eventsList[i]));
                }
                lcMethodEvents.Remove(deviceEvent);
                eventsList.Remove(deviceEvent);
                classApplicationLogger.LogMessage(0, string.Format("A control event for the device {0} was  removed from method {1}.", deviceEvent.SelectedDevice.Name, data.Method.Name));
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

            var indices = new int[listEvents.Count];

            // Construct the pivot table.
            var i = 0;
            foreach (var devEvent in listEvents)
            {
                var index = eventsList.IndexOf(devEvent);
                indices[i++] = index;
            }

            // Use the pivot table to move the items around
            var maxPos = 0;

            // Don't worry about checking for the length of indices because we already did that above
            // when we checked the number of selected events being of the right size.
            if (indices[0] == 0)
                maxPos = 1;

            for (var j = 0; j < i; j++)
            {
                if (indices[j] > maxPos)
                {
                    // Now swap the events
                    var preEvent = eventsList[indices[j]];
                    eventsList[indices[j]] = eventsList[indices[j] - 1];
                    eventsList[indices[j] - 1] = preEvent;
                    //update event list positions
                    eventsList[indices[j]].UpdateEventNum(eventsList.IndexOf(eventsList[indices[j]]) + 1);
                    preEvent.UpdateEventNum(eventsList.IndexOf(preEvent) + 1);

                    // Update the pivot index now so that we cannot advance above that.
                    maxPos = indices[j];
                }
            }

            classApplicationLogger.LogMessage(0, "Control event moved up.");
            RenderEventList();
        }

        /// <summary>
        /// Moves the selected events up in the list
        /// </summary>
        private void MoveSelectedEventsDown()
        {
            var listEvents = GetSelectedEvents();
            if (listEvents.Count <= 0)
                return;

            var indices = new int[listEvents.Count];
            // Construct the pivot table.
            var i = 0;
            foreach (var devEvent in listEvents)
            {
                var index = eventsList.IndexOf(devEvent);
                indices[i++] = index;
            }

            // Use the pivot table to move the items around
            var maxPos = eventsList.Count - 1;
            if (maxPos == indices[indices.Length - 1])
            {
                maxPos--;
            }

            for (var j = i - 1; j >= 0; j--)
            {
                if (indices[j] < maxPos)
                {
                    // Now swap the events
                    var preEvent = eventsList[indices[j]];
                    eventsList[indices[j]] = eventsList[indices[j] + 1];
                    eventsList[indices[j] + 1] = preEvent;
                    eventsList[indices[j]].UpdateEventNum(eventsList.IndexOf(eventsList[indices[j]]) + 1);
                    preEvent.UpdateEventNum(eventsList.IndexOf(preEvent) + 1);
                    // Update the pivot index now so that we cannot advance above that.
                }
                maxPos = indices[j];
            }

            classApplicationLogger.LogMessage(0, "Control event moved down.");
            RenderEventList();
        }

        #endregion

        #region Control Event Handlers

        /// <summary>
        /// Adds a new event.
        /// </summary>
        private void AddEvent()
        {
            AddNewEvent();
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

        #endregion

        #region Saving and Loading

        /// <summary>
        /// Saves the given method to file.
        /// </summary>
        /// <param name="method"></param>
        public bool SaveMethod(classLCMethod method)
        {
            if (method == null)
                return false;

            // Create a new writer.
            var writer = new classLCMethodWriter();

            // Construct the path
            var path = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH), classLCMethodFactory.CONST_LC_METHOD_FOLDER);
            path = Path.Combine(path, method.Name + classLCMethodFactory.CONST_LC_METHOD_EXTENSION);

            // Write the method out!
            classApplicationLogger.LogMessage(0, "Writing method to file " + path);
            return writer.WriteMethod(path, method);
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
            classApplicationLogger.LogMessage(0, "LC-Methods Saved.");
        }

        /// <summary>
        /// Opens a method.
        /// </summary>
        public void OpenMethod(string path)
        {
            var reader = new classLCMethodReader();
            var errors = new List<Exception>();
            var method = reader.ReadMethod(path, errors);

            if (method != null)
            {
                classLCMethodManager.Manager.AddMethod(method);
            }
        }

        /// <summary>
        /// Loads method stored in the method folder directory.
        /// </summary>
        public void LoadMethods()
        {
            var methods = Directory.GetFiles(MethodFolderPath, "*.xml");
            foreach (var method in methods)
            {
                classApplicationLogger.LogMessage(0, "Loading method " + method);
                OpenMethod(method);
            }

            classApplicationLogger.LogMessage(0, "Methods loaded.");
        }

        #endregion

        public ReactiveCommand<Unit, Unit> LoadMethodCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveMethodCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAllMethodsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> BuildMethodCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UpdateMethodCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AddEventCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RemoveEventCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> MoveEventUpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> MoveEventDownCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SelectAllCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeselectAllCommand { get; private set; }

        private void SetupCommands()
        {
            LoadMethodCommand = ReactiveCommand.Create(() => LoadMethods());
            SaveMethodCommand = ReactiveCommand.Create(() => SaveSelectedMethod(), this.WhenAnyValue(x => x.CanSave));
            SaveAllMethodsCommand = ReactiveCommand.Create(() => SaveMethods(), this.WhenAnyValue(x => x.CanSave));
            BuildMethodCommand = ReactiveCommand.Create(() => BuildSelectedMethod(), this.WhenAnyValue(x => x.CanBuild));
            UpdateMethodCommand = ReactiveCommand.Create(() => UpdateSelectedMethod(), this.WhenAnyValue(x => x.CanUpdate));
            AddEventCommand = ReactiveCommand.Create(() => AddEvent());
            RemoveEventCommand = ReactiveCommand.Create(() => DeleteEvent());
            MoveEventUpCommand = ReactiveCommand.Create(() => MoveEventUp());
            MoveEventDownCommand = ReactiveCommand.Create(() => MoveEventDown());
            SelectAllCommand = ReactiveCommand.Create(() => SetAllEventSelect(true));
            DeselectAllCommand = ReactiveCommand.Create(() => SetAllEventSelect(false));
        }
    }
}
