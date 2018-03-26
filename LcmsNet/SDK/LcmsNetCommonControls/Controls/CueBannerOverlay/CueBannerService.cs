using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace LcmsNetCommonControls.Controls.CueBannerOverlay
{
    /// <summary>
    /// Control for displaying text cues when a textbox/combobox is set to null
    /// </summary>
    /// <remarks>Copied from https://jasonkemp.ca/blog/the-missing-net-4-cue-banner-in-wpf-i-mean-watermark-in-wpf/ and https://stackoverflow.com/questions/833943/watermark-hint-text-placeholder-textbox-in-wpf </remarks>
    public static class CueBannerService
    {
        /// <summary>
        /// CueBanner Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CueBannerProperty = DependencyProperty.RegisterAttached(
           "CueBanner",
           typeof(object),
           typeof(CueBannerService),
           new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(OnCueBannerChanged)));

        /// <summary>
        /// ShowCueBannerOnComboBoxUnmatch Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowCueBannerOnComboBoxUnmatchProperty = DependencyProperty.RegisterAttached(
            "ShowCueBannerOnComboBoxUnmatch",
            typeof(bool),
            typeof(ComboBox), new PropertyMetadata(false));

        #region Private Fields

        /// <summary>
        /// Dictionary of ItemsControls
        /// </summary>
        private static readonly WeakDictionary<object, ItemsControl> itemsControls = new WeakDictionary<object, ItemsControl>();

        #endregion

        /// <summary>
        /// Gets the CueBanner property.  This dependency property indicates the CueBanner for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to get the property from</param>
        /// <returns>The value of the CueBanner property</returns>
        public static object GetCueBanner(DependencyObject d)
        {
            return (object)d.GetValue(CueBannerProperty);
        }

        /// <summary>
        /// Sets the CueBanner property.  This dependency property indicates the CueBanner for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetCueBanner(DependencyObject d, object value)
        {
            d.SetValue(CueBannerProperty, value);
        }

        /// <summary>
        /// Gets the GetShowCueBannerOnComboBoxUnmatch property.  This dependency property indicates if the CueBanner should be shown when a ComboBox's SelectedItem has no match in ItemsSource.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to get the property from</param>
        /// <returns>The value of the GetShowCueBannerOnComboBoxUnmatch property</returns>
        public static bool GetShowCueBannerOnComboBoxUnmatch(DependencyObject d)
        {
            return (bool)d.GetValue(ShowCueBannerOnComboBoxUnmatchProperty);
        }

        /// <summary>
        /// Sets the GetShowCueBannerOnComboBoxUnmatch property.  This dependency property indicates if the CueBanner should be shown when a ComboBox's SelectedItem has no match in ItemsSource.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetShowCueBannerOnComboBoxUnmatch(DependencyObject d, bool value)
        {
            d.SetValue(ShowCueBannerOnComboBoxUnmatchProperty, value);
        }

        /// <summary>
        /// Handles changes to the CueBanner property.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> that fired the event</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCueBannerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            control.Loaded += Control_Loaded;

            if (d is TextBox || d is PasswordBox || d is ComboBox)
            {
                control.GotKeyboardFocus += Control_GotKeyboardFocus;
                control.LostKeyboardFocus += Control_Loaded;

                if (d is TextBox tb)
                {
                    tb.TextChanged += Control_ContentChanged;
                }
                else if (d is PasswordBox pb)
                {
                    pb.PasswordChanged += Control_ContentChanged;
                }
                else if (d is ComboBox cb)
                {
                    cb.SelectionChanged += Control_ContentChanged;
                }
            }
            else if (d is ItemsControl)
            {
                ItemsControl i = (ItemsControl)d;

                itemsControls.RemoveCollectedEntries();

                // for Items property
                // Strong reference from the event source to local listener is not a memory leak (keeping an item that should be garbage collected alive)
                i.ItemContainerGenerator.ItemsChanged += ItemsChanged;
                // Using WeakDictionary to avoid keeping items alive when they are otherwise not used
                itemsControls.Add(i.ItemContainerGenerator, i);

                // for ItemsSource property
                // Original code, holds strong reference that would keep object from being garbage collected
                //DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, i.GetType());
                //prop.AddValueChanged(i, ItemsSourceChanged);

                // WpfPropertyChangeNotifier uses weak references and WPF data binding to avoid holding a strong reference to the object
                var notifier = new WpfPropertyChangeNotifier(i, nameof(i.ItemsSource));
                notifier.ValueChanged += ItemsSourceChanged;
            }
        }

        #region Event Handlers

        /// <summary>
        /// Handle the GotFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            Control c = (Control)sender;
            if (ShouldShowCueBanner(c))
            {
                RemoveCueBanner(c);
            }
        }

        /// <summary>
        /// Handle the Loaded and LostFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            Control control = (Control)sender;
            if (ShouldShowCueBanner(control))
            {
                ShowCueBanner(control);
            }
        }

        /// <summary>
        /// Event handler for the items source changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private static void ItemsSourceChanged(object sender, EventArgs e)
        {
            ItemsControl c = (ItemsControl)sender;
            if (c.ItemsSource != null)
            {
                if (ShouldShowCueBanner(c))
                {
                    ShowCueBanner(c);
                }
                else
                {
                    RemoveCueBanner(c);
                }
            }
            else
            {
                ShowCueBanner(c);
            }
        }

        /// <summary>
        /// Event handler for the items changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ItemsChangedEventArgs"/> that contains the event data.</param>
        private static void ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            ItemsControl control;
            if (itemsControls.TryGetValue(sender, out control))
            {
                if (ShouldShowCueBanner(control))
                {
                    ShowCueBanner(control);
                }
                else
                {
                    RemoveCueBanner(control);
                }
            }
        }

        /// <summary>
        /// Remove the CueBanner if the text has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Control_ContentChanged(object sender, RoutedEventArgs e)
        {
            var control = (Control)sender;
            if (ShouldShowCueBanner(control))
            {
                ShowCueBanner(control);
            }
            else
            {
                RemoveCueBanner(control);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Remove the CueBanner from the specified element
        /// </summary>
        /// <param name="control">Element to remove the CueBanner from</param>
        private static void RemoveCueBanner(UIElement control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(control);
                if (adorners == null)
                {
                    return;
                }

                foreach (Adorner adorner in adorners)
                {
                    if (adorner is CueBannerAdorner)
                    {
                        adorner.Visibility = Visibility.Hidden;
                        layer.Remove(adorner);
                    }
                }
            }
        }

        /// <summary>
        /// Show the CueBanner on the specified control
        /// </summary>
        /// <param name="control">Control to show the CueBanner on</param>
        private static void ShowCueBanner(Control control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
            {
                layer.Add(new CueBannerAdorner(control, GetCueBanner(control)));
            }
        }

        /// <summary>
        /// Indicates whether or not the CueBanner should be shown on the specified control
        /// </summary>
        /// <param name="c"><see cref="Control"/> to test</param>
        /// <returns>true if the CueBanner should be shown; false otherwise</returns>
        private static bool ShouldShowCueBanner(Control c)
        {
            if (c is ComboBox cb)
            {
                if (cb.SelectedItem == null)
                {
                    return true;
                }

                if (GetShowCueBannerOnComboBoxUnmatch(c))
                {
                    foreach (var obj in cb.ItemsSource)
                    {
                        if (obj.Equals(cb.SelectedItem))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;

                //return (c as ComboBox).SelectedItem == null;
                //return (c as ComboBox).Text == string.Empty;
            }
            else if (c is TextBox)
            {
                return (c as TextBox).Text == string.Empty;
            }
            else if (c is PasswordBox)
            {
                return (c as PasswordBox).Password == string.Empty;
            }
            else if (c is ItemsControl)
            {
                return (c as ItemsControl).Items.Count == 0;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
