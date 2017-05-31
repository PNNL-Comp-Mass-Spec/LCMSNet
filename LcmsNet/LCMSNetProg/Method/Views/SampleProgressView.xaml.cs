using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LcmsNet.Method.ViewModels;

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for SampleProgressView.xaml
    /// </summary>
    public partial class SampleProgressView : UserControl
    {
        public SampleProgressView()
        {
            InitializeComponent();
        }

        private void SampleProgressView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is SampleProgressViewModel spvm)
            {
                if (sampleProgressDataContext != null)
                {
                    sampleProgressDataContext.RefreshView -= RefreshVisual;
                }
                sampleProgressDataContext = spvm;
                sampleProgressDataContext.RefreshView += RefreshVisual;
            }
        }

        private SampleProgressViewModel sampleProgressDataContext = null;

        private void DrawingContainer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshVisual(sender, e);
        }

        private void RefreshVisual(object sender, EventArgs e)
        {
            if (!this.IsVisible)
            {
                return;
            }
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshVisualInternal());
            }
            else
            {
                RefreshVisualInternal();
            }
        }

        private void RefreshVisualInternal()
        {
            var drawingVisual = new DrawingVisual();
            var drawContext = drawingVisual.RenderOpen();
            drawContext.PushOpacity(100);
            drawContext.DrawRectangle(Brushes.White, null, new Rect(new Point(-5, -5), new Size(DrawingContainer.ActualWidth + 5, DrawingContainer.ActualHeight + 5)));
            drawContext.Pop();
            sampleProgressDataContext.RenderGraph(drawContext, new Rect(new Point(0, 0), DrawingContainer.RenderSize));
            drawContext.Close();
            DrawingContainer.AddDrawingVisual(drawingVisual);
        }
    }
}
