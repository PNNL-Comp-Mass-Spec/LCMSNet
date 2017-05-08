using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using LcmsNet.SampleQueue;
using LcmsNet.SampleQueue.Forms;
using LcmsNet.WPFControls.Views;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using ReactiveUI;

namespace LcmsNet.WPFControls.ViewModels
{
    public class SampleColumnControlViewModel : SampleControlViewModel
    {
        public override ReactiveList<SampleViewModel> Samples => FilteredSamples;

        public ReactiveList<SampleViewModel> FilteredSamples { get; private set; }

        // Local "wrapper" around the static class options, for data binding purposes
        public ReactiveList<classLCMethod> LcMethodComboBoxOptions => SampleQueueComboBoxOptions.LcMethodOptions;

        private readonly ObservableAsPropertyHelper<string> columnHeader;

        public string ColumnHeader => this.columnHeader?.Value ?? string.Empty;

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleColumnControlViewModel() : base()
        {
            FilteredSamples = new ReactiveList<SampleViewModel>();
            CheckboxColumnVisible = false;
            StatusColumnVisible = false;
            ColumnIdColumnVisible = false;
            DatasetTypeColumnVisible = false;
            LcMethodColumnVisible = false;
            InstrumentMethodColumnVisible = false;
            PalTrayColumnVisible = false;
            PalVialColumnVisible = false;
            VolumeColumnVisible = false;

            this.WhenAnyValue(x => x.Column, x => x.Column.ID, x => x.Column.Name)
                .Select(x => $"Column: (# {x.Item1.ID}) {x.Item1.Name}")
                .ToProperty(this, x => x.ColumnHeader, out this.columnHeader, "Column: NOT SET");

            Column = new classColumnData() {ID = -1, Name = "DevColumn"};
        }

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public SampleColumnControlViewModel(formDMSView dmsView, SampleDataManager sampleDataManager) : base(dmsView, sampleDataManager)
        {
            FilteredSamples = new ReactiveList<SampleViewModel>();
            BindingOperations.EnableCollectionSynchronization(FilteredSamples, this);
            ResetFilteredSamples();

            this.WhenAnyValue(x => x.Column).Throttle(TimeSpan.FromSeconds(0.25)).Subscribe(x => ResetFilteredSamples());

            SampleDataManager.Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.Sample.LCMethod)))
                .Throttle(TimeSpan.FromSeconds(0.25))
                .Subscribe(x => ResetFilteredSamples());

            this.WhenAnyValue(x => x.Column, x => x.Column.ID, x => x.Column.Name)
                .Select(x => $"Column: (# {x.Item1.ID}) {x.Item1.Name}")
                .ToProperty(this, x => x.ColumnHeader, out this.columnHeader, "Column: NOT SET");

            this.WhenAnyValue(x => x.Column.Status).Subscribe(x => this.SetColumnStatus());

            SampleDataManager.Samples.Changed.Throttle(TimeSpan.FromSeconds(0.25)).Subscribe(x => ResetFilteredSamples());

            CheckboxColumnVisible = false;
            StatusColumnVisible = false;
            ColumnIdColumnVisible = false;
            DatasetTypeColumnVisible = false;
            LcMethodColumnVisible = false;
            InstrumentMethodColumnVisible = false;
            PalTrayColumnVisible = false;
            PalVialColumnVisible = false;
            VolumeColumnVisible = false;
            CartConfigColumnVisible = false;

            m_columnData = new classColumnData
            {
                ID = -1
            };

            SetupCommands();
        }

        /// <summary>
        /// Number of columns on the left of the DataGrid that cannot be scrolled; columns in this group cannot be hidden
        /// Set to 0 to disable column freezing
        /// </summary>
        public override int NumFrozenColumns
        {
            get { return 0; }
        }

        private void ResetFilteredSamples()
        {
            using (FilteredSamples.SuppressChangeNotifications())
            {
                FilteredSamples.Clear();
                if (Column == null)
                {
                    FilteredSamples.AddRange(SampleDataManager.Samples);
                    return;
                }
                FilteredSamples.AddRange(SampleDataManager.Samples.Where(x => Column.Equals(x.Sample.ColumnData)));
            }
        }

        formExpansion m_expand;

        private List<Button> m_buttons;

        /// <summary>
        /// Moves the selected samples to another column selected through a dialog window.
        /// </summary>
        protected void MoveSamplesToColumn(enumColumnDataHandling handling)
        {
            var selector = new MoveToColumnSelectorViewModel();
            var selectorWindow = new MoveToColumnSelectorWindow() {DataContext = selector};

            var result = selectorWindow.ShowDialog();

            if (result.HasValue && result.Value &&
                selector.SelectedColumn != MoveToColumnSelectorViewModel.CONST_NO_COLUMN_SELECTED)
            {
                var column = selector.SelectedColumn;
                var selectedSamples = GetSelectedSamples();

                if (selectedSamples.Count < 1)
                    return;

                //
                // Make sure the samples can actually run, e.g. don't put a sample on column 2 already back onto column 2.
                // Don't put a column that has been run, at the end of the queue again.
                //
                var samples = new List<classSampleData>();
                foreach (var sample in selectedSamples)
                {
                    if (sample.RunningStatus == enumSampleRunningStatus.Queued && column != sample.ColumnData.ID)
                    {
                        samples.Add(sample);
                    }
                }

                using (Samples.SuppressChangeNotifications())
                {
                    //
                    // Get the list of unique id's from the samples and
                    // change the column to put the samples on.
                    //

                    // Could keep track of updated IDs with
                    // var ids = new List<long>();

                    foreach (var sample in samples)
                    {
                        // ids.Add(sample.UniqueID);
                        sample.ColumnData = classCartConfiguration.Columns[column];
                    }

                    // TODO: The below code was what would do the moving into unused samples, long disabled Should it be deleted?.
                    /*
                    // Then remove them from the queue
                    enumColumnDataHandling backFill = enumColumnDataHandling.CreateUnused;
                    if (selector.InsertIntoUnused)
                    {
                        backFill = enumColumnDataHandling.LeaveAlone;
                    }

                    SampleDataManager.SampleQueue.RemoveSample(ids, backFill);

                    // Then re-queue the samples.
                    try
                    {
                        if (selector.InsertIntoUnused)
                        {
                            SampleDataManager.SampleQueue.InsertIntoUnusedSamples(samples, handling);
                        }
                        else
                        {
                            //SampleDataManager.UpdateSamples(
                            SampleDataManager.SampleQueue.QueueSamples(samples, handling);
                        }
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(0, "Could not queue the samples when moving between columns.", ex);
                    }
                    if (samples.Count > 0)
                    {
                        SampleDataManager.SampleQueue.UpdateSamples(samples);
                    }
                    */
                }

                // Re-select the first sample
                SelectedSample = Samples.First(x => x.Sample.Equals(selectedSamples.First()));
            }
        }

        #region Members

        /// <summary>
        /// Data object reference to synchronize column data with.
        /// </summary>
        private classColumnData m_columnData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the column number id for this control.
        /// </summary>
        public classColumnData Column
        {
            get { return m_columnData; }
            set { this.RaiseAndSetIfChanged(ref m_columnData, value); }
        }

        #endregion

        public ReactiveCommand AddBlankAppendCommand { get; protected set; }
        public ReactiveCommand MoveToColumnCommand { get; protected set; }

        protected override void SetupCommands()
        {
            base.SetupCommands();
            AddBlankCommand = ReactiveCommand.Create(() => this.AddNewSample(true));
            AddBlankAppendCommand = ReactiveCommand.Create(() => this.AddNewSample(false));
            RemoveSelectedCommand = ReactiveCommand.Create(() => this.SampleDataManager.RemoveUnusedSamples(Column, enumColumnDataHandling.CreateUnused));
            MoveDownCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(1, enumMoveSampleType.Column));
            MoveUpCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(-1, enumMoveSampleType.Column));
            MoveToColumnCommand = ReactiveCommand.Create(() => this.MoveSamplesToColumn(enumColumnDataHandling.CreateUnused));
        }

        private void ShowExpansion()
        {
            //TODO: var buttonScreen = PointToScreen(mbutton_expand.Location);
            //TODO: var controlScreen = PointToScreen(mpanel_control.Location);
            //TODO: m_expand.StartPosition = FormStartPosition.Manual;
            //TODO: m_expand.Location = new Point(buttonScreen.X, controlScreen.Y + mbutton_expand.Top);
            //TODO: m_expand.Refresh();
            //TODO: m_expand.UpdateButtons(m_expansionList);
            //TODO: m_expand.Height = mpanel_control.Height;
            //TODO: var width = Screen.PrimaryScreen.WorkingArea.Width;
            //TODO:
            //TODO: var expandWidth = m_expand.Left + m_expand.Width;
            //TODO:
            //TODO: if (expandWidth > width)
            //TODO: {
            //TODO:     var p = m_expand.Location;
            //TODO:     p.X = width - m_expand.Width;
            //TODO:     m_expand.Location = p;
            //TODO: }
            //TODO:
            //TODO:
            //TODO: m_expand.ShowDialog();
            //TODO: m_expand.Hide();
        }

        private void mbutton_expand_Click(object sender, EventArgs e)
        {
            ShowExpansion();
        }

        private void mbutton_expand_MouseHover(object sender, EventArgs e)
        {
            ShowExpansion();
        }

        #region Constructors

        private const int CONST_BUTTON_PADDING = 2;
        private List<Button> m_expansionList;

        void controlColumnView_Resize(object sender, EventArgs e)
        {
            UpdateExpandButtonList();
        }

        private void UpdateExpandButtonList()
        {
            // Ideas:
            // Use an expander panel (test this)
            // Abstract the buttons out, always show the same set but have them operate on the last selected/focused view model...

            //TODO: m_expansionList.Clear();
            //TODO:
            //TODO: var width = 60;
            //TODO: var leftmost = Width - mbutton_expand.Width - CONST_BUTTON_PADDING;
            //TODO: var padding = CONST_BUTTON_PADDING;
            //TODO: var left = padding;
            //TODO: foreach (var button in m_buttons)
            //TODO: {
            //TODO:     var widthLeft = left + width + CONST_BUTTON_PADDING;
            //TODO:
            //TODO:     if (widthLeft >= leftmost)
            //TODO:     {
            //TODO:         button.Width = width;
            //TODO:         m_expansionList.Add(button);
            //TODO:         if (mpanel_control.Controls.Contains(button))
            //TODO:         {
            //TODO:             mpanel_control.Controls.Remove(button);
            //TODO:         }
            //TODO:     }
            //TODO:     else
            //TODO:     {
            //TODO:         if (!mpanel_control.Controls.Contains(button))
            //TODO:         {
            //TODO:             mpanel_control.Controls.Add(button);
            //TODO:         }
            //TODO:         button.Top = mbutton_expand.Top;
            //TODO:         button.Height = mbutton_expand.Height;
            //TODO:         button.Left = left;
            //TODO:         button.Width = width;
            //TODO:     }
            //TODO:     left += (width + padding);
            //TODO: }
            //TODO: PerformLayout();
        }

        private void InitializeButtons()
        {
            m_expand = new formExpansion();
            m_buttons = new List<Button>();
            m_expansionList = new List<Button>();
            //TODO: m_buttons.Add(mbutton_addBlank);
            //TODO: m_buttons.Add(mbutton_addBlankAppend);
            //TODO: m_buttons.Add(mbutton_addDMS);
            //TODO: m_buttons.Add(mbutton_removeSelected);
            //TODO: m_buttons.Add(mbutton_deleteUnused);
            //TODO: m_buttons.Add(mbutton_up);
            //TODO: m_buttons.Add(mbutton_down);
            //TODO: m_buttons.Add(mbutton_moveColumns);
            //TODO: m_buttons.Add(mbutton_fillDown);
            //TODO: m_buttons.Add(mbutton_trayVial);
            //TODO: m_buttons.Add(mbutton_randomize);
            //TODO: m_buttons.Add(mbutton_cartColumnDate);
            //TODO: m_buttons.Add(mbutton_dmsEdit);

            UpdateExpandButtonList();
        }

        #endregion

        #region Column Event Handlers and Methods

        /// <summary>
        /// Sets the user controls status
        /// </summary>
        private void SetColumnStatus()
        {
            //
            // Status updates
            //
            if (m_columnData.Status == enumColumnStatus.Disabled)
            {
                IsViewEnabled = false;
                BackColor = Brushes.DarkGray;
            }
            else
            {
                IsViewEnabled = true;
                BackColor = Brushes.White;
            }
        }

        #endregion

        #region Sample Queue Management Interface Methods

        /// <summary>
        /// Adds the specified samples ot this column.
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="insertIntoUnused"></param>
        /// <returns></returns>
        protected override void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
        {
            //
            // For every sample, add the column data to it, then add it into the manager.
            // We don't add to our list first so the manager can verify the sample and
            // make sure we don't have duplicates.
            //
            foreach (var sample in samples)
                sample.ColumnData = m_columnData;

            base.AddSamplesToManager(samples, insertIntoUnused);
        }

        /// <summary>
        /// Moves the selected samples by the offset.  First calculates how far to move the samples on this
        /// column by finding out how many columns are enabled in the configuration.
        /// </summary>
        /// <param name="offset">Amount to move the samples (-1 for lower sequence numbers) (1 for higher sequence numbers)</param>
        /// <param name="moveType"></param>
        protected override void MoveSelectedSamples(int offset, enumMoveSampleType moveType)
        {
            var numEnabledColumns = classCartConfiguration.NumberOfEnabledColumns;

            //
            // We are moving the sample by N in the queue to offset for the enabled / disabled columns.
            //
            offset *= numEnabledColumns;
            base.MoveSelectedSamples(offset, enumMoveSampleType.Column);
        }

        /// <summary>
        /// Handles removing unused samples from the sample queue only for this column.
        /// </summary>
        protected override void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            SampleDataManager.RemoveUnusedSamples(m_columnData, resortColumns);
        }

        /// <summary>
        /// Returns whether the sample queue has any unused samples on this column.
        /// </summary>
        /// <returns>True if an unused sample exists.</returns>
        protected override bool HasUnusedSamples()
        {
            return SampleDataManager.HasUnusedSamples(Column);
        }

        #endregion
    }
}
