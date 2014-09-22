//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
/* Last modified 01/16/2009
 * 
 *      1/16/2009:  Brian LaMarche
 *      
 * 
 */
/*********************************************************************************************************/
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Method;

namespace LcmsNet.SampleQueue.Forms
{
    /// <summary>
    /// Control for displaying samples in a column base fashion.
    /// </summary>
    public sealed partial class controlColumnView : controlSampleView
    {

        public delegate void DelegateUpdateUserInterface();

        #region Members
        /// <summary>
        /// Data object reference to synchronize column data with.
        /// </summary>
        private classColumnData mobj_columnData;
        #endregion

        private List<Button> mlist_buttons;
        formExpansion mform_expand;
        private const float CONST_COLUMN_HEADER_FONT_SIZE = 18.0F;        

        #region Constructors
        /// <summary>
        /// Constructor that 
        /// </summary>
        public controlColumnView(formDMSView dmsView, classSampleQueue sampleQueue):
             base(dmsView, sampleQueue)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();
            mobj_columnData    = new classColumnData();
            mobj_columnData.ID = -1;

            DisplayColumn(CONST_COLUMN_STATUS, false);
            DisplayColumn(CONST_COLUMN_COLUMN_ID, false);
            DisplayColumn(CONST_COLUMN_DATASET_TYPE, false);
            DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, false);
            DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, false);            
            DisplayColumn(CONST_COLUMN_PAL_TRAY, false);
            DisplayColumn(CONST_COLUMN_PAL_VIAL, false);
            DisplayColumn(CONST_COLUMN_VOLUME, false);
            mdataGrid_samples.SpecialPaint += new DelegateOnPaint(mdataGrid_samples_SpecialPaint);
            InitializeButtons();
            ShrinkLeftPanel();
            mlabel_columnNameHeader.SendToBack();
            mpanel_control.SendToBack();
            PerformLayout();
            EnableQueueing(false);
            
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlColumnView()
        {
            InitializeComponent();

            ColumnHandling = enumColumnDataHandling.CreateUnused;

            try
            {
                mobj_columnData = new classColumnData();
                mobj_columnData.ID = -1;


                DisplayColumn(CONST_COLUMN_STATUS, false);
                DisplayColumn(CONST_COLUMN_COLUMN_ID, false);
                DisplayColumn(CONST_COLUMN_DATASET_TYPE, false);
                DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, true);
                DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, false);
                DisplayColumn(CONST_COLUMN_PAL_TRAY, false);
                DisplayColumn(CONST_COLUMN_PAL_VIAL, false);
                DisplayColumn(CONST_COLUMN_VOLUME, false);
                mdataGrid_samples.SpecialPaint += new DelegateOnPaint(mdataGrid_samples_SpecialPaint);
                InitializeButtons();
                ShrinkLeftPanel();
                mlabel_columnNameHeader.SendToBack();
                mpanel_control.SendToBack();
                PerformLayout();

                Resize += new EventHandler(controlColumnView_Resize);
            }
            catch { }
        }

        private const int CONST_BUTTON_PADDING = 2;
        private List<Button> mlist_expansionList;

        void controlColumnView_Resize(object sender, EventArgs e)
        {
            UpdateExpandButtonList();
        }
        
        private void UpdateExpandButtonList()
        {
            mlist_expansionList.Clear();
            
            int width    = 60;
            int leftmost = Width - mbutton_expand.Width - CONST_BUTTON_PADDING;
            int padding  = CONST_BUTTON_PADDING;
            int left     = padding;
            for (int i = 0; i < mlist_buttons.Count; i++)
            {
                Button button = mlist_buttons[i];
                
                int widthLeft = left + width + CONST_BUTTON_PADDING;

                if (widthLeft >= leftmost)
                {
                    button.Width = width;
                    mlist_expansionList.Add(button);
                    if (mpanel_control.Controls.Contains(button))
                    {
                        mpanel_control.Controls.Remove(button);
                    }
                }
                else
                {
                    if (!mpanel_control.Controls.Contains(button))
                    {
                        mpanel_control.Controls.Add(button);
                    }
                    button.Top      = mbutton_expand.Top;
                    button.Height   = mbutton_expand.Height;
                    button.Left     = left;
                    button.Width    = width;
                }
                left += (width + padding);
            }
            PerformLayout();
           
        }

        private void InitializeButtons()
        {
            mform_expand        = new formExpansion();
            mlist_buttons       = new List<Button>();
            mlist_expansionList = new List<Button>();
            mlist_buttons.Add(mbutton_addBlank);
            mlist_buttons.Add(mbutton_addBlankAppend);
            mlist_buttons.Add(mbutton_addDMS);
            mlist_buttons.Add(mbutton_removeSelected);
            mlist_buttons.Add(mbutton_deleteUnused);
            mlist_buttons.Add(mbutton_up);
            mlist_buttons.Add(mbutton_down);
            mlist_buttons.Add(mbutton_moveColumns);
            mlist_buttons.Add(mbutton_fillDown);
            mlist_buttons.Add(mbutton_trayVial);
            mlist_buttons.Add(mbutton_randomize);
            mlist_buttons.Add(mbutton_cartColumnDate);
            mlist_buttons.Add(mbutton_dmsEdit);

            UpdateExpandButtonList();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the column number id for this control.
        /// </summary>
        public classColumnData Column
        {
            get
            {
                return mobj_columnData;
            }
            set
            {
                if (value == null)
                    return;
                
                if (mobj_columnData != null)
                {
                    mobj_columnData.ColorChanged  -= ColumnData_ColorChanged;
                    mobj_columnData.StatusChanged -= ColumnData_StatusChanged;
                    mobj_columnData.NameChanged   -= ColumnData_NameChanged;
                }

                mobj_columnData = value;
                mobj_columnData.ColorChanged += new classColumnData.DelegateColorChanged(ColumnData_ColorChanged);
                mobj_columnData.StatusChanged += new classColumnData.DelegateStatusChanged(ColumnData_StatusChanged);
                mobj_columnData.NameChanged += new classColumnData.DelegateNameChanged(ColumnData_NameChanged);
                
                SetColumnStatus();
                
                /// 
                /// Color 
                /// 
                //BackColor = mobj_columnData.Color;
                BackColor = Color.White;
                mlabel_columnNameHeader.Text = "Column: " + mobj_columnData.ID.ToString() + " " + mobj_columnData.Name;
            }
        }
        #endregion

        #region Column Event Handlers and Methods        
        /// <summary>
        /// Updates the screen with the new name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        private void ColumnData_NameChanged(object sender, string name, string oldName)
        {
            mdataGrid_samples.Refresh();
        }
        /// <summary>
        /// Sets the user controls status
        /// </summary>
        private void SetColumnStatus()
        {
            /// 
            /// Status updates 
            /// 
            if (mobj_columnData.Status == enumColumnStatus.Disabled)
            {
                this.Enabled = false;
                BackColor = Color.DarkGray;
            }
            else
            {
                this.Enabled = true;
                BackColor = Color.White;
            }
        }
        /// <summary>
        /// Handles the event when the columns status changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousStatus"></param>
        /// <param name="newStatus"></param>
        void ColumnData_StatusChanged(object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus)
        {
            if (sender == mobj_columnData)
            {
                if (InvokeRequired == true)
                    BeginInvoke(new DelegateUpdateUserInterface(SetColumnStatus));
                else
                    SetColumnStatus();
            }
        }
        /// <summary>
        /// Updates the back color of this control when the color definition for the column changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousColor"></param>
        /// <param name="newColor"></param>
        void ColumnData_ColorChanged(object sender, Color previousColor, Color newColor)
        {
            /// 
            /// Make sure we have the right event.
            /// 
            if (sender == mobj_columnData)
            {
                if (mobj_columnData.Status != enumColumnStatus.Disabled)
                {
                 //   BackColor = newColor;
                }
            }
        }
        #endregion

        #region Sample Queue Management Interface Methods
        /// <summary>
        /// Adds the specified samples ot this column.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        protected override void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
        {                   
            /// 
            /// For every sample, add the column data to it, then add it into the manager.
            /// We dont add to our list first so the manager can verify the sample and 
            /// make sure we dont have duplicates.
            /// 
            foreach (classSampleData sample in samples)
                sample.ColumnData = mobj_columnData;

            if (insertIntoUnused == false)
            {
                mobj_sampleQueue.QueueSamples(samples, ColumnHandling);
            }
            else
            {
                mobj_sampleQueue.InsertIntoUnusedSamples(samples, mobj_columnData, ColumnHandling);
            }
        }
        /// <summary>
        /// Override adding samples to the list to first check to make sure the sample belongs on this column.        
        /// </summary>
        /// <param name="sample">Sample to add.</param>
        /// <returns>True if added, and false if the data does not belong on this column.</returns>
        protected override bool AddSamplesToList(classSampleData sample)
        {
            if (sample.ColumnData.ID == mobj_columnData.ID)
                return base.AddSamplesToList(sample);            
            return false;
        }
        /// <summary>
        /// Moves the selected samples by the offset.  First calculates how far to move the samples on this 
        /// column by finding out how many columns are enabled in the configuration.
        /// </summary>
        /// <param name="offset">Amount to move the samples (-1 for lower sequence numbers) (1 for higher sequence numbers)</param>
        protected override void MoveSelectedSamples(int offset, enumMoveSampleType moveType)
        {
            int numEnabledColumns = classCartConfiguration.NumberOfEnabledColumns;

            /// 
            /// We are moving the sample by N in the queue to offset for the enabled / disabled columns.
            /// 
            offset *= numEnabledColumns;
            base.MoveSelectedSamples(offset, enumMoveSampleType.Column);
        }
        /// <summary>
        /// Removes the samples but does not resort the columns
        /// </summary>
        /// <param name="resortColumns"></param>
        protected override void RemoveSelectedSamples(enumColumnDataHandling resortColumns)
        {
            /// Override the resort columns flag
            base.RemoveSelectedSamples(resortColumns);
        }
        /// <summary>
        /// Handles removing unused samples from the sample queue only for this column.
        /// </summary>
        protected override void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            mobj_sampleQueue.RemoveUnusedSamples(mobj_columnData, resortColumns);
        }
        /// <summary>
        /// Returns whether the sample queue has any unused samples on this column.
        /// </summary>
        /// <returns>True if an unused sample exists.</returns>
        protected override bool HasUnusedSamples()
        {
            return mobj_sampleQueue.HasUnusedSamples(mobj_columnData);
        }
        #endregion              

        /// <summary>
        /// Paints the column name on the left side of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mdataGrid_samples_SpecialPaint(object sender, PaintEventArgs e)
        {
            if (mobj_columnData != null)
            {
                string name = mobj_columnData.Name;
                mlabel_columnNameHeader.Text = name;
            }
        }

        #region Sample Queue Override Event Methods
        /// <summary>
        /// Handles when a sample is updated....updating the ones that need to be
        /// then deleting the ones...WTF?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected override void SamplesUpdated(object sender, classSampleQueueArgs data)
        {
           //if (InvokeRequired == true)
            {
                /// 
                /// Update the samples
                /// 
                base.SamplesUpdated(sender, data);

                /// 
                /// Then delete the rows that are not part of this column.
                /// 
                foreach (classSampleData sample in data.Samples)
                {
                    /// 
                    /// Remove if it it does not belong to this column,
                    /// 
                    if (sample.ColumnData.ID != mobj_columnData.ID)
                    {
                        RemoveSamplesFromList(sample);
                    }
                    /// 
                    /// Otherwise add it if it does not exist.
                    /// 
                    else
                    {
                        int index = FindRowIndexFromUID(sample.UniqueID);
                        if (index < 0)
                        {
                            AddSamplesToList(sample);
                        }
                    }
                }
            }
        }
        #endregion

        #region Method Manager Events
        protected override bool CanAddLCMethodToItem(classLCMethod method)
        {
            if (method.Column > -1 && method.Column == mobj_columnData.ID)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        protected override void AddLCMethod(classLCMethod method)
        {
            if (method.Column > -1 && method.Column == mobj_columnData.ID && !method.IsSpecialMethod)
            {
                base.AddLCMethod(method);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected override bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            AddLCMethod(method);
            return true;             
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected override bool Manager_MethodUpdated(object sender, classLCMethod method)
        {
            if (method == null)
                return false;

            bool contains = ContainsMethod(method.Name);
            if (contains)
            {
                if (method.Column != mobj_columnData.ID)
                {
                    // Remove the method from the drop down list for this guy.
                    RemoveMethodName(method.Name);

                    //// Also remove the reference to that method in the waiting queue.
                    //List<classSampleData> samples = mobj_sampleQueue.GetWaitingQueue();
                    //foreach (classSampleData sample in samples)
                    //{
                    //    if (sample.LCMethod != null && sample.LCMethod.Name == method.Name)
                    //    {
                    //        sample.LCMethod = null;
                    //    }
                    //}
                    ////TODO: Is this the right place to do this.
                    //UpdateRows(samples);
                }
            }
            else
            {
                AddLCMethod(method);
            }
            return true;
        }
        #endregion 

        private void mbutton_addBlank_Click(object sender, EventArgs e)
        {

            AddNewSample(true);
        }

        private void mbutton_addDMS_Click(object sender, EventArgs e)
        {
            ShowDMSView();
        }

        private void mbutton_removeUnused_Click(object sender, EventArgs e)
        {
            RemoveUnusedSamples(enumColumnDataHandling.CreateUnused);
        }

        private void mbutton_removeSelected_Click(object sender, EventArgs e)
        {
            RemoveSelectedSamples(ColumnHandling);
        }

        private void mbutton_ccd_Click(object sender, EventArgs e)
        {
            AddDateCartnameColumnIDToDatasetName();
        }

        private void mbutton_fillDown_Click(object sender, EventArgs e)
        {
            FillDown();
        }

        private void mbutton_trayVial_Click(object sender, EventArgs e)
        {
            EditTrayAndVial();
        }

        private void mbutton_randomize_Click(object sender, EventArgs e)
        {
            RandomizeSelectedSamples();
        }

        private void mbutton_moveColumns_Click(object sender, EventArgs e)
        {
            MoveSamplesToColumn(enumColumnDataHandling.CreateUnused);
        }

        private void ShowExpansion()
        {
            Point buttonScreen              = PointToScreen(mbutton_expand.Location);
            Point controlScreen             = PointToScreen(mpanel_control.Location);
            mform_expand.StartPosition      = FormStartPosition.Manual;
            mform_expand.Location           = new Point(buttonScreen.X, controlScreen.Y + mbutton_expand.Top);
            mform_expand.Refresh();
            mform_expand.UpdateButtons(mlist_expansionList);
            mform_expand.Height = mpanel_control.Height;
            int width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;

            int expandWidth = mform_expand.Left + mform_expand.Width;

            if (expandWidth > width)
            {
                Point p               = mform_expand.Location;
                p.X                   = width - mform_expand.Width;
                mform_expand.Location = p;
            }
            

            DialogResult result             = mform_expand.ShowDialog();            
            mform_expand.Hide();
        }
        private void mbutton_expand_Click(object sender, EventArgs e)
        {
            ShowExpansion();
        }

        private void mbuttonAddEndBlank_click(object sender, EventArgs e)
        {
            AddNewSample(false);
        }

        private void mbutton_down_Click(object sender, EventArgs e)
        {
            MoveSelectedSamples(1, enumMoveSampleType.Column);
        }

        private void mbutton_up_Click(object sender, EventArgs e)
        {
            MoveSelectedSamples(-1, enumMoveSampleType.Column);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddNewSample(false);
        }

        private void mbutton_expand_MouseHover(object sender, EventArgs e)
        {
            ShowExpansion();
        }

        private void mbutton_addBlankAppend_Click(object sender, EventArgs e)
        {
            AddNewSample(false);
        }

        private void mbutton_dmsEdit_Click(object sender, EventArgs e)
        {
            EditDMSData();
        }

        private void mbutton_cartColumnDate_Click(object sender, EventArgs e)
        {
            AddDateCartnameColumnIDToDatasetName();
        }

    }
}
