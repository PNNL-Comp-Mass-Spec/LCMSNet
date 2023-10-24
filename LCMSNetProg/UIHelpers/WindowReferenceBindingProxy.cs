using System.Windows;
using WpfExtras;

namespace LcmsNet.UIHelpers
{
    /// <summary>
    /// Binding proxy to help with DataGrid binding to base DataContext
    /// </summary>
    /// <remarks>https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/</remarks>
    public class WindowReferenceBindingProxy : BindingProxy<IProvidesWindowReference>
    {
        protected override BindingProxy<IProvidesWindowReference> CreateNewInstance()
        {
            return new WindowReferenceBindingProxy();
        }

        /// <summary>
        /// Data object for binding
        /// </summary>
        public override IProvidesWindowReference Data
        {
            get => (IProvidesWindowReference)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        /// <summary>
        /// DependencyProperty definition for Data
        /// </summary>
        public new static readonly DependencyProperty DataProperty = BindingProxy<IProvidesWindowReference>.DataProperty.AddOwner(typeof(WindowReferenceBindingProxy));
    }
}
