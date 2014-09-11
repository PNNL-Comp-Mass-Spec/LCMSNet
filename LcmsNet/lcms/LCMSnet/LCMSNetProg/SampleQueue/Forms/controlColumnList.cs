using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using LcmsNetDataClasses;

namespace LcmsNetSampleQueue.Forms
{
    public partial class controlColumnList : UserControl
    {
        private formDMSView mform_dmsView;
        private classSampleQueue mobj_sampleQueue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlColumnList()
        {
            InitializeComponent();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the DMS View form.
        /// </summary>
        public formDMSView DMSView
        {
            get
            {
                return mform_dmsView;
            }
            set
            {
                mform_dmsView = value;
            }
        }
        /// <summary>
        /// Gets or sets the sample queue that handles all queue management at a low level.
        /// </summary>
        public classSampleQueue SampleQueue
        {
            get
            {
                return mobj_sampleQueue;
            }
            set
            {
                mobj_sampleQueue = value;                
            }
        }
        #endregion      

        /// <summary>
        /// Adds a sample to the listview.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public bool AddSample(classSampleData sample)
        {
            bool added = true;

            if (sample == null)
            {
                added = false;
            }
            else
            {
                /// 
                /// Serialize the sample into a list view item
                /// 
                ListViewItem item = new ListViewItem();
                //item.Text = sample.Name;
					 item.SubItems.Add(sample.DmsData.RequestName);	// Name column
					 item.SubItems.Add("");	// PAL Method column
					 item.SubItems.Add("");	// Experiment column
					 item.SubItems.Add(sample.DmsData.RequestID.ToString());	// Request ID column
					
                mlistview_samples.Items.Add(item);
            }

            return added;
        }

        private void mbutton_dmsImport_Click(object sender, EventArgs e)
        {
            ShowDMSView();
        }

        /// <summary>
        /// Displays the DMS View Dialog Window.
        /// </summary>
        private void ShowDMSView()
        {
            if (mform_dmsView != null)
            {
                DialogResult result = mform_dmsView.ShowDialog();

                /// 
                /// If the user clicks ok , then add the samples from the 
                /// form into the sample queue.  Don't add them directly to the 
                /// form so that the event model will update both this view
                /// and any other views that we may have.  For the sequence
                /// we dont care how we add them to the form.                  
                /// 
                if (result == DialogResult.OK)
                {
                    List<classSampleData> samples = mform_dmsView.GetNewSamplesDMSView();
                    if (samples != null)
                    {
                        foreach (classSampleData sample in samples)
                        {
                            mobj_sampleQueue.AddSample(sample);
                        }
                    }
                    mform_dmsView.ClearForm();
                }
            }
        }        


    }
}
