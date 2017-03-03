//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 08/18/2010
//
// Last modified 08/18/2010
//                      09/01/2010 (DAC) - Modified for autocomplete when selecting column name. Fixed bug
//                                                  that wasn't allowing changes to be passed to column object
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.Configuration
{
    public partial class controlColumn : UserControl
    {
        #region "Constructors"

        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlColumn()
        {
            InitializeComponent();
            m_columnData = new classColumnData();
            mdialog_color = new ColorDialog();

            mcomboBox_names.SelectedIndexChanged += mcomboBox_names_SelectedIndexChanged;
            ColumnObjectChanged += SelectColumnNameFromData;
            ColumnNamesChanged += SelectColumnNameFromData;
        }

        #endregion

        //*********************************************************************************************************
        // Control for displaying information about the column.
        //**********************************************************************************************************

        #region "Class variables"

        /// <summary>
        /// Column configuration object.
        /// </summary>
        private classColumnData m_columnData;

        /// <summary>
        /// Dialog box for selecting the color of the column.
        /// </summary>
        private readonly ColorDialog mdialog_color;

        /// <summary>
        /// Displays the ID - index of the column.
        /// </summary>
        private int m_columnID;

        /// <summary>
        /// Flag telling combo box index changed event that event was caused by a mouse click
        /// </summary>
        private bool m_ComboHasFocus;

        #endregion

        #region "Delegates"

        public delegate void delegateColumnNamesChanged();

        private delegate void delegateColumnObjectChanged();

        #endregion

        #region "Events"

        public event delegateColumnNamesChanged ColumnNamesChanged;
        private event delegateColumnObjectChanged ColumnObjectChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the data associated with the column.
        /// </summary>
        public classColumnData ColumnData
        {
            get { return m_columnData; }
            set
            {
                //
                // Reset the enabled
                //

                m_columnData = value;
                if (value != null)
                {
                    UpdateUserInterface();
                    value.ColorChanged += ColumnData_ColorChanged;
                    value.StatusChanged += ColumnData_StatusChanged;
                }
                // Signal the combo box to update selected name
                ColumnObjectChanged?.Invoke();
            }
        }

        /// <summary>
        /// Gets or sets the column index ID for display.
        /// </summary>
        public int ColumnID
        {
            get { return m_columnID; }
            set
            {
                m_columnID = value;
                mbutton_color.Text = m_columnID.ToString();
                PerformLayout();
            }
        }

        /// <summary>
        /// Sets the list of column names.
        /// </summary>
        public List<string> ColumnNames
        {
            set
            {
                mcomboBox_names.Items.Clear();
                var names = new string[value.Count];
                value.CopyTo(names);
                mcomboBox_names.BeginUpdate();
                mcomboBox_names.Items.Add("NOTSET");
                mcomboBox_names.Items.AddRange(names);
                mcomboBox_names.AutoCompleteSource = AutoCompleteSource.ListItems;
                mcomboBox_names.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                mcomboBox_names.EndUpdate();
                // Update the selected name in the combo box
                ColumnNamesChanged?.Invoke();
            }
        }

        #endregion

        #region Column Data Event Handlers

        /// <summary>
        /// Handles when the status for a column changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousStatus"></param>
        /// <param name="newStatus"></param>
        void ColumnData_StatusChanged(object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateUpdateStatus(SetStatusMessage), sender, previousStatus, newStatus);
            }
            else
            {
                SetStatusMessage(sender, previousStatus, newStatus);
            }
        }

        /// <summary>
        /// Handles when the color of a column changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousColor"></param>
        /// <param name="newColor"></param>
        void ColumnData_ColorChanged(object sender, Color previousColor, Color newColor)
        {
            SetColor(newColor);
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Displays the appropiate status message.
        /// </summary>
        private void SetStatusMessage(object sender, enumColumnStatus previousStatus, enumColumnStatus status)
        {
            mlabel_status.Text = string.Format("Status: {0}", status);

            var statusMessage = string.Format("Status: {0}", status);
            //TODO: change this magic number into a constant.
            classApplicationLogger.LogMessage(1, statusMessage);
        }

        /// <summary>
        /// Sets the color of the column.
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(Color color)
        {
            mbutton_color.BackColor = color;
            mdialog_color.Color = color;
        }

        /// <summary>
        /// Updates the display with accurate data.
        /// </summary>
        private void UpdateUserInterface()
        {
            if (m_columnData == null)
                return;

            SetColor(m_columnData.Color);
            SetStatusMessage(this, m_columnData.Status, m_columnData.Status);

            if (m_columnData.Status != enumColumnStatus.Disabled)
                mcheckBox_enabled.Checked = true;
            else
                mcheckBox_enabled.Checked = false;
        }

        /// <summary>
        /// Sets the column name combo box to match the value contained in the column data class
        /// </summary>
        private void SelectColumnNameFromData()
        {
            if (mcomboBox_names.Items.Count < 1) return; // Do nothing because the names haven't been loaded yet

            if ((m_columnData == null) || (m_columnData.Name == null) || (m_columnData.Name == ""))
                return; // No column name specified

            var nameIndx = mcomboBox_names.FindStringExact(m_columnData.Name);

            if (nameIndx == -1)
            {
                // Name match not found, set name to first available and exit
                mcomboBox_names.SelectedIndex = 0;
                return;
            }

            mcomboBox_names.SelectedIndex = nameIndx;
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Handles when the user clicks to change the color of the column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_color_Click(object sender, EventArgs e)
        {
            if (mdialog_color.ShowDialog() == DialogResult.OK)
            {
                m_columnData.Color = mdialog_color.Color;
            }
        }

        /// <summary>
        /// Handles when the user toggles the check box to enable or disable the column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcheckBox_enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (m_columnData != null)
            {
                if (mcheckBox_enabled.Checked == false)
                    m_columnData.Status = enumColumnStatus.Disabled;
                if (mcheckBox_enabled.Checked)
                    m_columnData.Status = enumColumnStatus.Idle;
            }
        }

        /// <summary>
        /// Handles change of selected item in combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mcomboBox_names_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_columnData != null)
            {
                // If selected index changed due to user action, then update the column data
                //  Otherwise, index may have changed because of initializing the combo box and should be ignored
                if (m_ComboHasFocus)
                {
                    m_columnData.Name = mcomboBox_names.SelectedItem.ToString();
                    ColumnNamesChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Sets flag to indicate combo box has focus (Used in SelectedIndexChanged method)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcomboBox_names_Enter(object sender, EventArgs e)
        {
            m_ComboHasFocus = true;
        }

        /// <summary>
        /// Clears flag indicating combo box focus (Used in SelectedIndexChanged method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcomboBox_names_Leave(object sender, EventArgs e)
        {
            m_ComboHasFocus = false;
        }

        #endregion
    }
}