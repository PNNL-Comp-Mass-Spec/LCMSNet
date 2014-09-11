using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace LcmsNet.SampleQueue.Forms
{
    public delegate void DelegateOnPaint(object sender, PaintEventArgs e);

    /// <summary>
    /// Datagrid class that overrides the Windows Forms class for special painting and selection.
    /// </summary>
    public partial class classDataGrid : DataGridView
    {
        public event DelegateOnPaint SpecialPaint;

        public classDataGrid()
        {			
            InitializeComponent();            
        }
        protected override void SetSelectedRowCore(int rowIndex, bool selected)
        {            
            base.SetSelectedRowCore(rowIndex, selected);
        }
        protected override void OnPaint(PaintEventArgs e)
        {            
            base.OnPaint(e);

            if (this.SpecialPaint != null)            
                SpecialPaint(this, e);                         
        }
    }
}
