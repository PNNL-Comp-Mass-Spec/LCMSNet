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

            mobj_systemData = new classSystemData();
            mdialog_color = new ColorDialog();
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the system data object.
        /// </summary>
        public classSystemData SystemData
        {
            get { return mobj_systemData; }
            set
            {
                if (value != null)
                {
                    mdialog_color.Color = value.Color;
                    mbutton_color.BackColor = value.Color;

                    int systemID = value.SystemIndex + 1;
                    mlabel_system.Text = "System " + systemID.ToString();

                    if (mobj_systemData != null)
                    {
                        mobj_systemData.ColorChanged -= SystemData_ColorChanged;
                    }
                    value.ColorChanged += new classSystemData.DelegateColorChanged(SystemData_ColorChanged);
                }
                mobj_systemData = value;
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
        private classSystemData mobj_systemData;

        /// <summary>
        /// Dialog box for displaying the choice of the color for the system.
        /// </summary>
        private ColorDialog mdialog_color;

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
                if (mobj_systemData != null)
                    mobj_systemData.Color = mdialog_color.Color;
            }
        }

        #endregion
    }
} // End namespace