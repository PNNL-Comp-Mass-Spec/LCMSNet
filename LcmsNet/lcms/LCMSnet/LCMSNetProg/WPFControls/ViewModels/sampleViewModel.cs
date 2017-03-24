﻿using LcmsNet.SampleQueue.Forms;
using LcmsNetDataClasses;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using LcmsNet.Method;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSQLiteTools;

namespace LcmsNet.WPFControls.ViewModels
{
    public class sampleViewModel : ReactiveObject
    {
        /// <summary>
        /// Index Offset for going from a zero based array for configuration data to the
        /// user-readable column display.
        /// </summary>
        protected const int CONST_COLUMN_INDEX_OFFSET = 1;

        public static ReactiveList<string> LcMethodOptionsStr { get; private set; }
        public static ReactiveList<classLCMethod> LcMethodOptions { get; private set; }
        public static ReactiveList<string> DatasetTypeOptions { get; private set; }
        public static ReactiveList<string> PalTrayOptions { get; private set; }

        static sampleViewModel()
        {
            LcMethodOptionsStr = new ReactiveList<string>();
            LcMethodOptions = new ReactiveList<classLCMethod>();
            DatasetTypeOptions = new ReactiveList<string>();
            PalTrayOptions = new ReactiveList<string>();

            //
            // Add the dataset type items to the data grid
            //
            try
            {
                var datasetTypes = classSQLiteTools.GetDatasetTypeList(false);
                DatasetTypeOptions.Clear();
                foreach (var datasetType in datasetTypes)
                {
                    DatasetTypeOptions.Add(datasetType);
                }
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(1, "The sample queue could not load the dataset type list.", ex);
            }
        }

        public classSampleData Sample { get; private set; }

        public sampleViewModel(classSampleData sample)
        {
            Sample = sample;
        }

        public ReactiveList<string> LcMethodComboBoxOptionsStr => LcMethodOptionsStr;
        public ReactiveList<classLCMethod> LcMethodComboBoxOptions => LcMethodOptions;
        public ReactiveList<string> DatasetTypeComboBoxOptions => DatasetTypeOptions;
        public ReactiveList<string> PalTrayComboBoxOptions => PalTrayOptions;

        public SolidColorBrush RowBackColor
        {
            get { return rowBackColor; }
            set { this.RaiseAndSetIfChanged(ref rowBackColor, value); }
        }

        public SolidColorBrush RowForeColor
        {
            get { return rowForeColor; }
            set { this.RaiseAndSetIfChanged(ref rowForeColor, value); }
        }

        public SolidColorBrush RowSelectionBackColor
        {
            get { return rowSelectionBackColor; }
            set { this.RaiseAndSetIfChanged(ref rowSelectionBackColor, value); }
        }

        public SolidColorBrush RowSelectionForeColor
        {
            get { return rowSelectionForeColor; }
            set { this.RaiseAndSetIfChanged(ref rowSelectionForeColor, value); }
        }

        private SolidColorBrush rowBackColor;
        private SolidColorBrush rowForeColor;
        private SolidColorBrush rowSelectionBackColor;
        private SolidColorBrush rowSelectionForeColor;

        private void SetRowColors()
        {
            switch (Sample.RunningStatus)
            {
                case enumSampleRunningStatus.Running:
                    RowBackColor = Brushes.Lime;
                    RowForeColor = Brushes.Black;
                    RowSelectionBackColor = RowBackColor;
                    RowSelectionForeColor = RowForeColor;
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    RowForeColor = Brushes.Black;
                    RowBackColor = Brushes.Yellow;
                    RowSelectionBackColor = RowBackColor;
                    RowSelectionForeColor = RowForeColor;
                    break;
                case enumSampleRunningStatus.Error:
                    if (Sample.DmsData.Block > 0)
                    {
                        RowBackColor = Brushes.Orange;
                        RowForeColor = Brushes.Black;
                    }
                    else
                    {
                        RowBackColor = Brushes.DarkRed;
                        RowForeColor = Brushes.White;
                    }
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
                case enumSampleRunningStatus.Stopped:
                    if (Sample.DmsData.Block > 0)
                    {
                        RowBackColor = Brushes.SeaGreen;
                        RowForeColor = Brushes.White;
                    }
                    else
                    {
                        RowBackColor = Brushes.Tomato;
                        RowForeColor = Brushes.Black;
                    }
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
                case enumSampleRunningStatus.Complete:
                    RowBackColor = Brushes.DarkGreen;
                    RowForeColor = Brushes.White;
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
                case enumSampleRunningStatus.Queued:
                    goto default;
                default:
                    RowBackColor = Brushes.White;
                    RowForeColor = Brushes.Black;
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
            }

            var status = Sample.ColumnData.Status;
            if (status == enumColumnStatus.Disabled)
            {
                RowBackColor = new SolidColorBrush(Color.FromArgb(RowBackColor.Color.A, (byte)Math.Max(0, RowBackColor.Color.R - 128),
                    (byte)Math.Max(0, RowBackColor.Color.G - 128),
                    (byte)Math.Max(0, RowBackColor.Color.B - 128)));
                RowForeColor = Brushes.LightGray;
            }
        }

