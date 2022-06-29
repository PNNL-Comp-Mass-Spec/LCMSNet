using System;
using System.Windows.Media;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNet.Configuration.ViewModels
{
    public class ColumnConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the column config view control that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ColumnConfigViewModel() : this(new ColumnData { ID = 0, Color = Colors.Red })
        { }

        public ColumnConfigViewModel(ColumnData column)
        {
            columnData = column;

            // Set all monitors for the column before we set any local monitors, since they get run on initialization
            columnId = this.WhenAnyValue(x => x.ColumnData.ID).ToProperty(this, x => x.ColumnId);
            this.WhenAnyValue(x => x.ColumnData.Status).Subscribe(x =>
            {
                this.ColumnEnabled = x != ColumnStatus.Disabled;
                LogColumnStatusChange();
            });
            this.WhenAnyValue(x => x.ColumnData.Name).Subscribe(x => this.ColumnNameChanged?.Invoke());

            // Local/instance variables should now be initialized according to the target object, so set the monitors going the other way.
            this.WhenAnyValue(x => x.ColumnEnabled).Subscribe(x =>
            {
                if (ColumnData != null)
                {
                    if (ColumnEnabled)
                    {
                        columnData.Status = ColumnStatus.Idle;
                    }
                    else
                    {
                        // Don't allow disabling if this is the last column that was still enabled
                        if (CartConfiguration.NumberOfEnabledColumns == 1 &&
                            LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLEDSPECIAL, true))
                        {
                            ColumnEnabled = true;
                        }
                        else
                        {
                            columnData.Status = ColumnStatus.Disabled;
                        }
                    }
                }
            });
        }

        private bool columnEnabled = true;

        /// <summary>
        /// Column configuration object.
        /// </summary>
        private ColumnData columnData;

        private readonly ObservableAsPropertyHelper<int> columnId;

        public delegate void delegateColumnNamesChanged();

        public event delegateColumnNamesChanged ColumnNameChanged;

        public bool ColumnEnabled
        {
            get => columnEnabled;
            set => this.RaiseAndSetIfChanged(ref columnEnabled, value);
        }

        public int ColumnId => columnId?.Value + 1 ?? 0;

        /// <summary>
        /// Gets or sets the data associated with the column.
        /// </summary>
        public ColumnData ColumnData
        {
            get => columnData;
            private set => this.RaiseAndSetIfChanged(ref columnData, value);
        }

        /// <summary>
        /// Handles when the status for a column changes.
        /// </summary>
        private void LogColumnStatusChange()
        {
            var statusMessage = string.Format("Status: {0}", ColumnData.Status);
            //TODO: change this magic number into a constant.
            ApplicationLogger.LogMessage(1, statusMessage);
        }
    }
}
