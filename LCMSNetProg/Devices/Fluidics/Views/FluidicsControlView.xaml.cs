﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LcmsNet.Devices.Fluidics.ViewModels;

namespace LcmsNet.Devices.Fluidics.Views
{
    /// <summary>
    /// Interaction logic for FluidicsControlView.xaml
    /// </summary>
    public partial class FluidicsControlView : UserControl
    {
        public FluidicsControlView()
        {
            InitializeComponent();

            Loaded += (sender, args) => thisWindow = Window.GetWindow(this);
        }

        private Window thisWindow;

        private void FluidicsControlView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is FluidicsControlViewModel fcvm)
            {
                if (fluidicsControlDataContext != null)
                {
                    fluidicsControlDataContext.RefreshView -= RefreshVisual;
                }
                fluidicsControlDataContext = fcvm;
                fluidicsControlDataContext.RefreshView += RefreshVisual;
            }
        }

        private FluidicsControlViewModel fluidicsControlDataContext = null;

        private void DrawingContainer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (fluidicsControlDataContext != null)
            {
                fluidicsControlDataContext.DesignPanelSize = e.NewSize;
            }
            RefreshVisual(sender, e);
        }

        private void RefreshVisual(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(RefreshVisualInternal);
            }
            else
            {
                RefreshVisualInternal();
            }
        }

        private void RefreshVisualInternal()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            var drawingVisual = new DrawingVisual();
            Size size;
            using (var drawContext = drawingVisual.RenderOpen())
            {
                drawContext.PushOpacity(100);
                // Use scrollviewer size, since the size of the contained items isn't automatically stretched to match the scrollviewer boundaries
                //drawContext.DrawRectangle(Brushes.White, null, new Rect(new Point(-5, -5), new Size(DrawingContainer.ActualWidth + 5, DrawingContainer.ActualHeight + 5)));
                drawContext.DrawRectangle(Brushes.White, null,
                    new Rect(new Point(-5, -5), new Size(ScrollViewer.ActualWidth + 10, ScrollViewer.ActualHeight + 10)));
                drawContext.Pop();
                //var size = fluidicsControlDataContext.RenderGraphics(drawContext, DrawingContainer.RenderSize);
                size = fluidicsControlDataContext.RenderGraphics(drawContext, ScrollViewer.RenderSize);
            }

            DrawingContainer.Width = size.Width;
            DrawingContainer.Height = size.Height;
            DrawingContainer.AddDrawingVisual(drawingVisual, true);
        }

        private bool leftMouseDown = false;
        private Point oldMouseLoc = new Point(0, 0);

        private void DrawingContainer_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount > 1)
                {
                    fluidicsControlDataContext.DesignDoubleClick(e.GetPosition(DrawingContainer));
                    return;
                }
                leftMouseDown = true;
                oldMouseLoc = e.GetPosition(DrawingContainer);
                fluidicsControlDataContext.MouseDownUpdates(oldMouseLoc);
            }
        }

        private void DrawingContainer_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                leftMouseDown = false;
                fluidicsControlDataContext.MouseUpUpdates(e.GetPosition(DrawingContainer), thisWindow);
            }
        }

        private void DrawingContainer_OnMouseMove(object sender, MouseEventArgs e)
        {
            //if left mouse is held down and render mode is conversation...scroll this way.
            if (leftMouseDown)
            {
                fluidicsControlDataContext.MouseMovedUpdates(e.GetPosition(DrawingContainer));
            }
        }
    }
}
