using System;
using System.Windows.Forms;
using LcmsNetDataClasses;

namespace LcmsNet.Method.Forms
{
    public partial class formColumnSampleProgress : Form
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public formColumnSampleProgress()
        {
            InitializeComponent();

            mcontrol_sampleProgress.RenderAllEvents = false;
            mcontrol_sampleProgress.RenderCurrent = true;

            mcontrol_sampleProgressFull.RenderAllEvents = true;
            mcontrol_sampleProgressFull.RenderCurrent = false;
            mcontrol_sampleProgressFull.RenderDisplayWindow = true;
        }

        /// <summary>
        /// Updates the progress window with the sample data.
        /// </summary>
        /// <param name="sample"></param>
        public void UpdateSample(classSampleData sample)
        {
            mcontrol_sampleProgress.UpdateSample(sample);
            mcontrol_sampleProgressFull.UpdateSample(sample);
        }

        public void UpdateError(classSampleData sample, LcmsNetDataClasses.Method.classLCEvent lcEvent)
        {
            mcontrol_sampleProgressFull.UpdateError(sample, lcEvent);
        }

        /// <summary>
        /// Updates preview lable minute text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnum_previewMinutes_ValueChanged(object sender, EventArgs e)
        {
            mlabel_previewMinutes.Text = Convert.ToString(mnum_previewMinutes.Value) + "-minute-preview";
            mcontrol_sampleProgress.PreviewMinutes = Convert.ToInt32(mnum_previewMinutes.Value);
            mcontrol_sampleProgressFull.PreviewMinutes = mcontrol_sampleProgress.PreviewMinutes;
        }

        public event EventHandler<SampleProgressPreviewArgs> PreviewAvailable;

        private void mtimer_preview_Tick(object sender, EventArgs e)
        {
            mcontrol_sampleProgress.Refresh();
            mcontrol_sampleProgressFull.Refresh();

            if (PreviewAvailable != null)
            {
                try
                {
                    System.Drawing.Bitmap map = new System.Drawing.Bitmap(mcontrol_sampleProgressFull.Width,
                        mcontrol_sampleProgressFull.Height);
                    System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(map);
                    mcontrol_sampleProgressFull.RenderGraph(gfx);
                        //DrawToBitmap(map,new System.Drawing.Rectangle(0, 0, mcontrol_sampleProgressFull.Width, mcontrol_sampleProgressFull.Height));
                    PreviewAvailable(this, new SampleProgressPreviewArgs((System.Drawing.Image) map.Clone()));
                    map.Dispose();
                    gfx.Dispose();
                }
                catch
                {
                    LcmsNetDataClasses.Logging.classApplicationLogger.LogError(0,
                        "Error attempting to update column sample progress.");
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void milliseconds_ValueChanged(object sender, EventArgs e)
        {
        }
    }

    public class SampleProgressPreviewArgs : EventArgs, IDisposable
    {
        public bool disposed;

        public SampleProgressPreviewArgs(System.Drawing.Image image)
        {
            disposed = false;
            PreviewImage = image;
        }

        /// <summary>
        /// Gets the preview image for the sample progress
        /// </summary>
        public System.Drawing.Image PreviewImage { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposeOthers)
        {
            if (disposed)
            {
                return;
            }
            if (disposeOthers)
            {
                PreviewImage.Dispose();
                PreviewImage = null;
            }
            disposed = true;
        }
    }
}