using System.Windows.Forms;

namespace LcmsNet.SampleQueue.Forms
{
    public delegate void DelegateOnPaint(object sender, PaintEventArgs e);

    /// <summary>
    /// Datagrid class that overrides the Windows Forms class for special painting and selection.
    /// </summary>
    public partial class classDataGrid : DataGridView
    {
        public classDataGrid()
        {
            InitializeComponent();
        }

        public event DelegateOnPaint SpecialPaint;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            SpecialPaint?.Invoke(this, e);
        }
    }
}