using System.Windows;
using WpfExtras;

namespace LcmsNetPlugins.ZaberStage.UI
{
    /// <summary>
    /// Binding proxy to help with DataGrid binding to base DataContext
    /// </summary>
    /// <remarks>https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/</remarks>
    public class StageConfigBindingProxy : BindingProxy<StageConfigViewModel>
    {
        protected override BindingProxy<StageConfigViewModel> CreateNewInstance()
        {
            return new StageConfigBindingProxy();
        }

        /// <summary>
        /// Data object for binding
        /// </summary>
        public override StageConfigViewModel Data
        {
            get => (StageConfigViewModel)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        /// <summary>
        /// DependencyProperty definition for Data
        /// </summary>
        public new static readonly DependencyProperty DataProperty = BindingProxy<StageConfigViewModel>.DataProperty.AddOwner(typeof(StageConfigBindingProxy));
    }
}
