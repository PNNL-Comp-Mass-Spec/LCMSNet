﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Media;
using LcmsNetSDK;
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
                Column1ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 1, Color = Colors.Red });
                Column2ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 2, Color = Colors.Yellow });
                Column3ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 3, Color = Colors.Green });
                Column4ViewModel = new ColumnConfigViewModel(new ColumnData { ID = 4, Color = Colors.Orange });
                return;
            }
#endif

            TriggerLocation = LCMSSettings.GetParameter(LCMSSettings.PARAM_TRIGGERFILEFOLDER);
            PdfPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_PDFPATH);

            Column1ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[0]);
            Column2ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[1]);
            Column3ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[2]);
            Column4ViewModel = new ColumnConfigViewModel(CartConfiguration.Columns[3]);

            RegisterColumn(Column1ViewModel);
            RegisterColumn(Column2ViewModel);
            RegisterColumn(Column3ViewModel);
            RegisterColumn(Column4ViewModel);

            SpecialColumnEnabled = !LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, true);

            // Cart name
            CartName = CartConfiguration.CartName;

            LoadApplicationSettings();

            MinVolume = CartConfiguration.MinimumVolume;

            //load time zones into combobox
            TimeZoneComboBoxOptions = TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id).ToList().AsReadOnly();
            TimeZone = LCMSSettings.GetParameter(LCMSSettings.PARAM_TIMEZONE);

            this.WhenAnyValue(x => x.SpecialColumnEnabled).Subscribe(x => LCMSSettings.SetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, (!x).ToString()));

            BrowsePdfPathCommand = ReactiveCommand.Create(BrowsePdfPath);
        }

        #endregion

        public event EventHandler ColumnNameChanged;

        #region "Class variables"

        private double minVolume = 1;
        private string triggerLocation = "";
        private string pdfPath = "";
        private string timeZone = "";
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

        public ReadOnlyCollection<string> TimeZoneComboBoxOptions { get; }
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

        /// <summary>
        /// Loads the application settings to the user interface.
        /// </summary>
        private void LoadApplicationSettings()
        {
            //checkBox_createMethodFolders.Checked = LCMSSettings.GetParameter(LCMSSettings.PARAM_CREATEMETHODFOLDERS, false));
            //checkBox_copyMethodFolders.Checked = LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYMETHODFOLDERS, false));
        }

        #endregion

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
    }
}