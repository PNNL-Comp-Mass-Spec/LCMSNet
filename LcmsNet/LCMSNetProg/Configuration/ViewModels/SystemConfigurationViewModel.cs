using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using DynamicData;
using LcmsNet.IO.DMS;
using LcmsNet.IO.DMS.Data;
using LcmsNet.IO.SQLite;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetData.Logging;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReactiveUI;

namespace LcmsNet.Configuration.ViewModels
{
    /// <summary>
    /// Displays application and cart configurations.
    /// </summary>
    public class SystemConfigurationViewModel : ReactiveObject
    {
        #region "Constructors"

        /// <summary>
        ///  Default constructor for displaying column data.
        /// </summary>
        public SystemConfigurationViewModel()
        {
#if DEBUG
            // Avoid exceptions caused from not being able to access program settings, when being run to provide design-time data context for the designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Column1ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 1, Color = Colors.Red }, ColumnNameComboBoxOptions);
                Column2ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 2, Color = Colors.Yellow }, ColumnNameComboBoxOptions);
                Column3ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 3, Color = Colors.Green }, ColumnNameComboBoxOptions);
                Column4ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 4, Color = Colors.Orange }, ColumnNameComboBoxOptions);
                return;
            }
#endif

            instrumentComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var instrumentComboBoxOptionsBound).Subscribe();
            cartConfigComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var cartConfigComboBoxOptionsBound).Subscribe();
            separationTypeComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var separationTypeComboBoxOptionsBound).Subscribe();
            operatorsComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var operatorsComboBoxOptionsBound).Subscribe();
            columnNameComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var columnNameComboBoxOptionsBound).Subscribe();
            InstrumentComboBoxOptions = instrumentComboBoxOptionsBound;
            CartConfigComboBoxOptions = cartConfigComboBoxOptionsBound;
            SeparationTypeComboBoxOptions = separationTypeComboBoxOptionsBound;
            OperatorsComboBoxOptions = operatorsComboBoxOptionsBound;
            ColumnNameComboBoxOptions = columnNameComboBoxOptionsBound;

            TriggerLocation = LCMSSettings.GetParameter(LCMSSettings.PARAM_TRIGGERFILEFOLDER);
            PdfPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_PDFPATH);

            Column1ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[0], ColumnNameComboBoxOptions);
            Column2ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[1], ColumnNameComboBoxOptions);
            Column3ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[2], ColumnNameComboBoxOptions);
            Column4ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[3], ColumnNameComboBoxOptions);

            RegisterColumn(Column1ViewModel);
            RegisterColumn(Column2ViewModel);
            RegisterColumn(Column3ViewModel);
            RegisterColumn(Column4ViewModel);
            SpecialColumnEnabled = !LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, true);

            // Cart name
            CartName = CartConfiguration.CartName;

            LoadSeparationTypes();
            LoadInstrumentInformation();
            LoadApplicationSettings();

            MinVolume = CartConfiguration.MinimumVolume;

            //load time zones into combobox
            TimeZoneComboBoxOptions = TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id).ToList().AsReadOnly();
            TimeZone = LCMSSettings.GetParameter(LCMSSettings.PARAM_TIMEZONE);

            LoadUserCombo(SQLiteTools.GetUserList(false));

            ReloadData(true);

            this.WhenAnyValue(x => x.InstrumentOperator).Subscribe(x => this.OperatorNotSaved = true);
            this.WhenAnyValue(x => x.InstrumentName).Subscribe(x => this.InstrumentNameNotSaved = true);
            this.WhenAnyValue(x => x.SpecialColumnEnabled).Subscribe(x => LCMSSettings.SetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, (!x).ToString()));
            OperatorNotSaved = false;
            InstrumentNameNotSaved = false;

            SetInstrumentCommand = ReactiveCommand.Create(() => SaveInstrument(false));
            SetOperatorCommand = ReactiveCommand.Create(() => SaveOperator(false));
            ReloadCartDataCommand = ReactiveCommand.Create(() => ReloadData(false));
            BrowsePdfPathCommand = ReactiveCommand.Create(BrowsePdfPath);
        }

        #endregion

        public event EventHandler ColumnNameChanged;

        #region "Class variables"

        /// <summary>
        /// List of users
        /// </summary>
        private Dictionary<string, UserInfo> dmsUserList;

        private double minVolume = 1;
        private string triggerLocation = "";
        private string pdfPath = "";
        private string cartConfigName = "";
        private string timeZone = "";
        private string instrumentName = "";
        private string separationType = "";
        private string instrumentOperator = "";
        private bool instrumentNameNotSaved;
        private bool operatorNotSaved;
        private readonly SourceList<string> instrumentComboBoxOptions = new SourceList<string>();
        private readonly SourceList<string> cartConfigComboBoxOptions = new SourceList<string>();
        private readonly SourceList<string> separationTypeComboBoxOptions = new SourceList<string>();
        private readonly SourceList<string> operatorsComboBoxOptions = new SourceList<string>();
        private readonly SourceList<string> columnNameComboBoxOptions = new SourceList<string>();
        private bool specialColumnEnabled;

        #endregion

        #region Properties

        public double MinVolume
        {
            get => minVolume;
            set
            {
                var oldValue = minVolume;
                this.RaiseAndSetIfChanged(ref minVolume, value);
                if (!oldValue.Equals(minVolume))
                {
                    CartConfiguration.MinimumVolume = minVolume;
                }
            }
        }

        public string TriggerLocation
        {
            get => triggerLocation;
            set => this.RaiseAndSetIfChanged(ref triggerLocation, value);
        }

        public string PdfPath
        {
            get => pdfPath;
            set => this.RaiseAndSetIfChanged(ref pdfPath, value);
        }

        public string CartName { get; }

        public string CartConfigName
        {
            get => cartConfigName;
            set
            {
                var oldValue = cartConfigName;
                this.RaiseAndSetIfChanged(ref cartConfigName, value);
                if (!oldValue.Equals(cartConfigName))
                {
                    SaveCartConfigName();
                }
            }
        }

        public string TimeZone
        {
            get => timeZone;
            set
            {
                var oldValue = timeZone;
                this.RaiseAndSetIfChanged(ref timeZone, value);
                if (!oldValue.Equals(timeZone))
                {
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_TIMEZONE, timeZone);
                }
            }
        }

        public string InstrumentName
        {
            get => instrumentName;
            set => this.RaiseAndSetIfChanged(ref instrumentName, value);
        }

        public string SeparationType
        {
            get => separationType;
            set {
                var oldValue = separationType;
                this.RaiseAndSetIfChanged(ref separationType, value);
                if (!oldValue.Equals(separationType))
                {
                    SaveSeparationType();
                }
            }
        }

        public string InstrumentOperator
        {
            get => instrumentOperator;
            set => this.RaiseAndSetIfChanged(ref instrumentOperator, value);
        }

        public ColumnConfigViewModel Column1ViewModel { get; }

        public ColumnConfigViewModel Column2ViewModel { get; }

        public ColumnConfigViewModel Column3ViewModel { get; }

        public ColumnConfigViewModel Column4ViewModel { get; }

        public bool SpecialColumnEnabled
        {
            get => specialColumnEnabled;
            set
            {
                // Don't allow disabling if this is the last column that was still enabled
                if (value || CartConfiguration.NumberOfEnabledColumns > 0)
                {
                    this.RaiseAndSetIfChanged(ref specialColumnEnabled, value);
                }
                else
                {
                    // Trigger UI refresh of the unchanged value.
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool InstrumentNameNotSaved
        {
            get => instrumentNameNotSaved;
            private set => this.RaiseAndSetIfChanged(ref instrumentNameNotSaved, value);
        }

        public bool OperatorNotSaved
        {
            get => operatorNotSaved;
            private set => this.RaiseAndSetIfChanged(ref operatorNotSaved, value);
        }

        public ReadOnlyCollection<string> TimeZoneComboBoxOptions { get; }
        public ReadOnlyObservableCollection<string> InstrumentComboBoxOptions { get; }
        public ReadOnlyObservableCollection<string> CartConfigComboBoxOptions { get; }
        public ReadOnlyObservableCollection<string> SeparationTypeComboBoxOptions { get; }
        public ReadOnlyObservableCollection<string> OperatorsComboBoxOptions { get; }
        public ReadOnlyObservableCollection<string> ColumnNameComboBoxOptions { get; }

        public ReactiveCommand<Unit, Unit> SetInstrumentCommand { get; }
        public ReactiveCommand<Unit, Unit> SetOperatorCommand { get; }
        public ReactiveCommand<Unit, Unit> ReloadCartDataCommand { get; }
        public ReactiveCommand<Unit, Unit> BrowsePdfPathCommand { get; }

        #endregion

        #region "Methods"

        private void BrowsePdfPath()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                InitialDirectory = PdfPath,
                Multiselect = false,
                EnsurePathExists = true,
            };

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                PdfPath = dialog.FileName;
                LCMSSettings.SetParameter(LCMSSettings.PARAM_PDFPATH, PdfPath);
            }
        }

        void RegisterColumn(ColumnConfigViewModel column)
        {
            column.ColumnNameChanged += ColumnNameChangedHandler;
        }

        void ColumnNameChangedHandler()
        {
            ColumnNameChanged?.Invoke(this, null);
        }

        private void LoadInstrumentInformation()
        {
            // Load combo box
            var instList = SQLiteTools.GetInstrumentList(false).ToList();

            if (instList.Count < 1)
            {
                ApplicationLogger.LogError(0, "No instruments found.");
                return;
            }

            instrumentComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(instList.Select(x => x.DMSName));
            });

            // Determine if presently specified instrument name is in list. If it is, display it.
            InstrumentName = LCMSSettings.GetParameter(LCMSSettings.PARAM_INSTNAME);

            // Determine if presently specified separation type is in list. If it is, display it.
            SeparationType = LCMSSettings.GetParameter(LCMSSettings.PARAM_SEPARATIONTYPE);

            InstrumentNameNotSaved = false;
        }

        /// <summary>
        /// Loads the application settings to the user interface.
        /// </summary>
        private void LoadApplicationSettings()
        {
            //checkBox_createMethodFolders.Checked = LCMSSettings.GetParameter(LCMSSettings.PARAM_CREATEMETHODFOLDERS, false));
            //checkBox_copyMethodFolders.Checked = LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYMETHODFOLDERS, false));
        }

        /// <summary>
        /// Loads the user combo box
        /// </summary>
        /// <param name="userList"></param>
        private void LoadUserCombo(IEnumerable<UserInfo> userList)
        {
            // Make a dictionary based on the input list, with the first entry = "(None)"
            if (dmsUserList == null)
            {
                dmsUserList = new Dictionary<string, UserInfo>();
            }
            else
            {
                dmsUserList.Clear();
            }

            // Create dummy user. User is not recognized by DMS, so that trigger files will fail and let
            //      operator know that user name was not provided
            var tmpUser = new UserInfo
            {
                PayrollNum = "None",
                UserName = "(None)"
            };
            dmsUserList.Add(tmpUser.PayrollNum, tmpUser);

            // Add users to dictionary from user list
            foreach (var user in userList)
            {
                var data = string.Format("{0} - ({1})", user.UserName,
                    user.PayrollNum);

                dmsUserList.Add(data, user);
            }

            // Now add user list to combo box
            operatorsComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(dmsUserList.Select(x => x.Key));
            });

            // Determine if presently specified operator name is in list. If it is, display it.
            var savedName = LCMSSettings.GetParameter(LCMSSettings.PARAM_OPERATOR);
            InstrumentOperator = savedName;
            foreach (var item in dmsUserList)
            {
                if (item.Value.UserName.Equals(savedName))
                {
                    InstrumentOperator = item.Key;
                    return;
                }
            }
            InstrumentOperator = LCMSSettings.GetParameter(LCMSSettings.PARAM_OPERATOR);

            OperatorNotSaved = false;
        }

        #endregion

        #region Column events

        ///// <summary>
        ///// Handles the event when the column status is changed.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="previousStatus"></param>
        ///// <param name="newStatus"></param>
        //void column_StatusChanged(object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus)
        //{
        //    if (InvokeRequired == true)
        //    {
        //        BeginInvoke(new DelegateUpdateStatus(StatusChanged), new object[] { sender, previousStatus, newStatus });
        //    }
        //    else
        //    {
        //        StatusChanged(sender, previousStatus, newStatus);
        //    }
        //}

        #endregion

        #region Form Events

        private void SaveCartConfigName()
        {
            LCMSSettings.SetParameter(LCMSSettings.PARAM_CARTCONFIGNAME, CartConfigName);
            SQLiteTools.SaveSelectedCartConfigName(LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTCONFIGNAME));
        }

        private void SaveSeparationType()
        {
            LCMSSettings.SetParameter(LCMSSettings.PARAM_SEPARATIONTYPE, SeparationType);
            SQLiteTools.SaveSelectedSeparationType(LCMSSettings.GetParameter(LCMSSettings.PARAM_SEPARATIONTYPE));
        }

        private void ReloadData(bool isLoading = false)
        {
            // Get a fresh list of columns from DMS and store it in the cache db
            try
            {
                var dmsTools = LcmsNet.Configuration.DMSDataContainer.DBTools;
                // Just re-load everything
                dmsTools.LoadCacheFromDMS();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            LoadSeparationTypes();

            UpdateCartConfigNames(isLoading);
            UpdateColumnNameLists(isLoading);

            RestoreCurrentSelections();
        }

        private void RestoreCurrentSelections()
        {
            var cartConfig = LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTCONFIGNAME);
            if (!string.IsNullOrWhiteSpace(cartConfig))
            {
                CartConfigName = cartConfig;
            }

            var sepType = LCMSSettings.GetParameter(LCMSSettings.PARAM_SEPARATIONTYPE);
            if (string.IsNullOrWhiteSpace(sepType))
            {
                SeparationType = "none";
            }
            else
            {
                SeparationType = sepType;
            }
        }

        private void LoadSeparationTypes()
        {
            var separationTypes = SQLiteTools.GetSepTypeList(false);

            separationTypeComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(separationTypes);
            });
        }

        private void UpdateCartConfigNames(bool isLoading)
        {
            List<string> cartConfigNameList = null;
            var fullCount = 0;
            try
            {
                // Get the new cart config names from the cache db
                cartConfigNameList = SQLiteTools.GetCartConfigNameList(true).ToList();
                fullCount = cartConfigNameList.Count;
                cartConfigNameList = SQLiteTools.GetCartConfigNameList(CartConfiguration.CartName, false).ToList();
            }
            catch (DatabaseDataException ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            if (!isLoading)
            {
                // If a valid list was received, update the display
                if (cartConfigNameList == null)
                {
                    // No new cart config names were obtained
                    ApplicationLogger.LogError(0, "Cart config name list null when refreshing list");
                    MessageBox.Show(@"List not updated. Cart config name list from DMS is null");
                    return;
                }

                if (cartConfigNameList.Count < 1)
                {
                    if (fullCount < 1)
                    {
                        // No names found in list
                        ApplicationLogger.LogError(0, "No cart config names found when refreshing list");
                        MessageBox.Show(@"List not updated. No cart config names were found.");
                    }
                    else
                    {
                        // No names in list after cart name filter
                        ApplicationLogger.LogError(0, "No cart config names found when refreshing list - none match the cart name");
                        MessageBox.Show(@"List not updated. No cart config names were found - none match the cart name.");
                    }
                    return;
                }
            }

            cartConfigComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(cartConfigNameList);
            });

            if (!isLoading)
            {
                ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Cart config name lists updated");
            }
        }

        private void UpdateColumnNameLists(bool isLoading)
        {
            List<string> columnList = null;
            try
            {
                // Get the new list of columns from the cache db
                columnList = SQLiteTools.GetColumnList(true).ToList();
            }
            catch (DatabaseDataException ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            if (!isLoading)
            {
                // If a valid list was received, update the display
                if (columnList == null)
                {
                    // No new column list obtained
                    ApplicationLogger.LogError(0, "Column name list null when refreshing list");
                    MessageBox.Show("List not updated. Column name list from DMS is null");
                    return;
                }
                if (columnList.Count < 1)
                {
                    // No names found in list
                    ApplicationLogger.LogError(0, "No column names found when refreshing list");
                    MessageBox.Show("List not updated. No column names found.");
                    return;
                }
            }

            // Everything was OK, so update the list
            columnNameComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(columnList);
            });

            if (!isLoading)
            {
                ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Column name lists updated");
            }
        }

        private void SaveInstrument(bool isLoading)
        {
            if (!isLoading)
                LCMSSettings.SetParameter(LCMSSettings.PARAM_INSTNAME, InstrumentName);
            InstrumentNameNotSaved = false;
        }

        private void SaveOperator(bool isLoading)
        {
            if (!isLoading)
            {
                var operatorName = InstrumentOperator;
                if (dmsUserList.ContainsKey(operatorName))
                {
                    var instOperator = dmsUserList[operatorName];
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_OPERATOR, instOperator.UserName);
                }
                else
                {
                    ApplicationLogger.LogError(0,
                        "Could not use the current user as the operator.  Was not present in the system.");
                }
            }
            OperatorNotSaved = false;
        }

        #endregion
    }
}
