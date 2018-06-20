﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace LcmsNetCommonControls.Controls.CueBannerOverlay
{
    /// <summary>
    /// Control for displaying text cues when a textbox/combobox is set to null
    /// </summary>
    /// <remarks>Copied from https://jasonkemp.ca/blog/the-missing-net-4-cue-banner-in-wpf-i-mean-watermark-in-wpf/ and https://stackoverflow.com/questions/833943/watermark-hint-text-placeholder-textbox-in-wpf </remarks>
    internal class CueBannerAdorner : Adorner
    {
        #region Private Fields

        /// <summary>
        /// <see cref="ContentPresenter"/> that holds the cue banner
        /// </summary>
        private readonly ContentPresenter contentPresenter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CueBannerAdorner"/> class
        /// </summary>
        /// <param name="adornedElement"><see cref="UIElement"/> to be adorned</param>
        /// <param name="cueBanner">The cue banner</param>
        public CueBannerAdorner(UIElement adornedElement, object cueBanner) :
           base(adornedElement)
        {
            this.IsHitTestVisible = false;

            this.contentPresenter = new ContentPresenter();
            this.contentPresenter.Content = cueBanner;
            this.contentPresenter.Opacity = 0.5;
            this.contentPresenter.Margin = new Thickness(Control.Padding.Left, Control.Padding.Top + 1, Control.Padding.Right, Control.Padding.Bottom);

            if (this.Control is ItemsControl && !(this.Control is ComboBox))
            {
                this.contentPresenter.VerticalAlignment = VerticalAlignment.Center;
                this.contentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
            }

            // Hide the control adorner when the adorned element is hidden
            this.SetBinding(VisibilityProperty, new Binding(IsVisibleProperty.Name)
            {
                Mode = BindingMode.OneWay,
                Source = adornedElement,
                Converter = new BooleanToVisibilityConverter(),
            });

            // Make sure the DataContext is approriately set
            this.SetBinding(DataContextProperty, new Binding(DataContextProperty.Name)
            {
                Mode = BindingMode.OneWay,
                Source = AdornedElement,
            });
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the number of children for the <see cref="ContainerVisual"/>.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the control that is being adorned
        /// </summary>
        private Control Control
        {
            get { return (Control)this.AdornedElement; }
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Returns a specified child <see cref="Visual"/> for the parent <see cref="ContainerVisual"/>.
        /// </summary>
        /// <param name="index">A 32-bit signed integer that represents the index value of the child <see cref="Visual"/>. The value of index must be between 0 and <see cref="VisualChildrenCount"/> - 1.</param>
        /// <returns>The child <see cref="Visual"/>.</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.contentPresenter;
        }

        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>A <see cref="Size"/> object representing the amount of layout space needed by the adorner.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            // Here's the secret to getting the adorner to cover the whole control
            this.contentPresenter.Measure(Control.RenderSize);
            return Control.RenderSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        #endregion
    }
}
