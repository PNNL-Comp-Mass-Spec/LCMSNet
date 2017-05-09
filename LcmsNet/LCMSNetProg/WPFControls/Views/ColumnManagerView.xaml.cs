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
using LcmsNet.WPFControls.ViewModels;

namespace LcmsNet.WPFControls.Views
{
    /// <summary>
    /// Interaction logic for ColumnManagerView.xaml
    /// </summary>
    public partial class ColumnManagerView : UserControl
    {
        public ColumnManagerView()
        {
            InitializeComponent();
        }

        private void Column_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ColumnControlView ccv)
            {
                ColumnDefinition cd = new ColumnDefinition();
                if (ReferenceEquals(ccv, Column1View))
                {
                    cd = Column1Column;
                }
                else if (ReferenceEquals(ccv, Column2View))
                {
                    cd = Column2Column;
                }
                else if (ReferenceEquals(ccv, Column3View))
                {
                    cd = Column3Column;
                }
                else if (ReferenceEquals(ccv, Column4View))
                {
                    cd = Column4Column;
                }
                if (ccv.IsVisible)
                {
                    cd.Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    cd.Width = new GridLength(0);
                }
            }
        }

        private void Border_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.DataContext is ColumnControlViewModel ccvm)
            {
                if (this.DataContext is ColumnManagerViewModel cmvm)
                {
                    cmvm.ClearFocused();
                }
                ccvm.ContainsKeyboardFocus = true;
            }
        }
    }
}
