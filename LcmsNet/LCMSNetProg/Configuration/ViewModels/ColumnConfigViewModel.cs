using System;
using System.Windows.Media;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;
using ReactiveUI;

namespace LcmsNet.Configuration.ViewModels
{
    public class ColumnConfigViewModel : ReactiveObject
    {
        #region "Constructors"

        /// <summary>
        /// Default constructor for the column config view control that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ColumnConfigViewModel()
        {
            columnData = new classColumnData
            {
                ID = 0,
                Color = Colors.Red
            };

            columnNamesComboBoxOptions = new ReactiveList<string>(new string[] {"NOTSET"});

            Initialize();
        }

        public ColumnConfigViewModel(classColumnData column, IReadOnlyReactiveList<string> columnNames)
        {
            columnData = column;
            columnNamesComboBoxOptions = columnNames;

            Initialize();
        }

        private void Initialize()
        {
            // Set all monitors for the column before we set any local monitors, since they get run on initialization
            this.WhenAnyValue(x => x.ColumnData.ID).ToProperty(this, x => x.ColumnId, out columnId);
            this.WhenAnyValue(x => x.ColumnData.Status).Subscribe(x =>
            {
                this.ColumnEnabled = x != enumColumnStatus.Disabled;
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
                        columnData.Status = enumColumnStatus.Idle;
                    }
                    else
                    {
                        columnData.Status = enumColumnStatus.Disabled;
                    }
                }
            });
        }

        #endregion

        #region "Class variables"

        private bool columnEnabled = true;

        /// <summary>
        /// Column configuration object.
        /// </summary>
        private classColumnData columnData;

        private ObservableAsPropertyHelper<int> columnId;
        private readonly IReadOnlyReactiveList<string> columnNamesComboBoxOptions;

        #endregion

        #region "Delegates"

        public delegate void delegateColumnNamesChanged();

        #endregion

        #region "Events"

        public event delegateColumnNamesChanged ColumnNameChanged;

        #endregion

        #region Properties

        public bool ColumnEnabled
        {
            get { return columnEnabled; }
            set { this.RaiseAndSetIfChanged(ref columnEnabled, value); }
        }

        public int ColumnId
        {
            get { return columnId?.Value + 1 ?? 0; }
        }

        /// <summary>
        /// Gets or sets the data associated with the column.
        /// </summary>
        public classColumnData ColumnData
        {
            get { return columnData; }
            private set { this.RaiseAndSetIfChanged(ref columnData, value); }
        }

        /// <summary>
        /// Sets the list of column names.
        /// </summary>
        public IReadOnlyReactiveList<string> ColumnNamesComboBoxOptions => columnNamesComboBoxOptions;

        #endregion

        #region Column Data Event Handlers

        /// <summary>
        /// Handles when the status for a column changes.
        /// </summary>
        private void LogColumnStatusChange()
        {
            var statusMessage = string.Format("Status: {0}", ColumnData.Status);
            //TODO: change this magic number into a constant.
            classApplicationLogger.LogMessage(1, statusMessage);
        }

        #endregion
    }
}
