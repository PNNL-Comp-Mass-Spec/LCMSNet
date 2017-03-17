using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using LcmsNet.Properties;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Control that represents a specific stage in the LC-Experiment.
    /// </summary>
    public partial class controlLCMethodStage : UserControl
    {
        /// <summary>
        /// Mode used to preview the method throughput.
        /// </summary>
        public enum enumLCMethodRenderMode
        {
            Column,
            Time
        }

        /// <summary>
        /// Constant defining where the LC-Methods are stored.
        /// </summary>
        private const string CONST_METHOD_FOLDER_PATH = "LCMethods";

        /// <summary>
        /// Maps a method name to itself so that we can update the ui from the method manager.
        /// </summary>
        private readonly Dictionary<string, string> m_methodMap;

        private bool m_triggeredUpdate;

        /// <summary>
        /// Maps the check box clicked to a specific column data.
        /// </summary>
        private readonly Dictionary<string, classColumnData> m_checkBoxToColumnDataMap;

        /// <summary>
        /// List of controls that manage events.
        /// </summary>
        private readonly List<controlLCMethodEvent> m_events;

        /// <summary>
        /// Constructor for holding the list of events.
        /// </summary>
        public controlLCMethodStage()
        {
            InitializeComponent();
            m_events = new List<controlLCMethodEvent>();
            m_methodMap = new Dictionary<string, string>();
            MethodFolderPath = CONST_METHOD_FOLDER_PATH;

            // Build the button to method dictionary.
            m_checkBoxToColumnDataMap = new Dictionary<string, classColumnData>();
            UpdateConfiguration();
            mtextBox_methodName.LostFocus += mtextBox_methodName_LostFocus;
            classLCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
            classLCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;
            classLCMethodManager.Manager.MethodUpdated += Manager_MethodUpdated;
        }

        private bool IgnoreUpdates { get; set; }

        public bool AllowPreOverlap
        {
            get { return mcheckBox_preOverlap.Checked; }
            set { mcheckBox_preOverlap.Checked = value; }
        }

        public bool AllowPostOverlap
        {
            get { return mcheckBox_postOverlap.Checked; }
            set { mcheckBox_postOverlap.Checked = value; }
        }

        #region Properties

        public List<classLCEvent> LCEvents
        {
            get
            {
                //
                // Grab the selected method items from the user interfaces
                //
                var data = new List<classLCMethodData>();
                foreach (var lcEvent in m_events)
                {
                    data.Add(lcEvent.SelectedMethod);
                }

                //
                // Then return a list of events for this stage that are
                // assigned with the correct timing data.
                //
                return classLCMethodOptimizer.ConstructEvents(data);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets what folder path the methods are stored in.
        /// </summary>
        public string MethodFolderPath { get; set; }

        public event EventHandler EventChanged;


        bool Manager_MethodUpdated(object sender, classLCMethod method)
        {
            //only stages that already have this method loaded should reload it.
            if (TextBoxNameGetText() == method.Name && !m_triggeredUpdate)
            {
                LoadMethod(method);
            }
            else
            {
                m_triggeredUpdate = false;
            }
            return true;
        }

        public bool Manager_MethodRemoved(object sender, classLCMethod method)
        {
            if (method != null)
            {
                if (m_methodMap.ContainsKey(method.Name))
                {
                    m_methodMap.Remove(method.Name);
                    ComboBoxSavedItemsRemoveItem(method.Name);
                }
            }
            return true;
        }

        public bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            if (method != null)
            {
                if (!m_methodMap.ContainsKey(method.Name))
                {
                    m_methodMap.Add(method.Name, method.Name);
                    ComboBoxSavedItemsAddItem(method.Name);
                }
            }
            return true;
        }

        public void ComboBoxSavedItemsRemoveItem(string name)
        {
            mcomboBox_savedMethods.Items.Remove(name);
        }

        public void ComboBoxSavedItemsAddItem(string name)
        {
            mcomboBox_savedMethods.Items.Add(name);
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
                TextBoxNameSetText(method.Name);
            }
        }

        /// <summary>
        /// Loads the events from the selected method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcomboBox_savedMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = mcomboBox_savedMethods.Text;
            var method = FindMethods(name);
            IgnoreUpdates = true;
            LoadMethod(method);
            TextBoxNameSetText(name);
            OnEditMethod();
            IgnoreUpdates = false;

            mbutton_build.Enabled = false;
            mbutton_buildUpdate.Enabled = MethodExists();
        }

        private void OnEditMethod()
        {
            UpdatingMethod?.Invoke(this, new classMethodEditingEventArgs(mtextBox_methodName.Text));
        }

        /// <summary>
        /// Updates the button text when the user types a method name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtextBox_methodName_TextChanged(object sender, EventArgs e)
        {
            var text = mtextBox_methodName.Text;
            var found = false;
            foreach (var o in mcomboBox_savedMethods.Items)
            {
                if (o.ToString().Equals(text))
                {
                    found = true;
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
                UpdateUserInterface(found);
                OnEventChanged();
                OnEditMethod();
            }
        }

        /// <summary>
        /// Updates the user interface if a method exists and is loaded into the User interface.
        /// </summary>
        /// <param name="?"></param>
        private void UpdateUserInterface(bool needsAnUpdate)
        {
            if (!IgnoreUpdates)
            {
                mbutton_build.Enabled = (!needsAnUpdate);
                mbutton_buildUpdate.Enabled = needsAnUpdate;
            }
        }

        public string TextBoxNameGetText()
        {
            return mtextBox_methodName.Text;
        }

        public void TextBoxNameSetText(string text)
        {
            mtextBox_methodName.Text = text;
        }

        public ComboBox.ObjectCollection GetSavedMethods()
        {
            return mcomboBox_savedMethods.Items;
        }

        public int GetColumn()
        {
            var column = -1;

            if (mcomboBox_column.SelectedItem == null)
            {
                return column;
            }

            var key = mcomboBox_column.SelectedItem.ToString();
            if (m_checkBoxToColumnDataMap.ContainsKey(key))
            {
                column = m_checkBoxToColumnDataMap[key].ID;
            }
            return column;
        }


        /// <summary>
        /// Updates the configuration data and the user interface.
        /// </summary>
        /// <param name="configuration"></param>
        public void UpdateConfiguration()
        {
            m_checkBoxToColumnDataMap.Clear();
            mcomboBox_column.Items.Clear();

            if (classCartConfiguration.Columns == null)
                return;

            foreach (var column in classCartConfiguration.Columns)
            {
                var id = (column.ID + 1).ToString();
                m_checkBoxToColumnDataMap.Add(id, column);
                mcomboBox_column.Items.Add(id);
            }
            mcomboBox_column.Items.Add("Special/All");
        }

        public bool IsColumnSelected()
        {
            if (mcomboBox_column.SelectedItem == null)
            {
                return false;
            }
            return true;
        }

        private void mcomboBox_column_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnEventChanged();
        }

        private void mcheckBox_preOverlap_CheckedChanged(object sender, EventArgs e)
        {
            OnEventChanged();
        }

        private void mcheckBox_postOverlap_CheckedChanged(object sender, EventArgs e)
        {
            OnEventChanged();
        }

        /// <summary>
        /// returns index of the event in the event list, used to number the events on screen.
        /// </summary>
        /// <param name="thisEvent"></param>
        /// <returns>an integer</returns>
        public int eventIndex(controlLCMethodEvent thisEvent)
        {
            return m_events.IndexOf(thisEvent);
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
            //
            // Construct the method
            //
            classLCMethod method = null;
            method = BuildMethod();
            if (method == null)
                return;

            method.Name = TextBoxNameGetText();

            //
            // Renders the method built
            //
            //m_currentMethod = method;

            //
            // Register the method
            //
            classLCMethodManager.Manager.AddMethod(method);
            OnEventChanged();
        }


        private void mbutton_buildUpdate_Click(object sender, EventArgs e)
        {
            m_triggeredUpdate = true;
            Build();
            mbutton_buildUpdate.Enabled = false;
            mbutton_saveAll.Enabled = true;
            mbutton_save.Enabled = true;

            classApplicationLogger.LogMessage(0, "LC-Methods updated internally.");
        }

        /// <summary>
        /// Handles when the user wants to build the method.  Builds it for them
        /// and fires an event to build the method for preview.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event arguments</param>
        private void mbutton_build_Click(object sender, EventArgs e)
        {
            var columnSelected = IsColumnSelected();
            if (!columnSelected)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Please select a column before building the method.");
                return;
            }
            Build();

            mbutton_saveAll.Image = Resources.SaveWithIndicator;
            mbutton_buildUpdate.Enabled = true;
            mbutton_build.Enabled = false;
            mbutton_saveAll.Enabled = true;
            mbutton_save.Enabled = true;
            classApplicationLogger.LogMessage(0, "Method built.");
        }

        /// <summary>
        /// Handles when the text box loses focus and checks to see if the method needs to change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mtextBox_methodName_LostFocus(object sender, EventArgs e)
        {
            var text = mtextBox_methodName.Text;
            foreach (var o in mcomboBox_savedMethods.Items)
            {
                if (o.ToString().Equals(text))
                {
                    mcomboBox_savedMethods.SelectedItem = o;
                    var method = FindMethods(text);
                    LoadMethod(method);
                    OnEditMethod();
                    break;
                }
            }
        }

        private void mbutton_load_Click(object sender, EventArgs e)
        {
            LoadMethods();
        }

        private void mbutton_save_Click(object sender, EventArgs e)
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

        private void mbutton_saveAll_Click(object sender, EventArgs e)
        {
            SaveMethods();
        }

        private bool MethodExists()
        {
            var found = false;
            if (!string.IsNullOrEmpty(mtextBox_methodName.Text))
            {
                var text = mtextBox_methodName.Text;

                foreach (var o in mcomboBox_savedMethods.Items)
                {
                    if (o.ToString().Equals(text))
                    {
                        found = true;
                        break;
                    }
                }
            }
            else
            {
                found = false;
            }

            return found;
        }

        /// <summary>
        /// Fired when editing a method.
        /// </summary>
        public event EventHandler<classMethodEditingEventArgs> UpdatingMethod;


        /// <summary>
        /// Arguments for
        /// </summary>
        public class EditingLCMethod : EventArgs
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="methodName">Method currently editing.</param>
            /// <param name="modified">Flag indicating if a sample has been saved.</param>
            public EditingLCMethod(string methodName, bool modified)
            {
                MethodName = methodName;
                IsDirty = modified;
            }

            /// <summary>
            /// Gets the flag indicating if the sample method needs to be saved.
            /// </summary>
            public bool IsDirty { get; private set; }

            /// <summary>
            /// Gets the method name being edited.
            /// </summary>
            public string MethodName { get; private set; }
        }

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
            mpanel_stage.SuspendLayout();
            mpanel_stage.Controls.Clear();

            var events = new List<controlLCMethodEvent>();
            events.AddRange(m_events);
            events.Reverse();

            //
            // Render them from back to front since we are using docking = top
            // This way they will show up in the order to be executed at the top
            //
            var tabIndex = mpanel_stage.TabIndex + events.Count + 1;
            for (var i = 0; i < events.Count; i++)
            {
                var lcEvent = events[i];
                lcEvent.TabIndex = tabIndex--;
                lcEvent.BringToFront();
                mpanel_stage.Controls.Add(lcEvent);
            }
            mpanel_stage.ResumeLayout();
        }

        /// <summary>
        /// Returns a list of selected events.
        /// </summary>
        /// <returns></returns>
        private List<controlLCMethodEvent> GetSelectedEvents()
        {
            var listEvents = new List<controlLCMethodEvent>();
            foreach (var stage in m_events)
            {
                if (stage.IsSelected)
                {
                    listEvents.Add(stage);
                }
            }
            return listEvents;
        }

        /// <summary>
        /// Adds a new event to the list of events to run.
        /// </summary>
        private void AddNewEvent()
        {
            var deviceEvent = new controlLCMethodEvent(m_events.Count + 1);
            AddNewEvent(deviceEvent);
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            EventChanged?.Invoke(this, null);
            UpdateUserInterface(MethodExists());
        }

        /// <summary>
        /// Adds a new event to the list of events to run.
        /// </summary>
        private void AddNewEvent(controlLCMethodEvent deviceEvent)
        {
            //
            // Adds a new event into the user interface and records the new event in the event list
            //
            deviceEvent.Dock = DockStyle.Top;

            //
            // Make a locking event.
            //
            deviceEvent.Lock += deviceEvent_Lock;
            deviceEvent.EventChanged += deviceEvent_EventChanged;

            var eventData = "";
            if (deviceEvent.SelectedMethod != null)
            {
                if (deviceEvent.SelectedMethod.MethodAttribute != null)
                {
                    eventData = string.Format("{0} - {1}",
                        deviceEvent.SelectedMethod.Device.Name,
                        deviceEvent.SelectedMethod.MethodAttribute.Name,
                        deviceEvent.SelectedMethod.Parameters);
                }
            }
            classApplicationLogger.LogMessage(0, "Control event added - " + eventData);
            m_events.Add(deviceEvent);
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
                m_events.Clear();
            SuspendLayout();
            foreach (var lcEvent in method.Events)
            {
                var parameters = new classLCMethodEventParameter();
                for (var i = 0; i < lcEvent.Parameters.Length; i++)

                {
                    var parameter = lcEvent.Parameters[i];
                    var name = lcEvent.ParameterNames[i];
                    Control control = null;

                    if (lcEvent.MethodAttribute.DataProviderIndex == i)
                    {
                        //
                        // Figure out what index to adjust the data provider for.
                        //
                        var combo = new controlParameterComboBox();

                        //
                        // Register the event to automatically get new data when the data provider has new
                        // stuff.
                        //
                        lcEvent.Device.RegisterDataProvider(lcEvent.MethodAttribute.DataProvider,
                            combo.FillData);
                        control = combo;
                    }
                    else
                    {
                        if (parameter != null)
                        {
                            control = controlLCMethodEvent.GetControlFromType(parameter.GetType());
                        }
                    }

                    parameters.AddParameter(parameter,
                        control,
                        name,
                        lcEvent.MethodAttribute.DataProvider);
                }
                var data = new classLCMethodData(lcEvent.Device,
                    lcEvent.Method,
                    lcEvent.MethodAttribute,
                    parameters);
                data.OptimizeWith = lcEvent.OptimizeWith;
                //
                // Construct an event.  We send false as locked because its not a locking event.
                //
                var controlEvent = new controlLCMethodEvent(data, false);
                controlEvent.SetBreakPoint(lcEvent.BreakPoint);
                AddNewEvent(controlEvent);
                controlEvent.updateEventNum(eventIndex(controlEvent) + 1);
            }
            // Then set all the check boxes accordingly.
            if (method.Column < 0)
            {
                mcomboBox_column.SelectedItem = mcomboBox_column.Items[mcomboBox_column.Items.Count - 1];
            }
            else
            {
                var column = classCartConfiguration.Columns[method.Column];
                if (column != null)
                {
                    foreach (var item in mcomboBox_column.Items)
                    {
                        var id = (column.ID + 1).ToString();
                        if (item.ToString() == id)
                        {
                            mcomboBox_column.SelectedItem = item;
                        }
                    }
                }
            }

            mcheckBox_preOverlap.Checked = method.AllowPreOverlap;
            mcheckBox_postOverlap.Checked = method.AllowPostOverlap;
            ResumeLayout();
        }

        /// <summary>
        /// Handles creating an unlock event when the event is locked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="optimize"></param>
        void deviceEvent_Lock(object sender, bool enabled, classLCMethodData method)
        {
            if (enabled)
            {
                var senderEvent = sender as controlLCMethodEvent;
                var newEvent = new controlLCMethodEvent(method, true);
                AddNewEvent(newEvent);
            }
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
            //
            // Find out what events to remove
            //
            var listEvents = GetSelectedEvents();


            //
            // Remove the events from the list
            //
            var indexOfLastDeleted = 0;
            foreach (var deviceEvent in listEvents)
            {
                indexOfLastDeleted = m_events.IndexOf(deviceEvent);
                var data = deviceEvent.SelectedMethod;

                var device = deviceEvent.Device;
                var type = device.GetType();

                var parameter = data.Parameters;

                for (var i = 0; i < parameter.Names.Count; i++)
                {
                    var combo = parameter.Controls[i] as controlParameterComboBox;

                    if (combo != null)
                    {
                        var key = parameter.DataProviderNames[i];

                        if (key != null)
                        {
                            try
                            {
                                device.UnRegisterDataProvider(key, combo.FillData);
                            }
                            catch (Exception ex)
                            {
                                classApplicationLogger.LogError(0,
                                    "There was an error when removing the device event. " + ex.Message, ex);
                            }
                        }
                    }
                }
                for (var i = indexOfLastDeleted; i < m_events.Count; i++)
                {
                    m_events[i].updateEventNum(m_events.IndexOf(m_events[i]));
                }
                mpanel_stage.Controls.Remove(deviceEvent);
                m_events.Remove(deviceEvent);
                classApplicationLogger.LogMessage(0,
                    string.Format("A control event for the device {0} was  removed from method {1}.",
                        deviceEvent.Device.Name,
                        data.Method.Name));
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

            //
            // Construct the pivot table.
            //
            var i = 0;
            foreach (var devEvent in listEvents)
            {
                var index = m_events.IndexOf(devEvent);
                indices[i++] = index;
            }

            //
            // Use the pivot table to move the items around
            //
            var maxPos = 0;

            // Dont worry about checking for the length of indices because we already did that above
            // when we checked the number of selected events being of the right size.
            //
            if (indices[0] == 0)
                maxPos = 1;

            for (var j = 0; j < i; j++)
            {
                if (indices[j] > maxPos)
                {
                    //
                    // Now swap the events
                    //
                    var preEvent = m_events[indices[j]];
                    m_events[indices[j]] = m_events[indices[j] - 1];
                    m_events[indices[j] - 1] = preEvent;
                    //update event list positions
                    m_events[indices[j]].updateEventNum(m_events.IndexOf(m_events[indices[j]]) + 1);
                    preEvent.updateEventNum(m_events.IndexOf(preEvent) + 1);

                    //
                    // Update the pivot index now so that we cannot advance above that.
                    //
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
            //
            // Construct the pivot table.
            //
            var i = 0;
            foreach (var devEvent in listEvents)
            {
                var index = m_events.IndexOf(devEvent);
                indices[i++] = index;
            }

            //
            // Use the pivot table to move the items around
            //
            var maxPos = m_events.Count - 1;
            if (maxPos == indices[indices.Length - 1])
            {
                maxPos--;
            }

            for (var j = i - 1; j >= 0; j--)
            {
                if (indices[j] < maxPos)
                {
                    //
                    // Now swap the events
                    //
                    var preEvent = m_events[indices[j]];
                    m_events[indices[j]] = m_events[indices[j] + 1];
                    m_events[indices[j] + 1] = preEvent;
                    m_events[indices[j]].updateEventNum(m_events.IndexOf(m_events[indices[j]]) + 1);
                    preEvent.updateEventNum(m_events.IndexOf(preEvent) + 1);
                    //
                    // Update the pivot index now so that we cannot advance above that.
                    //
                }
                maxPos = indices[j];
            }

            classApplicationLogger.LogMessage(0, "Control event moved down.");
            RenderEventList();
        }

        private void SelectAll(bool selected)
        {
            foreach (var lcEvent in m_events)
            {
                lcEvent.IsSelected = selected;
                lcEvent.Refresh();
            }
        }

        #endregion

        #region Control Event Handlers

        private void mbutton_selectDeselectAll_Click(object sender, EventArgs e)
        {
            SelectAll(true);
        }

        private void mbutton_deselectAll_Click(object sender, EventArgs e)
        {
            SelectAll(false);
        }

        /// <summary>
        /// Adds a new event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_addEvent_Click(object sender, EventArgs e)
        {
            AddNewEvent();
            OnEventChanged();
        }

        /// <summary>
        /// Removes the checked events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_deleteEvent_Click(object sender, EventArgs e)
        {
            DeleteSelectedEvents();
            OnEventChanged();
        }

        /// <summary>
        /// Moves the LC event down in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_moveDown_Click(object sender, EventArgs e)
        {
            MoveSelectedEventsDown();
            OnEventChanged();
        }

        /// <summary>
        /// Moves the LC event up in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_moveUp_Click(object sender, EventArgs e)
        {
            MoveSelectedEventsUp();
            OnEventChanged();
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

            //
            // Create a new writer.
            //
            var writer = new classLCMethodWriter();

            //
            // Construct the path
            //
            var path = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH),
                classLCMethodFactory.CONST_LC_METHOD_FOLDER);
            path = Path.Combine(path, method.Name + classLCMethodFactory.CONST_LC_METHOD_EXTENSION);

            //
            // Write the method out!
            //
            return writer.WriteMethod(path, method);
        }

        /// <summary>
        /// Saves the current method selected in the method combo box.
        /// </summary>
        public void SaveMethod()
        {
            if (mcomboBox_savedMethods.SelectedIndex >= 0 && mcomboBox_savedMethods.Items.Count > 0)
            {
                var method =
                    FindMethods(Convert.ToString(mcomboBox_savedMethods.Items[mcomboBox_savedMethods.SelectedIndex]));

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
            foreach (var nameObject in mcomboBox_savedMethods.Items)
            {
                var name = Convert.ToString(nameObject);
                var method = FindMethods(name);
                SaveMethod(method);
            }

            mbutton_saveAll.Enabled = false;
            mbutton_save.Enabled = false;
            classApplicationLogger.LogMessage(0, "LC-Methods Saved.");
        }

        /// <summary>
        /// Opens a method.
        /// </summary>
        public void OpenMethod(string path)
        {
            var reader = new classLCMethodReader();
            var errors = new List<Exception>();
            var method = reader.ReadMethod(path, ref errors);

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
    }
}