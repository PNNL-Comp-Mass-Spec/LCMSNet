﻿using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using LcmsNetCommonControls.ViewModels;
using ReactiveUI;

namespace LcmsNetCommonControls.Views
{
    /// <summary>
    /// Interaction logic for PopoutWindow.xaml
    /// </summary>
    public partial class PopoutWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PopoutWindow()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.Content).Subscribe(x => this.SetContentDataContext());
        }

        private void SetContentDataContext()
        {
            if (this.Content is FrameworkElement fe)
            {
                var binding = new Binding("Child");
                fe.SetBinding(DataContextProperty, binding);
            }
        }

        /// <summary>
        /// Button horizontal position dependency property
        /// </summary>
        public static readonly DependencyProperty HorizontalButtonAlignmentProperty =
            DependencyProperty.Register("HorizontalButtonAlignment", typeof(HorizontalAlignment), typeof(PopoutWindow), new PropertyMetadata(HorizontalAlignment.Left, UpdatePositioning));

        /// <summary>
        /// Button vertical position dependency property
        /// </summary>
        public static readonly DependencyProperty VerticalButtonAlignmentProperty =
            DependencyProperty.Register("VerticalButtonAlignment", typeof(VerticalAlignment), typeof(PopoutWindow), new PropertyMetadata(VerticalAlignment.Bottom, UpdatePositioning));

        /// <summary>
        /// Button position overlay dependency property
        /// </summary>
        public static readonly DependencyProperty OverlayButtonProperty =
            DependencyProperty.Register("OverlayButton", typeof(bool), typeof(PopoutWindow), new PropertyMetadata(false, UpdatePositioning));

        /// <summary>
        /// Button grid row dependency property
        /// </summary>
        public static readonly DependencyProperty ButtonGridRowProperty =
            DependencyProperty.Register("ButtonGridRow", typeof(int), typeof(PopoutWindow), new PropertyMetadata(2));

        /// <summary>
        /// Button grid column dependency property
        /// </summary>
        public static readonly DependencyProperty ButtonGridColumnProperty =
            DependencyProperty.Register("ButtonGridColumn", typeof(int), typeof(PopoutWindow), new PropertyMetadata(1));

        /// <summary>
        /// Button border preference dependency property
        /// </summary>
        public static readonly DependencyProperty PreferVerticalBorderProperty =
            DependencyProperty.Register("PreferVerticalBorder", typeof(bool), typeof(PopoutWindow), new PropertyMetadata(false, UpdatePositioning));

        /// <summary>
        /// Horizontal positioning of the popout button
        /// </summary>
        public HorizontalAlignment HorizontalButtonAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalButtonAlignmentProperty);
            set => SetValue(HorizontalButtonAlignmentProperty, value);
        }

        /// <summary>
        /// Vertical positioning of the popout button
        /// </summary>
        public VerticalAlignment VerticalButtonAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalButtonAlignmentProperty);
            set => SetValue(VerticalButtonAlignmentProperty, value);
        }

        /// <summary>
        /// If the button should be overlaid on top of the content
        /// </summary>
        public bool OverlayButton
        {
            get => (bool)GetValue(OverlayButtonProperty);
            set => SetValue(OverlayButtonProperty, value);
        }

        /// <summary>
        /// Positioning of the button in the display grid rows. Set internally.
        /// </summary>
        public int ButtonGridRow
        {
            get => (int)GetValue(ButtonGridRowProperty);
            private set => SetValue(ButtonGridRowProperty, value);
        }

        /// <summary>
        /// Positioning of the button in the display grid columns. Set internally.
        /// </summary>
        public int ButtonGridColumn
        {
            get => (int)GetValue(ButtonGridColumnProperty);
            private set => SetValue(ButtonGridColumnProperty, value);
        }

        /// <summary>
        /// When button is not overlaid and placed in a corner, if a vertical border is prefered over a horizontal border
        /// </summary>
        public bool PreferVerticalBorder
        {
            get => (bool)GetValue(PreferVerticalBorderProperty);
            set => SetValue(PreferVerticalBorderProperty, value);
        }

        private static void UpdatePositioning(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PopoutWindow pw))
            {
                return;
            }
            if (pw.OverlayButton)
            {
                pw.ButtonGridRow = 1;
                pw.ButtonGridColumn = 1;
                return;
            }
            var row = 1;
            var column = 1;
            switch (pw.HorizontalButtonAlignment)
            {
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    column = 1;
                    break;
                case HorizontalAlignment.Right:
                    column = 2;
                    break;
                case HorizontalAlignment.Left:
                default:
                    column = 0;
                    break;
            }
            switch (pw.VerticalButtonAlignment)
            {
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    row = 1;
                    break;
                case VerticalAlignment.Top:
                    row = 0;
                    break;
                case VerticalAlignment.Bottom:
                default:
                    row = 2;
                    break;
            }
            if (row != 1 && column != 1)
            {
                if (pw.PreferVerticalBorder)
                {
                    row = 1;
                }
                else
                {
                    column = 1;
                }
            }
            pw.ButtonGridRow = row;
            pw.ButtonGridColumn = column;
        }

        private void PopoutWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is PopoutViewModel pvm && !isClosed)
            {
                pvm.WhenAnyValue(x => x.Tacked).Where(x => x).Subscribe(x => this.Close());
            }
        }

        private bool isClosed = false;

        private void PopoutWindow_OnClosed(object sender, EventArgs e)
        {
            if (this.DataContext is PopoutViewModel pvm && !pvm.Tacked)
            {
                isClosed = true;
                pvm.Tacked = true;
            }
        }
    }
}