        public bool IsChecked
        {
            get { return Checkbox == controlSampleView.enumCheckboxStatus.Checked; } 
            set { }
        }

        public controlSampleView.enumCheckboxStatus Checkbox
        {
            get
            {
                var state = GetCheckboxStatusFromSampleStatus();
                if (state == controlSampleView.enumCheckboxStatus.Disabled)
                {
                    return controlSampleView.enumCheckboxStatus.Checked;
                }
                return state;
            }
        }

        public bool CheckboxDisabled
        {
            get
            {
                var state = GetCheckboxStatusFromSampleStatus();
                return state == controlSampleView.enumCheckboxStatus.Disabled;
            }
        }

        public string SpecialColumnNumber { get; set; }

        public string ColumnNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SpecialColumnNumber))
                {
                    return SpecialColumnNumber;
                }
                return (Sample.ColumnData.ID + CONST_COLUMN_INDEX_OFFSET).ToString();
            }
        }

        public SolidColorBrush ColumnNumberBgColor
        {
            get
            {
                var color = Sample.ColumnData.Color;
                return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            }
        }

        public long SequenceNumber => Sample.SequenceID;

        /// <summary>
        /// Returns the status message pertaining to a given samples running status.
        /// </summary>
        /// <returns>String representing the status of the running state.</returns>
        protected controlSampleView.enumCheckboxStatus GetCheckboxStatusFromSampleStatus()
        {
            var status = controlSampleView.enumCheckboxStatus.Disabled;
            switch (Sample.RunningStatus)
            {
                case enumSampleRunningStatus.Complete:
                    status = controlSampleView.enumCheckboxStatus.Disabled;
                    break;
                case enumSampleRunningStatus.Error:
                    status = controlSampleView.enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Stopped:
                    status = controlSampleView.enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Queued:
                    status = controlSampleView.enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Running:
                    status = controlSampleView.enumCheckboxStatus.Disabled;
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    status = controlSampleView.enumCheckboxStatus.Checked;
                    break;
                default:
                    //
                    // Should never get here
                    //
                    break;
            }

            return status;
        }

        public string Status
        {
            get
            {
                var statusMessage = "";
                SetRowColors();
                switch (Sample.RunningStatus)
                {
                    case enumSampleRunningStatus.Complete:
                        statusMessage = "Complete";
                        break;
                    case enumSampleRunningStatus.Error:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage = "Block Error";
                        }
                        else
                        {
                            statusMessage = "Error";
                        }
                        break;
                    case enumSampleRunningStatus.Stopped:
                        statusMessage = "Stopped";
                        break;
                    case enumSampleRunningStatus.Queued:
                        statusMessage = "Queued";
                        break;
                    case enumSampleRunningStatus.Running:
                        statusMessage = "Running";
                        break;
                    case enumSampleRunningStatus.WaitingToRun:
                        statusMessage = "Waiting";
                        break;
                    default:
                        //
                        // Should never get here
                        //
                        break;
                }

                return statusMessage;
            }
        }

        // controlSampleView.Status.ToolTipText
        public string StatusToolTipText
        {
            get
            {
                var statusMessage = "";
                switch (Sample.RunningStatus)
                {
                    case enumSampleRunningStatus.Complete:
                        statusMessage = "The sample ran successfully.";
                        break;
                    case enumSampleRunningStatus.Error:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage =
                                "There was an error and this sample was part of a block.  You should re-run the block of samples";
                        }
                        else
                        {
                            statusMessage = "An error occured while running this sample.";
                        }
                        break;
                    case enumSampleRunningStatus.Stopped:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage =
                                "The sample was stopped but was part of a block.  You should re-run the block of samples";
                        }
                        else
                        {
                            statusMessage = "The sample execution was stopped.";
                        }
                        break;
                    case enumSampleRunningStatus.Queued:
                        statusMessage = "The sample is queued but not scheduled to run.";
                        break;
                    case enumSampleRunningStatus.Running:
                        statusMessage = "The sample is running.";
                        break;
                    case enumSampleRunningStatus.WaitingToRun:
                        statusMessage = "The sample is scheduled to run and waiting.";
                        break;
                    default:
                        //
                        // Should never get here
                        //
                        break;
                }

                return statusMessage;
            }
        }

        // controlSampleView.mcolumn_blockNumber
        public int BlockNumber => Sample.DmsData.Block;

        // controlSampleView.mcolumn_runOrder
        public int RunOrder => Sample.DmsData.RunOrder;

        // controlSampleView.mcolumn_requestName
        public string RequestName
        {
            get { return Sample.DmsData.RequestName; }
            set
            {
                if (Sample.DmsData.RequestName != value)
                {
                    Sample.DmsData.RequestName = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        // controlSampleView.mcolumn_PalTray
        public string PALTray
        {
            get { return Sample.PAL.PALTray; }
            set
            {
                if (Sample.PAL.PALTray != value)
                {
                    Sample.PAL.PALTray = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        // controlSampleView.mcolumn_palVial
        public int PALVial
        {
            get { return Sample.PAL.Well; }
            set
            {
                if (Sample.PAL.Well != value)
                {
                    if (controlSampleView2.CONST_MIN_WELLPLATE <= value && value <= controlSampleView2.CONST_MAX_WELLPLATE)
                    {
                        Sample.PAL.Well = value;
                    }
                    this.RaisePropertyChanged();
                }
            }
        }

        // controlSampleView.mcolumn_PALVolume
        public double PALVolume
        {
            get { return Sample.Volume; }
            set
            {
                if (!Sample.Volume.Equals(value))
                {
                    if (value >= controlSampleView2.CONST_MIN_VOLUME)
                    {
                        Sample.Volume = value;
                    }
                    this.RaisePropertyChanged();
                }
            }
        }

        // controlSampleView.mcolumn_LCMethod,
        //public string LCMethodStr
        //{
        //    get
        //    {
        //        if (Sample.LCMethod != null)
        //        {
        //            return Sample.LCMethod.Name;
        //        }
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        //if (value.Equals(controlSampleView.CONST_NOT_SELECTED))
        //        //{
        //        //    // Don't change anything, just revert to the previous value
        //        //    this.RaisePropertyChanged();
        //        //    return;
        //        //}
        //        if (Sample.LCMethod != null && Sample.LCMethod.Name != value)
        //        {
        //            Sample.LCMethod.Name = value;
        //            this.RaisePropertyChanged();
        //        }
        //    }
        //}

        // controlSampleView.mcolumn_LCMethod,
        public classLCMethod LCMethod
        {
            get
            {
                return Sample.LCMethod;
            }
            set
            {
                //if (value.ToString().Equals(controlSampleView.CONST_NOT_SELECTED))
                //{
                //    // Don't change anything, just revert to the previous value
                //    this.RaisePropertyChanged();
                //    return;
                //}
                if (!Sample.LCMethod.Equals(value))
                {
                    if (value.Column != Sample.ColumnData.ID)
                    {
                        if (value.Column >= 0)
                        {
                            Sample.ColumnData = classCartConfiguration.Columns[value.Column];
                        }
                    }

                    Sample.LCMethod = value;
                    this.RaisePropertyChanged(nameof(ColumnNumber));
                    this.RaisePropertyChanged(nameof(ColumnNumberBgColor));
                    this.RaisePropertyChanged();
                }
            }
        }

        // controlSampleView.mcolumn_instrumentMethod
        public string InstrumentMethod
        {
            get { return Sample.InstrumentData.MethodName; }
            set
            {
                if (Sample.InstrumentData.MethodName != value)
                {
                    Sample.InstrumentData.MethodName = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        // controlSampleView.mcolumn_datasetType
        public string DatasetType
        {
            get { return Sample.DmsData.DatasetType; }
            set
            {
                if (Sample.DmsData.DatasetType != value)
                {
                    Sample.DmsData.DatasetType = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        // controlSampleView.mcolumn_batchID
        public int BatchID => Sample.DmsData.Batch;
    }
}
