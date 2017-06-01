using System.Windows.Forms;
using LcmsNet.Method.ViewModels;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Form that displays a control for editing methods.
    /// </summary>
    public partial class formMethodEditor2 : Form
    {
        private LCMethodEditorViewModel lcMethodEditorViewModel;

        /// <summary>
        /// Constructor.
        /// </summary>
        public formMethodEditor2()
        {
            InitializeComponent();

            lcMethodEditorViewModel = new LCMethodEditorViewModel();
            lcMethodEditorView.DataContext = lcMethodEditorViewModel;
        }
    }
}