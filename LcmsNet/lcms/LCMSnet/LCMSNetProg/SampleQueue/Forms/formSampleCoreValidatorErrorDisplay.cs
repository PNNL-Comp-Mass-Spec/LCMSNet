using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using LcmsNetDataClasses;

namespace LcmsNet.SampleQueue
{
    public partial class formSampleValidatorErrorDisplay : Form
    {
        public formSampleValidatorErrorDisplay(Dictionary<classSampleData, List<classSampleValidationError>> errors)
        {
            InitializeComponent();

            
            int i = 0;
            foreach (classSampleData sample in errors.Keys)
            {
                foreach (classSampleValidationError error in errors[sample])
                {
                    i = i + 1;
                    ListViewItem item = new ListViewItem();
                    item.Text = sample.DmsData.DatasetName;
                    item.SubItems.Add(error.Error);
                    if ((i % 2) == 0)
                        item.BackColor = Color.LightGray;
                    mlistview_errors.Items.Add(item);
                }                
            }
        }
    }
}
