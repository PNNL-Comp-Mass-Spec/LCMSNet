using System;
using System.Drawing;
using System.Windows.Forms;
using LcmsNet.Method.ViewModels;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method.Forms
{
    public partial class formColumnSampleProgress2 : Form
    {
        private ColumnSampleProgressViewModel columnSampleProgressViewModel;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public formColumnSampleProgress2()
        {
            InitializeComponent();

            columnSampleProgressViewModel = new ColumnSampleProgressViewModel();
            columnSampleProgressView.DataContext = columnSampleProgressViewModel;

            columnSampleProgressViewModel.PreviewAvailable += PreviewAvailableHandler;
        }

        /// <summary>
        /// Updates the progress window with the sample data.
        /// </summary>
        /// <param name="sample"></param>
        public void UpdateSample(classSampleData sample)
        {
            columnSampleProgressViewModel.UpdateSample(sample);
        }

        public void UpdateError(classSampleData sample, classLCEvent lcEvent)
        {
            columnSampleProgressViewModel.UpdateError(sample, lcEvent);
        }

        public event EventHandler<SampleProgressPreviewArgs> PreviewAvailable;

        private void PreviewAvailableHandler(object sender, ViewModels.SampleProgressPreviewArgs args)
        {
            PreviewAvailable?.Invoke(sender, new SampleProgressPreviewArgs(args.PreviewImage.ToImage()));
        }
    }
}