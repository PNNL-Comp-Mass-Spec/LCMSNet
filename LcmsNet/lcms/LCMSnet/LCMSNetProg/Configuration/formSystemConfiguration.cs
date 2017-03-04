//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche, Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 07/23/2010
//
// Updates:
// - 08/20/2010 (DAC) - Modified to save config settings during program shutdown
// - 08/31/2010 (DAC) - Changes resulting from move to LcmsNet namespace
// - 09/01/2010 (DAC) - Modified to allow operator reload of column names
// - 09/11/2014 (CJW) - Modified to use new classDMSToolsManager
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK;
using LcmsNetSQLiteTools;

namespace LcmsNet.Configuration
{

    #region "Namespace delegates"

    internal delegate void DelegateUpdateStatus(
        object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus);

    #endregion

    /// <summary>
    /// Displays application and cart configurations.
    /// </summary>
    public partial class formSystemConfiguration : Form
    {
        readonly bool m_isLoading;

        #region "Constructors"

        /// <summary>
        ///  Default constructor for displaying column data.
        /// </summary>
        public formSystemConfiguration()
        {
            m_isLoading = true;
            InitializeComponent();
            m_isLoading = false;

            Initialize();
        }

        #endregion

        public event EventHandler ColumnNameChanged;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_isLoading)
                classCartConfiguration.MinimumVolume = Convert.ToDouble(numericUpDown1.Value);
        }

        private void btnSetPdfPath_Click(object sender, EventArgs e)
        {
            var path = txtPdfPath.Text;
            if (Directory.Exists(path))
            {
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_PDFPATH, path);
            }
            else
            {
                DialogResult = MessageBox.Show("That directory does not exist, create it?", "Error",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                if (DialogResult == DialogResult.Yes)
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to create directory" + path, "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
                else
                {
                    txtPdfPath.SelectAll();
                }
            }
        }

        private void comboTimeZone_SelectedValueChanged(object sender, EventArgs e)
        {
            classLCMSSettings.SetParameter(classLCMSSettings.PARAM_TIMEZONE, comboTimeZone.SelectedItem.ToString());
        }

        private void comboDmsTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboDmsTools = sender as ComboBox;
            var selectedTool = comboDmsTools.Items[comboDmsTools.SelectedIndex].ToString();
            var toolInfo = selectedTool.Split('-');
            try
            {
                classDMSToolsManager.Instance.SelectTool(toolInfo[0], toolInfo[1]);
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_DMSTOOL, selectedTool);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                    "Unable to select dms tool: " + ex.Message);
            }
        }

        #region "Class variables"

        /// <summary>
        /// List of cart configuration names
        /// </summary>
        private List<string> m_CartConfigNames;

        /// <summary>
        /// List of available separation types
        /// </summary>
        private List<string> m_SeparationTypes;

        /// <summary>
        /// List of users
        /// </summary>
        private Dictionary<string, classUserInfo> m_Users;

        #endregion

        #region "Properties"

        /// <summary>
        /// Cart configuration names
        /// </summary>
        public List<string> CartConfigNames
        {
            set
            {
                m_CartConfigNames = value;
                mcombo_CartConfigName.Items.Clear();
                foreach (var cartConfigName in m_CartConfigNames)
                {
                    mcombo_CartConfigName.Items.Add(cartConfigName);
                }
            }
        }
        /// <summary>
        /// Sets the list of column names.
        /// </summary>
        public List<string> ColumnNames
        {
            set
            {
                mcontrol_columnOne.ColumnNames = value;
                mcontrol_columnTwo.ColumnNames = value;
                mcontrol_columnThree.ColumnNames = value;
                mcontrol_columnFour.ColumnNames = value;
            }
        }

        /// <summary>
        /// Separation types
        /// </summary>
        public List<string> SeparationTypes
        {
            set
            {
                m_SeparationTypes = value;
                mcombo_SepType.Items.Clear();
                foreach (var sepType in m_SeparationTypes)
                {
                    mcombo_SepType.Items.Add(sepType);
                }
            }
        }

        /// <summary>
        /// User list
        /// </summary>
        public List<classUserInfo> Users
        {
            set { LoadUserCombo(value); }
        }

        public string PdfPath
        {
            set { txtPdfPath.Text = value; }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Initializes data for the controls.
        /// </summary>
        private void Initialize()
        {
            mtextbox_triggerLocation.Text = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_TRIGGERFILEFOLDER);
            txtPdfPath.Text = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_PDFPATH);
            mcontrol_columnOne.ColumnData = classCartConfiguration.Columns[0];
            mcontrol_columnTwo.ColumnData = classCartConfiguration.Columns[1];
            mcontrol_columnThree.ColumnData = classCartConfiguration.Columns[2];
            mcontrol_columnFour.ColumnData = classCartConfiguration.Columns[3];

            RegisterColumn(mcontrol_columnOne);
            RegisterColumn(mcontrol_columnTwo);
            RegisterColumn(mcontrol_columnThree);
            RegisterColumn(mcontrol_columnFour);

            // Cart name
            mlabel_Cart.Text = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);

            LoadInstrumentInformation();
            LoadApplicationSettings();

            numericUpDown1.Value = Convert.ToDecimal(classCartConfiguration.MinimumVolume);

            //load time zones into combobox
            foreach (var tzi in TimeZoneInfo.GetSystemTimeZones())
            {
                comboTimeZone.Items.Add(tzi.Id);
            }

            comboTimeZone.SelectedIndex = comboTimeZone.FindStringExact(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_TIMEZONE));

            //load dms tools into combobox
            comboDmsTools.Items.AddRange(classDMSToolsManager.Instance.ListTools().ToArray());
            try
            {
                comboDmsTools.SelectedIndex = comboDmsTools.Items.IndexOf(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_DMSTOOL));
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                    "SystemConfiguration: Unable to select last selected Dms tool" + ex.Message);
                if (comboDmsTools.Items.Count > 0)
                {
                    comboDmsTools.SelectedIndex = 0;
                }
            }
        }

        void RegisterColumn(controlColumn column)
        {
            column.ColumnNamesChanged +=
                mcontrol_columnOne_ColumnNamesChanged;
        }

        void mcontrol_columnOne_ColumnNamesChanged()
        {
            ColumnNameChanged?.Invoke(this, null);
        }

        private void LoadInstrumentInformation()
        {
            // Load combo box
            var instList = classSQLiteTools.GetInstrumentList(false);

            if (instList == null)
            {
                classApplicationLogger.LogError(0, "Instrument list retrieval returned null.");
                return;
            }

            if (instList.Count < 1)
            {
                classApplicationLogger.LogError(0, "No instruments found.");
                return;
            }

            comboBoxAvailInstruments.Items.Clear();
            foreach (var instData in instList)
            {
                comboBoxAvailInstruments.Items.Add(instData.DMSName);
            }

            // Determine if presently specified instrument name is in list. If it is, display it.
            var currentName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_INSTNAME);
            var indx = 0;
            var found = false;
            foreach (string itemName in comboBoxAvailInstruments.Items)
            {
                if (itemName == currentName)
                {
                    found = true;
                    break;
                }
                indx++;
            }

            if (found)
            {
                comboBoxAvailInstruments.SelectedIndex = indx;
            }
            else comboBoxAvailInstruments.SelectedIndex = 0;


            // Determine if presently specified separation type is in list. If it is, display it.
            currentName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE);
            indx = 0;
            found = false;
            foreach (string itemName in mcombo_SepType.Items)
            {
                if (itemName == currentName)
                {
                    found = true;
                    break;
                }
                indx++;
            }

            if (found)
            {
                mcombo_SepType.SelectedIndex = indx;
            }
            else if (mcombo_SepType.Items.Count > 0)
            {
                mcombo_SepType.SelectedIndex = 0;
            }

            mbutton_accept.BackColor = Color.FromName("ButtonHighlight");
        }

        /// <summary>
        /// Loads the application settings to the user interface.
        /// </summary>
        private void LoadApplicationSettings()
        {
            //mcheckBox_createTriggerFiles.Checked = Convert.ToBoolean(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CREATETRIGGERFILES));
            //mcheckBox_copyTriggerFiles.Checked = Convert.ToBoolean(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COPYTRIGGERFILES));
            //mcheckBox_createMethodFolders.Checked = Convert.ToBoolean(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CREATEMETHODFOLDERS));
            //mcheckBox_copyMethodFolders.Checked = Convert.ToBoolean(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COPYMETHODFOLDERS));
        }

        /// <summary>
        /// Loads the user combo box
        /// </summary>
        /// <param name="userList"></param>
        private void LoadUserCombo(List<classUserInfo> userList)
        {
            // Make a dictionary based on the input list, with the first entry = "(None)"
            if (m_Users == null)
            {
                m_Users = new Dictionary<string, classUserInfo>();
            }
            else m_Users.Clear();

            // Create dummy user. User is not recognized by DMS, so that trigger files will fail and let
            //      operator know that user name was not provided
            var tmpUser = new classUserInfo();
            tmpUser.PayrollNum = "None";
            tmpUser.UserName = "(None)";
            m_Users.Add(tmpUser.PayrollNum, tmpUser);

            // Add users to dictionary from user list
            foreach (var currUser in userList)
            {
                var data = string.Format("{0} - ({1})", currUser.UserName,
                    currUser.PayrollNum);

                m_Users.Add(data, currUser);
            }

            // Now add user list to combo box
            mcombo_Operator.Items.Clear();
            foreach (var currKey in m_Users)
            {
                mcombo_Operator.Items.Add(string.Format("{0} - ({1})",
                    currKey.Value.UserName,
                    currKey.Value.PayrollNum));
            }

            // Set combo box to display first entry
            mcombo_Operator.SelectedIndex = 0;


            // Determine if presently specified operator name is in list. If it is, display it.
            var currentName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_OPERATOR);
            var indx = 0;
            var found = false;
            foreach (string itemName in mcombo_Operator.Items)
            {
                var array = itemName.Split('-');
                if (array[0].Trim() == currentName)
                {
                    found = true;
                    break;
                }
                indx++;
            }

            if (found)
            {
                mcombo_Operator.SelectedIndex = indx;
            }
            else if (mcombo_Operator.Items.Count > 0)
            {
                mcombo_Operator.SelectedIndex = 0;
            }
            mbutton_acceptOperator.BackColor = Color.FromName("ButtonHighlight");
        }

        /// <summary>
        /// Sets the cart configuration name in use
        /// </summary>
        /// <param name="cartConfigName">Cart configuration name</param>
        public void SetCartConfigName(string cartConfigName)
        {
            var indx = mcombo_CartConfigName.FindString(cartConfigName, -1);
            if (indx == -1 && mcombo_CartConfigName.Items.Count > 0)
            {
                mcombo_CartConfigName.SelectedIndex = 0;
            }
            else
            {
                mcombo_CartConfigName.SelectedIndex = indx;
            }
        }

        /// <summary>
        /// Sets the separation type
        /// </summary>
        /// <param name="separationType">New separation type</param>
        public void SetSeparationType(string separationType)
        {
            var indx = mcombo_SepType.FindString(separationType, -1);
            if (indx == -1 && mcombo_SepType.Items.Count > 0)
            {
                mcombo_SepType.SelectedIndex = 0;
            }
            else
            {
                mcombo_SepType.SelectedIndex = indx;
            }
        }

        #endregion

        #region Column events

        ///// <summary>
        ///// Handles the event when the column status is changed.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="previousStatus"></param>
        ///// <param name="newStatus"></param>
        //void column_StatusChanged(object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus)
        //{
        //    if (InvokeRequired == true)
        //    {
        //        BeginInvoke(new DelegateUpdateStatus(StatusChanged), new object[] { sender, previousStatus, newStatus });
        //    }
        //    else
        //    {
        //        StatusChanged(sender, previousStatus, newStatus);
        //    }
        //}

        #endregion

        #region Form Events

        private void mcombo_CartConfigName_SelectedIndexChanged(object sender, EventArgs e)
        {
            classLCMSSettings.SetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME, mcombo_CartConfigName.Text);
            classSQLiteTools.SaveSelectedCartConfigName(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME));
        }

        private void mcombo_SepType_SelectedIndexChanged(object sender, EventArgs e)
        {
            classLCMSSettings.SetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE, mcombo_SepType.Text);
            classSQLiteTools.SaveSelectedSeparationType(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE));
        }

        private void mbutton_Reload_Click(object sender, EventArgs e)
        {

            // Instruct the selected tool to get a fresh list of columns from DMS and store it in the cache db
            try
            {
                classDMSToolsManager.Instance.SelectedTool.GetColumnListFromDMS();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            UpdateCartConfigNames();

            UpdateColumnNameLists();
        }

        private void UpdateCartConfigNames()
        {
            List<string> cartConfigNameList = null;
            try
            {
                // Get the new cart config names from the cache db
                cartConfigNameList = classSQLiteTools.GetCartConfigNameList(true);
            }
            catch (classDatabaseDataException ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            // If a valid list was received, update the display
            if (cartConfigNameList == null)
            {
                // No new cart config names were obtained
                classApplicationLogger.LogError(0, "Cart config name list null when refreshing list");
                MessageBox.Show("List not updated. Cart config name list from DMS is null");
                return;
            }

            if (cartConfigNameList.Count < 1)
            {
                // No names found in list
                classApplicationLogger.LogError(0, "No cart config names found when refreshing list");
                MessageBox.Show("List not updated. No cart config names were found.");
                return;
            }

            mcombo_CartConfigName.Items.Clear();
            if (cartConfigNameList.Any())
            {
                foreach (var cartConfig in cartConfigNameList)
                {
                    mcombo_CartConfigName.Items.Add(cartConfig);
                }
            }

            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                "Cart config name lists updated");
        }

        private void UpdateColumnNameLists()
        {

            List<string> columnList = null;
            try
            {
                // Get the new list of columns from the cache db
                columnList = classSQLiteTools.GetColumnList(true);
            }
            catch (classDatabaseDataException ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
            }

            // If a valid list was received, update the display
            if (columnList == null)
            {
                // No new column list obtained
                classApplicationLogger.LogError(0, "Column name list null when refreshing list");
                MessageBox.Show("List not updated. Column name list from DMS is null");
                return;
            }
            if (columnList.Count < 1)
            {
                // No names found in list
                classApplicationLogger.LogError(0, "No column names found when refreshing list");
                MessageBox.Show("List not updated. No column names found.");
                return;
            }

            // Everything was OK, so update the list
            mcontrol_columnOne.ColumnNames = columnList;
            mcontrol_columnTwo.ColumnNames = columnList;
            mcontrol_columnThree.ColumnNames = columnList;
            mcontrol_columnFour.ColumnNames = columnList;

            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                "Column name lists updated");
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            if (!m_isLoading)
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_INSTNAME, comboBoxAvailInstruments.SelectedItem.ToString());
            mbutton_accept.BackColor = Color.FromName("ButtonHighlight");
        }

        private void comboBoxAvailInstruments_SelectedIndexChanged(object sender, EventArgs e)
        {
            mbutton_accept.BackColor = Color.Red;
        }

        private void mbutton_acceptOperator_Click(object sender, EventArgs e)
        {
            if (!m_isLoading)
            {
                var operatorName = mcombo_Operator.SelectedItem.ToString();
                if (m_Users.ContainsKey(operatorName))
                {
                    var instrumentOperator = m_Users[operatorName];
                    classLCMSSettings.SetParameter(classLCMSSettings.PARAM_OPERATOR, instrumentOperator.UserName);
                }
                else
                {
                    classApplicationLogger.LogError(0,
                        "Could not use the current user as the operator.  Was not present in the system.");
                }
            }
            mbutton_acceptOperator.BackColor = Color.FromName("ButtonHighlight");
        }

        private void mcombo_Operator_SelectedIndexChanged(object sender, EventArgs e)
        {
            mbutton_acceptOperator.BackColor = Color.Red;
        }

        #endregion
    }
}