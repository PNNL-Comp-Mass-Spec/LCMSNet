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
    /// Interaction logic for MethodManagerView.xaml
    /// </summary>
    public partial class MethodManagerView : UserControl
    {
        public MethodManagerView()
        {
            InitializeComponent();
        }

        private void Method_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MethodControlView mcv)
            {
                ColumnDefinition cd = new ColumnDefinition();
                if (ReferenceEquals(mcv, Method1View))
                {
                    cd = Method1Column;
                }
                else if (ReferenceEquals(mcv, Method2View))
                {
                    cd = Method2Column;
                }
                else if (ReferenceEquals(mcv, Method3View))
                {
                    cd = Method3Column;
                }
                else if (ReferenceEquals(mcv, Method4View))
                {
                    cd = Method4Column;
                }
                else if (ReferenceEquals(mcv, Method5View))
                {
                    cd = Method5Column;
                }
                else if (ReferenceEquals(mcv, Method6View))
                {
                    cd = Method6Column;
                }
                else if (ReferenceEquals(mcv, Method7View))
                {
                    cd = Method7Column;
                }
                else if (ReferenceEquals(mcv, Method8View))
                {
                    cd = Method8Column;
                }
                if (mcv.IsVisible)
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
            if (sender is Border b && b.DataContext is MethodControlViewModel mcvm)
            {
                if (this.DataContext is MethodManagerViewModel mmvm)
                {
                    mmvm.ClearFocused();
                }
                mcvm.ContainsKeyboardFocus = true;
            }
        }
    }
}
