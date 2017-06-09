using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Media;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;
using LcmsNetSQLiteTools;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReactiveUI;

namespace LcmsNet.Configuration.ViewModels
{
    #region "Namespace delegates"

    internal delegate void DelegateUpdateStatus(object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus);

    #endregion

    /// <summary>
    /// Displays application and cart configurations.
    /// </summary>
    public class SystemConfigurationViewModel : ReactiveObject
    {
        private readonly bool mIsLoading;

        #region "Constructors"

        /// <summary>
        ///  Default constructor for displaying column data.
        /// </summary>
        public SystemConfigurationViewModel()
        {
            mIsLoading = true;

            Initialize();

            mIsLoading = false;
        }

        #endregion

        public event EventHandler ColumnNameChanged;

        #region "Class variables"

        /// <summary>
        /// List of users
        /// </summary>
        private Dictionary<string, classUserInfo> dmsUserList;

        private double minVolume = 1;
        private string triggerLocation = "";
        private string pdfPath = "";
        private string cartName = "";
        private string cartConfigName = "";
        private string timeZone = "";
        private string instrumentName = "";
        private string separationType = "";
        private string instrumentOperator = "";
        private ColumnConfigViewModel column1ViewModel;
        private ColumnConfigViewModel column2ViewModel;
        private ColumnConfigViewModel column3ViewModel;
        private ColumnConfigViewModel column4ViewModel;
        private bool instrumentNameNotSaved;
        private bool operatorNotSaved;
        private readonly ReactiveList<string> timeZoneComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<string> instrumentComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<string> cartConfigComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<string> separationTypeComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<string> operatorsComboBoxOptions = new ReactiveList<string>();
        private readonly ReactiveList<string> columnNameComboBoxOptions = new ReactiveList<string>();

        #endregion

        #region Properties

        public double MinVolume
        {
            get { return minVolume; }
            set
            {
                var oldValue = minVolume;
                this.RaiseAndSetIfChanged(ref minVolume, value);
                if (!oldValue.Equals(minVolume))
                {
                    classCartConfiguration.MinimumVolume = minVolume;
                }
            }
        }

        public string TriggerLocation
        {
            get { return triggerLocation; }
            set { this.RaiseAndSetIfChanged(ref triggerLocation, value); }
        }

        public string PdfPath
        {
            get { return pdfPath; }
            set { this.RaiseAndSetIfChanged(ref pdfPath, value); }
        }

        public string CartName
        {
            get { return cartName; }
            set { this.RaiseAndSetIfChanged(ref cartName, value); }
        }

        public string CartConfigName
        {
            get { return cartConfigName; }
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
            get { return timeZone; }
            set
            {
                var oldValue = timeZone;
                this.RaiseAndSetIfChanged(ref timeZone, value);
                if (!oldValue.Equals(timeZone))
                {
                    classLCMSSettings.SetParameter(classLCMSSettings.PARAM_TIMEZONE, timeZone);
                }
            }
        }

        public string InstrumentName
        {
            get { return instrumentName; }
            set { this.RaiseAndSetIfChanged(ref instrumentName, value); }
        }

        public string SeparationType
        {
            get { return separationType; }
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
            get { return instrumentOperator; }
            set { this.RaiseAndSetIfChanged(ref instrumentOperator, value); }
        }

        public ColumnConfigViewModel Column1ViewModel
        {
            get { return column1ViewModel; }
            set { this.RaiseAndSetIfChanged(ref column1ViewModel, value); }
        }

        public ColumnConfigViewModel Column2ViewModel
        {
            get { return column2ViewModel; }
            set { this.RaiseAndSetIfChanged(ref column2ViewModel, value); }
        }

        public ColumnConfigViewModel Column3ViewModel
        {
            get { return column3ViewModel; }
            set { this.RaiseAndSetIfChanged(ref column3ViewModel, value); }
        }

        public ColumnConfigViewModel Column4ViewModel
        {
            get { return column4ViewModel; }
            set { this.RaiseAndSetIfChanged(ref column4ViewModel, value); }
        }

