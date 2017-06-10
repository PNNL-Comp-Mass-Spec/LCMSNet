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
using ReactiveUI;

namespace LcmsNet.Method.Views
{
    /// <summary>
    /// Interaction logic for LCMethodTimelineView.xaml
    /// </summary>
    public partial class LCMethodTimelineView : UserControl
    {
        public LCMethodTimelineView()
        {
            InitializeComponent();
        }

        private void TimelineView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is LCMethodTimelineViewModel lcmtvm)
            {
                if (lcMethodTimelineDataContext != null)
                {
                    lcMethodTimelineDataContext.RefreshView -= RefreshVisual;
                }
                lcMethodTimelineDataContext = lcmtvm;
                lcMethodTimelineDataContext.RefreshView += RefreshVisual;

                lcMethodTimelineDataContext.WhenAnyValue(x => x.StartEventIndex).Subscribe(x =>
                {
                    if (!unmoved)
                    {
                        RefreshVisual(this, EventArgs.Empty);
                    }
                });
            }
        }

        private LCMethodTimelineViewModel lcMethodTimelineDataContext = null;

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
            lcMethodTimelineDataContext.RenderGraphics(drawContext, new Rect(new Point(0, 0), DrawingContainer.RenderSize));
            drawContext.Close();
            DrawingContainer.AddDrawingVisual(drawingVisual, true);
        }

        private bool leftMouseDown = false;
        private bool unmoved = false;
        private Point oldMouseLoc = new Point(0, 0);

        private void DrawingContainer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            leftMouseDown = true;
            oldMouseLoc = e.GetPosition(DrawingContainer);
            unmoved = true;
        }

        private void DrawingContainer_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftMouseDown = false;
            if (lcMethodTimelineDataContext.RenderMode == enumLCMethodRenderMode.Conversation)
            {
                lcMethodTimelineDataContext.MouseUpUpdates(e.GetPosition(DrawingContainer), ref unmoved);
            }
        }

        private void DrawingContainer_OnMouseMove(object sender, MouseEventArgs e)
        {
            //if left mouse is held down and render mode is conversation...scroll this way.
            if (leftMouseDown && lcMethodTimelineDataContext.RenderMode == enumLCMethodRenderMode.Conversation)
            {
                lcMethodTimelineDataContext.MouseMovedUpdates(e.GetPosition(DrawingContainer), ref oldMouseLoc);
                unmoved = false;
            }
        }
    }
}
