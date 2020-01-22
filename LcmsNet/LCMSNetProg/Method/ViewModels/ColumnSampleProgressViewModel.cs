using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LcmsNetData.Logging;
using LcmsNetSDK.Data;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class ColumnSampleProgressViewModel : ReactiveObject, IDisposable
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnSampleProgressViewModel()
        {
            SampleProgress = new SampleProgressViewModel();
            SampleProgressFull = new SampleProgressViewModel();

            SampleProgress.RenderAllEvents = false;
            SampleProgress.RenderCurrent = true;

            SampleProgressFull.RenderAllEvents = true;
            SampleProgressFull.RenderCurrent = false;
            SampleProgressFull.RenderDisplayWindow = true;

            this.WhenAnyValue(x => x.Minutes, x => x.Seconds).Subscribe(x => this.PreviewTimeUpdated());

            previewUpdateTimer = new Timer(UpdateDrawings, this, 1000, 1000);
        }

        ~ColumnSampleProgressViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            previewUpdateTimer?.Dispose();
            GC.SuppressFinalize(this);
        }

        private const int MinTimeSeconds = 5;

        private string previewLabelText = "30-minute Preview";
        private int minutes = 30;
        private int seconds = 0;
        private int minSeconds = -1;
        private readonly Timer previewUpdateTimer;
        private SampleProgressViewModel sampleProgress;
        private SampleProgressViewModel sampleProgressFull;

        public string PreviewLabelText
        {
            get => previewLabelText;
            set => this.RaiseAndSetIfChanged(ref previewLabelText, value);
        }

        public int MinSeconds
        {
            get => minSeconds;
            set => this.RaiseAndSetIfChanged(ref minSeconds, value);
        }

        public int Minutes
        {
            get => minutes;
            set
            {
                if (minutes != value)
                {
                    if (value == 0)
                    {
                        MinSeconds = MinTimeSeconds;
                        if (Seconds < MinSeconds)
                        {
                            Seconds = MinSeconds;
                        }
                    }
                    else
                    {
                        MinSeconds = -1;
                        if (Seconds == MinTimeSeconds)
                        {
                            Seconds = 0;
                        }
                    }
                }

                this.RaiseAndSetIfChanged(ref minutes, value);
            }
        }

        public int Seconds
        {
            get => seconds;
            set
            {
                if (value == 60)
                {
                    Minutes += 1;
                    value = 0;
                }
                else if (value == -1)
                {
                    Minutes -= 1;
                    value = 59;
                }
                this.RaiseAndSetIfChanged(ref seconds, value);
            }
        }

        public SampleProgressViewModel SampleProgress
        {
            get => sampleProgress;
            set => this.RaiseAndSetIfChanged(ref sampleProgress, value);
        }

        public SampleProgressViewModel SampleProgressFull
        {
            get => sampleProgressFull;
            set => this.RaiseAndSetIfChanged(ref sampleProgressFull, value);
        }

        /// <summary>
        /// Use the given sample to clear out displayed progress and errors for the column
        /// </summary>
        /// <param name="sample"></param>
        public void ResetColumn(SampleData sample)
        {
            SampleProgress.ResetColumn(sample);
            SampleProgressFull.ResetColumn(sample);
        }

        /// <summary>
        /// Clears out the existing visuals
        /// </summary>
        public void ResetVisuals()
        {
            SampleProgress.ClearSamples();
            SampleProgressFull.ClearSamples();
        }

        /// <summary>
        /// Updates the progress window with the sample data.
        /// </summary>
        /// <param name="sample"></param>
        public void UpdateSample(SampleData sample)
        {
            SampleProgress.UpdateSample(sample);
            SampleProgressFull.UpdateSample(sample);
        }

        public void UpdateError(SampleData sample, LCEvent lcEvent)
        {
            SampleProgress.UpdateError(sample, lcEvent);
            SampleProgressFull.UpdateError(sample, lcEvent);
        }

        /// <summary>
        /// Updates preview label minute text.
        /// </summary>
        private void PreviewTimeUpdated()
        {
            var labelText = Minutes + "-minute Preview";
            if (Seconds == 0)
            {
                labelText = Minutes + "-minute Preview";
            }
            else if (Minutes == 0)
            {
                labelText = Seconds + "-second Preview";
            }
            else
            {
                labelText = $"{Minutes}:{Seconds:00}-minute Preview";
            }
            PreviewLabelText = labelText;
            SampleProgress.UpdatePreviewTime(Minutes, Seconds);
            SampleProgressFull.UpdatePreviewTime(Minutes, Seconds);
        }

        public event EventHandler<SampleProgressPreviewArgs> PreviewAvailable;

        private void UpdateDrawings(object sender)
        {
            SampleProgress.Refresh();
            SampleProgressFull.Refresh();

            if (PreviewAvailable != null)
            {
                try
                {
                    var drawVisual = new DrawingVisual();
                    var drawContext = drawVisual.RenderOpen();
                    sampleProgressFull.RenderGraph(drawContext, new Rect(0, 0, 800, 200));
                    drawContext.Close();
                    var rtb = new RenderTargetBitmap(800, 200, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(drawVisual);
                    rtb.Freeze();

                    PreviewAvailable(this, new SampleProgressPreviewArgs(rtb));
                }
                catch
                {
                    ApplicationLogger.LogError(0, "Error attempting to update column sample progress.");
                }
            }
        }
    }

    public class SampleProgressPreviewArgs : EventArgs
    {
        public bool disposed;

        public SampleProgressPreviewArgs(BitmapSource image)
        {
            disposed = false;
            PreviewImage = image;
        }

        /// <summary>
        /// Gets the preview image for the sample progress
        /// </summary>
        public BitmapSource PreviewImage { get; private set; }
    }
}