        public bool InstrumentNameNotSaved
        {
            get { return instrumentNameNotSaved; }
            private set { this.RaiseAndSetIfChanged(ref instrumentNameNotSaved, value); }
        }

        public bool OperatorNotSaved
        {
            get { return operatorNotSaved; }
            private set { this.RaiseAndSetIfChanged(ref operatorNotSaved, value); }
        }

        public ReactiveList<string> TimeZoneComboBoxOptions => timeZoneComboBoxOptions;
        public ReactiveList<string> InstrumentComboBoxOptions => instrumentComboBoxOptions;
        public ReactiveList<string> CartConfigComboBoxOptions => cartConfigComboBoxOptions;
        public ReactiveList<string> SeparationTypeComboBoxOptions => separationTypeComboBoxOptions;
        public ReactiveList<string> OperatorsComboBoxOptions => operatorsComboBoxOptions;
        public ReactiveList<string> ColumnNameComboBoxOptions => columnNameComboBoxOptions;

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> SetInstrumentCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SetOperatorCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ReloadCartDataCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> BrowsePdfPathCommand { get; private set; }

        private void SetupCommands()
        {
            SetInstrumentCommand = ReactiveCommand.Create(() => this.SaveInstrument());
            SetOperatorCommand = ReactiveCommand.Create(() => this.SaveOperator());
            ReloadCartDataCommand = ReactiveCommand.Create(() => this.ReloadData());
            BrowsePdfPathCommand = ReactiveCommand.Create(() => this.BrowsePdfPath());
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Initializes data for the controls.
        /// </summary>
        private void Initialize()
        {
#if DEBUG
            // Avoid exceptions caused from not being able to access program settings, when being run to provide design-time data context for the designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Column1ViewModel = new ColumnConfigViewModel(new classColumnData() { ID = 1, ColorWpf = Colors.Red }, ColumnNameComboBoxOptions);
                Column2ViewModel = new ColumnConfigViewModel(new classColumnData() { ID = 2, ColorWpf = Colors.Yellow }, ColumnNameComboBoxOptions);
                Column3ViewModel = new ColumnConfigViewModel(new classColumnData() { ID = 3, ColorWpf = Colors.Green }, ColumnNameComboBoxOptions);
                Column4ViewModel = new ColumnConfigViewModel(new classColumnData() { ID = 4, ColorWpf = Colors.Orange }, ColumnNameComboBoxOptions);

                return;
            }
#endif

            TriggerLocation = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_TRIGGERFILEFOLDER);
            PdfPath = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_PDFPATH);

            Column1ViewModel = new ColumnConfigViewModel(classCartConfiguration.Columns[0], ColumnNameComboBoxOptions);
            Column2ViewModel = new ColumnConfigViewModel(classCartConfiguration.Columns[1], ColumnNameComboBoxOptions);
            Column3ViewModel = new ColumnConfigViewModel(classCartConfiguration.Columns[2], ColumnNameComboBoxOptions);
            Column4ViewModel = new ColumnConfigViewModel(classCartConfiguration.Columns[3], ColumnNameComboBoxOptions);

            RegisterColumn(Column1ViewModel);
            RegisterColumn(Column2ViewModel);
            RegisterColumn(Column3ViewModel);
            RegisterColumn(Column4ViewModel);

            // Cart name
            CartName = classCartConfiguration.CartName;

            LoadSeparationTypes();
            LoadInstrumentInformation();
            LoadApplicationSettings();

            MinVolume = classCartConfiguration.MinimumVolume;

