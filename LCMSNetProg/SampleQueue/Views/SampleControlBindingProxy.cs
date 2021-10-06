using System.Windows;
using LcmsNet.SampleQueue.ViewModels;
using WpfExtras;

namespace LcmsNet.SampleQueue.Views
{
    /// <summary>
    /// Binding proxy to help with DataGrid binding to base DataContext
    /// </summary>
    /// <remarks>https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/</remarks>
    public class SampleControlBindingProxy : BindingProxy<SampleControlViewModel>
    {
        protected override BindingProxy<SampleControlViewModel> CreateNewInstance()
        {
            return new SampleControlBindingProxy();
        }

        /// <summary>
        /// Data object for binding
        /// </summary>
        public override SampleControlViewModel Data
        {
            get => (SampleControlViewModel)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        /// <summary>
        /// DependencyProperty definition for Data
        /// </summary>
        public new static readonly DependencyProperty DataProperty = BindingProxy<SampleControlViewModel>.DataProperty.AddOwner(typeof(SampleControlBindingProxy));
    }
}
