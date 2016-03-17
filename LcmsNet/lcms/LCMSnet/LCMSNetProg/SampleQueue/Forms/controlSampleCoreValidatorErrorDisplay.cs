using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Experiment;

namespace LcmsNet.SampleQueue
{
    public partial class controlSampleValidatorErrorDisplay : UserControl
    {
        public controlSampleValidatorErrorDisplay(classSampleData sample, List<classSampleValidationError> errors)
        {
            InitializeComponent();

            mlabel_sampleName.Text = sample.DmsData.DatasetName;
            foreach (classSampleValidationError error in errors)
            {
                AddError(error);
            }
        }

        private void AddError(classSampleValidationError error)
        {
            ListViewItem item = new ListViewItem();
            item.Text = error.Error;
            listView1.Items.Add(item);
        }
    }
}