            //load time zones into combobox
            using (TimeZoneComboBoxOptions.SuppressChangeNotifications())
            {
                TimeZoneComboBoxOptions.AddRange(TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id));
            }
            TimeZone = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_TIMEZONE);

            LoadUserCombo(classSQLiteTools.GetUserList(false));

            ReloadData();

            this.WhenAnyValue(x => x.InstrumentOperator).Subscribe(x => this.OperatorNotSaved = true);
            this.WhenAnyValue(x => x.InstrumentName).Subscribe(x => this.InstrumentNameNotSaved = true);
            OperatorNotSaved = false;
            InstrumentNameNotSaved = false;

            SetupCommands();
        }

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
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_PDFPATH, PdfPath);
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
            var instList = classSQLiteTools.GetInstrumentList(false);

            if (instList == null)
            {
                classApplicationLogger.LogError(0, "Instrument list retrieval returned null.");
                return;
            }

            if (instList.Count < 1)
            {
                classApplicationLogger.LogError(0, "No instruments found.");
                return;
            }

            using (InstrumentComboBoxOptions.SuppressChangeNotifications())
            {
                InstrumentComboBoxOptions.Clear();
                InstrumentComboBoxOptions.AddRange(instList.Select(x => x.DMSName));
            }

            // Determine if presently specified instrument name is in list. If it is, display it.
            InstrumentName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_INSTNAME);

            // Determine if presently specified separation type is in list. If it is, display it.
            SeparationType = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE);

            InstrumentNameNotSaved = false;
        }

        /// <summary>
        /// Loads the application settings to the user interface.
        /// </summary>
        private void LoadApplicationSettings()
        {
            //mcheckBox_createTriggerFiles.Checked = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CREATETRIGGERFILES, false));
            //mcheckBox_copyTriggerFiles.Checked = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COPYTRIGGERFILES, false));
            //mcheckBox_createMethodFolders.Checked = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CREATEMETHODFOLDERS, false));
            //mcheckBox_copyMethodFolders.Checked = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COPYMETHODFOLDERS, false));
        }

        /// <summary>
        /// Loads the user combo box
        /// </summary>
        /// <param name="userList"></param>
        private void LoadUserCombo(List<classUserInfo> userList)
        {
            // Make a dictionary based on the input list, with the first entry = "(None)"
            if (dmsUserList == null)
            {
                dmsUserList = new Dictionary<string, classUserInfo>();
            }
            else
            {
                dmsUserList.Clear();
            }

            // Create dummy user. User is not recognized by DMS, so that trigger files will fail and let
            //      operator know that user name was not provided
            var tmpUser = new classUserInfo
            {
                PayrollNum = "None",
                UserName = "(None)"
            };
            dmsUserList.Add(tmpUser.PayrollNum, tmpUser);

            // Add users to dictionary from user list
            foreach (var currUser in userList)
            {
                var data = string.Format("{0} - ({1})", currUser.UserName,
                    currUser.PayrollNum);

                dmsUserList.Add(data, currUser);
            }

            // Now add user list to combo box
            using (OperatorsComboBoxOptions.SuppressChangeNotifications())
            {
                OperatorsComboBoxOptions.Clear();
                OperatorsComboBoxOptions.AddRange(dmsUserList.Select(x => x.Key));
            }

            // Determine if presently specified operator name is in list. If it is, display it.
            var savedName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_OPERATOR);
            InstrumentOperator = savedName;
            foreach (var item in dmsUserList)
            {
                if (item.Value.UserName.Equals(savedName))
                {
                    InstrumentOperator = item.Key;
                    return;
                }
            }
            InstrumentOperator = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_OPERATOR);

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
            classLCMSSettings.SetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME, CartConfigName);
            classSQLiteTools.SaveSelectedCartConfigName(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME));
        }

        private void SaveSeparationType()
        {
            classLCMSSettings.SetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE, SeparationType);
            classSQLiteTools.SaveSelectedSeparationType(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE));
        }

        private void ReloadData()
        {
            // Get a fresh list of columns from DMS and store it in the cache db
            try
            {
                var dmsTools = LcmsNet.Configuration.clsDMSDataContainer.DBTools;
                dmsTools.GetColumnListFromDMS();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            LoadSeparationTypes();

            UpdateCartConfigNames();
            UpdateColumnNameLists();

            RestoreCurrentSelections();
        }

        private void RestoreCurrentSelections()
        {
            var cartConfig = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME);
            if (!string.IsNullOrWhiteSpace(cartConfig))
            {
                CartConfigName = cartConfig;
            }

            var sepType = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE);
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
            var separationTypes = classSQLiteTools.GetSepTypeList(false);

            using (SeparationTypeComboBoxOptions.SuppressChangeNotifications())
            {
                SeparationTypeComboBoxOptions.Clear();
                SeparationTypeComboBoxOptions.AddRange(separationTypes);
            }
        }

        private void UpdateCartConfigNames()
        {
            List<string> cartConfigNameList = null;
            var fullCount = 0;
            try
            {
                // Get the new cart config names from the cache db
                cartConfigNameList = classSQLiteTools.GetCartConfigNameList(true);
                fullCount = cartConfigNameList.Count;
                cartConfigNameList = classSQLiteTools.GetCartConfigNameList(classCartConfiguration.CartName, false);
            }
            catch (classDatabaseDataException ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            if (!mIsLoading)
            {
                // If a valid list was received, update the display
                if (cartConfigNameList == null)
                {
                    // No new cart config names were obtained
                    classApplicationLogger.LogError(0, "Cart config name list null when refreshing list");
                    MessageBox.Show(@"List not updated. Cart config name list from DMS is null");
                    return;
                }

                if (cartConfigNameList.Count < 1)
                {
                    if (fullCount < 1)
                    {
                        // No names found in list
                        classApplicationLogger.LogError(0, "No cart config names found when refreshing list");
                        MessageBox.Show(@"List not updated. No cart config names were found.");
                    }
                    else
                    {
                        // No names in list after cart name filter
                        classApplicationLogger.LogError(0, "No cart config names found when refreshing list - none match the cart name");
                        MessageBox.Show(@"List not updated. No cart config names were found - none match the cart name.");
                    }
                    return;
                }
            }

            using (CartConfigComboBoxOptions.SuppressChangeNotifications())
            {
                CartConfigComboBoxOptions.Clear();
                CartConfigComboBoxOptions.AddRange(cartConfigNameList);
            }

            if (!mIsLoading)
            {
                classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Cart config name lists updated");
            }
        }

        private void UpdateColumnNameLists()
        {
            List<string> columnList = null;
            try
            {
                // Get the new list of columns from the cache db
                columnList = classSQLiteTools.GetColumnList(true);
            }
            catch (classDatabaseDataException ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            if (!mIsLoading)
            {
                // If a valid list was received, update the display
                if (columnList == null)
                {
                    // No new column list obtained
                    classApplicationLogger.LogError(0, "Column name list null when refreshing list");
                    MessageBox.Show("List not updated. Column name list from DMS is null");
                    return;
                }
                if (columnList.Count < 1)
                {
                    // No names found in list
                    classApplicationLogger.LogError(0, "No column names found when refreshing list");
                    MessageBox.Show("List not updated. No column names found.");
                    return;
                }
            }

            // Everything was OK, so update the list
            using (ColumnNameComboBoxOptions.SuppressChangeNotifications())
            {
                ColumnNameComboBoxOptions.Clear();
                ColumnNameComboBoxOptions.AddRange(columnList);
            }

            if (!mIsLoading)
            {
                classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Column name lists updated");
            }
        }

        private void SaveInstrument()
        {
            if (!mIsLoading)
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_INSTNAME, InstrumentName);
            InstrumentNameNotSaved = false;
        }

        private void SaveOperator()
        {
            if (!mIsLoading)
            {
                var operatorName = InstrumentOperator;
                if (dmsUserList.ContainsKey(operatorName))
                {
                    var instOperator = dmsUserList[operatorName];
                    classLCMSSettings.SetParameter(classLCMSSettings.PARAM_OPERATOR, instOperator.UserName);
                }
                else
                {
                    classApplicationLogger.LogError(0,
                        "Could not use the current user as the operator.  Was not present in the system.");
                }
            }
            OperatorNotSaved = false;
        }

        #endregion
    }
}
