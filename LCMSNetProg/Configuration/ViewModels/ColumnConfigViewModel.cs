using System;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNet.Data;
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
        public ColumnConfigViewModel() : this(new ColumnData(0, color: Colors.Red))
        { }

        public ColumnConfigViewModel(ColumnData column)
        {
            columnData = column;
            columnEnabled = column.Status != ColumnStatus.Disabled;

            // Set all monitors for the column before we set any local monitors, since they get run on initialization
            columnId = this.WhenAnyValue(x => x.ColumnData.ID).ToProperty(this, x => x.ColumnId);
            this.WhenAnyValue(x => x.ColumnData.Status).Subscribe(x =>
            {
                ColumnEnabled = x != ColumnStatus.Disabled;
                LogColumnStatusChange();
            });

            // Local/instance variables should now be initialized according to the target object, so set the monitors going the other way.
            this.WhenAnyValue(x => x.ColumnEnabled).Subscribe(x =>
            {
                if (ColumnData != null)
                {
                    columnData.Status = ColumnEnabled ? ColumnStatus.Idle : ColumnStatus.Disabled;
                }
            });

            allowDisableColumn = this.WhenAnyValue(x => x.ColumnEnabled, x => x.CartConfig.NumberOfColumnsEnabled)
                .Select(x => !x.Item1 || x.Item2 > 1).ToProperty(this, x => x.AllowDisableColumn);
        }

        private CartConfiguration CartConfig => CartConfiguration.Instance;
        private bool columnEnabled;
        private readonly ObservableAsPropertyHelper<bool> allowDisableColumn;

        /// <summary>
        /// Column configuration object.
        /// </summary>
        private ColumnData columnData;

        private readonly ObservableAsPropertyHelper<int> columnId;

        public bool ColumnEnabled
        {
            get => columnEnabled;
            set
            {
                // Don't allow disabling if this is the last column that was still enabled
                if (value || CartConfiguration.NumberOfEnabledColumns > 1)
                {
                    this.RaiseAndSetIfChanged(ref columnEnabled, value);
                }
                else
                {
                    // Trigger UI refresh of the unchanged value.
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool AllowDisableColumn => allowDisableColumn.Value;

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
            ApplicationLogger.LogMessage(LogLevel.Info, statusMessage);
        }
    }
}
