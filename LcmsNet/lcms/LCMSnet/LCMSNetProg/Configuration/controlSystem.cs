//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/23/2010
//
// Last modified 02/23/2010
//                      09/01/2010 (DAC) - Reformatted for easier reading
//*********************************************************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

namespace LcmsNet.Configuration
{
    public partial class controlSystem : UserControl
    {
        #region "Constructors"

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public controlSystem()
        {
            InitializeComponent();

            m_systemData = new classSystemData();
            mdialog_color = new ColorDialog();
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the system data object.
        /// </summary>
        public classSystemData SystemData
        {
            get { return m_systemData; }
            set
            {
                if (value != null)
                {
                    mdialog_color.Color = value.Color;
                    mbutton_color.BackColor = value.Color;

                    var systemID = value.SystemIndex + 1;
                    mlabel_system.Text = "System " + systemID;

                    if (m_systemData != null)
                    {
                        m_systemData.ColorChanged -= SystemData_ColorChanged;
                    }
                    value.ColorChanged += SystemData_ColorChanged;
                }
                m_systemData = value;
            }
        }

        #endregion

        //*********************************************************************************************************
        // Control representing a cart system (2 columns)
        //**********************************************************************************************************

        #region "Class variables"

        /// <summary>
        /// Object that describes the system information.
        /// </summary>
        private classSystemData m_systemData;

        /// <summary>
        /// Dialog box for displaying the choice of the color for the system.
        /// </summary>
        private readonly ColorDialog mdialog_color;

        #endregion

        #region "Delegates"

        #endregion

        #region "Events"

        #endregion

        #region "Methods"

        /// <summary>
        /// Handles when the color is changed for a system displaying the new version.
        /// </summary>
        /// <param name="sender">System Data that called for the change.</param>
        /// <param name="previousColor">Previous color set.</param>
        /// <param name="newColor">New color set.</param>
        void SystemData_ColorChanged(object sender, Color previousColor, Color newColor)
        {
            mbutton_color.BackColor = newColor;
        }

        /// <summary>
        /// Displays the dialog box to change the color of the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_color_Click(object sender, EventArgs e)
        {
            if (mdialog_color.ShowDialog() == DialogResult.OK)
            {
                if (m_systemData != null)
                    m_systemData.Color = mdialog_color.Color;
            }
        }

        #endregion
    }
}