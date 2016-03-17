using System;
using System.Windows.Forms;
using System.Collections.Generic;
using LcmsNetDataClasses;

namespace LcmsNet.SampleQueue.Forms
{
    public partial class formSampleBadBlockDisplay : Form
    {
        public formSampleBadBlockDisplay(List<classSampleData> samples)
        {
            InitializeComponent();

            mlistview_samples.BeginUpdate();
            mlistview_samples.Items.Clear();
            foreach (classSampleData sample in samples)
            {
                DisplaySample(sample);
            }
            mlistview_samples.EndUpdate();
        }

        private void DisplaySample(classSampleData sample)
        {
            ListViewItem item = new ListViewItem();
            item.Text = sample.DmsData.Batch.ToString();

            ListViewItem.ListViewSubItem block = new ListViewItem.ListViewSubItem(item, sample.DmsData.Block.ToString());
            ListViewItem.ListViewSubItem dataset = new ListViewItem.ListViewSubItem(item, sample.DmsData.DatasetName);
            ListViewItem.ListViewSubItem column = new ListViewItem.ListViewSubItem(item,
                (sample.ColumnData.ID + 1).ToString());
            ListViewItem.ListViewSubItem method = new ListViewItem.ListViewSubItem(item, sample.LCMethod.Name);

            item.SubItems.Add(block);
            item.SubItems.Add(column);
            item.SubItems.Add(dataset);
            item.SubItems.Add(method);

            mlistview_samples.Items.Add(item);
        }

        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}