using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.SampleQueue
{
    partial class formDMSView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(formDMSView));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBoxSelectCartConfig = new System.Windows.Forms.ComboBox();
            this.labelLCCartConfig = new System.Windows.Forms.Label();
            this.labelRequestCount = new System.Windows.Forms.Label();
            this.buttonUpdateCartList = new System.Windows.Forms.Button();
            this.labelPleaseWait = new System.Windows.Forms.Label();
            this.comboBoxSelectCart = new System.Windows.Forms.ComboBox();
            this.listviewAvailableRequests = new System.Windows.Forms.ListView();
            this.columnHeaderAvailRequestsReqName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAvailRequestsReqNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAvailRequestsCart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAvailRequestsUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAvailRequestsUsageType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAvailRequestsBatch = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAvailRequestsBlock = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAvailRequestsRunOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textWellplate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBlock = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBatchID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxUnAssignedOnly = new System.Windows.Forms.CheckBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.comboBoxCarts = new System.Windows.Forms.ComboBox();
            this.textRequestNumMax = new System.Windows.Forms.TextBox();
            this.textRequestNumMin = new System.Windows.Forms.TextBox();
            this.textRequestName = new System.Windows.Forms.TextBox();
            this.labelCarts = new System.Windows.Forms.Label();
            this.labelRequestsTo = new System.Windows.Forms.Label();
            this.labelRequestsFrom = new System.Windows.Forms.Label();
            this.labelRequestName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.labelLCCart = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listViewRequestsToRun = new System.Windows.Forms.ListView();
            this.columnHeaderRequestsToRunReqName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRequestsToRunReqNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRequestsToRunCart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRequestsToRunUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRequestsToRunUsageType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRequestsToRunBatch = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRequestsToRunBlock = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRequestsToRunRunOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.comboBoxSelectCartConfig);
            this.panel2.Controls.Add(this.labelLCCartConfig);
            this.panel2.Controls.Add(this.labelRequestCount);
            this.panel2.Controls.Add(this.buttonUpdateCartList);
            this.panel2.Controls.Add(this.labelPleaseWait);
            this.panel2.Controls.Add(this.comboBoxSelectCart);
            this.panel2.Controls.Add(this.listviewAvailableRequests);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.buttonMoveDown);
            this.panel2.Controls.Add(this.buttonMoveUp);
            this.panel2.Controls.Add(this.labelLCCart);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // comboBoxSelectCartConfig
            // 
            this.comboBoxSelectCartConfig.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxSelectCartConfig, "comboBoxSelectCartConfig");
            this.comboBoxSelectCartConfig.Name = "comboBoxSelectCartConfig";
            this.comboBoxSelectCartConfig.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectCartConfig_SelectedIndexChanged);
            // 
            // labelLCCartConfig
            // 
            resources.ApplyResources(this.labelLCCartConfig, "labelLCCartConfig");
            this.labelLCCartConfig.Name = "labelLCCartConfig";
            // 
            // labelRequestCount
            // 
            resources.ApplyResources(this.labelRequestCount, "labelRequestCount");
            this.labelRequestCount.Name = "labelRequestCount";
            // 
            // buttonUpdateCartList
            // 
            resources.ApplyResources(this.buttonUpdateCartList, "buttonUpdateCartList");
            this.buttonUpdateCartList.Name = "buttonUpdateCartList";
            this.buttonUpdateCartList.UseVisualStyleBackColor = true;
            this.buttonUpdateCartList.Click += new System.EventHandler(this.buttonUpdateCartList_Click);
            // 
            // labelPleaseWait
            // 
            this.labelPleaseWait.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.labelPleaseWait, "labelPleaseWait");
            this.labelPleaseWait.Name = "labelPleaseWait";
            // 
            // comboBoxSelectCart
            // 
            this.comboBoxSelectCart.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxSelectCart, "comboBoxSelectCart");
            this.comboBoxSelectCart.Name = "comboBoxSelectCart";
            this.comboBoxSelectCart.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectCart_SelectedIndexChanged);
            // 
            // listviewAvailableRequests
            // 
            resources.ApplyResources(this.listviewAvailableRequests, "listviewAvailableRequests");
            this.listviewAvailableRequests.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderAvailRequestsReqName,
            this.columnHeaderAvailRequestsReqNum,
            this.columnHeaderAvailRequestsCart,
            this.columnHeaderAvailRequestsUser,
            this.columnHeaderAvailRequestsUsageType,
            this.columnHeaderAvailRequestsBatch,
            this.columnHeaderAvailRequestsBlock,
            this.columnHeaderAvailRequestsRunOrder});
            this.listviewAvailableRequests.FullRowSelect = true;
            this.listviewAvailableRequests.GridLines = true;
            this.listviewAvailableRequests.Name = "listviewAvailableRequests";
            this.listviewAvailableRequests.UseCompatibleStateImageBehavior = false;
            this.listviewAvailableRequests.View = System.Windows.Forms.View.Details;
            this.listviewAvailableRequests.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listviewAvailableRequests_ColumnClick);
            // 
            // columnHeaderAvailRequestsReqName
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsReqName, "columnHeaderAvailRequestsReqName");
            // 
            // columnHeaderAvailRequestsReqNum
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsReqNum, "columnHeaderAvailRequestsReqNum");
            // 
            // columnHeaderAvailRequestsCart
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsCart, "columnHeaderAvailRequestsCart");
            // 
            // columnHeaderAvailRequestsUser
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsUser, "columnHeaderAvailRequestsUser");
            // 
            // columnHeaderAvailRequestsUsageType
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsUsageType, "columnHeaderAvailRequestsUsageType");
            // 
            // columnHeaderAvailRequestsBatch
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsBatch, "columnHeaderAvailRequestsBatch");
            // 
            // columnHeaderAvailRequestsBlock
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsBlock, "columnHeaderAvailRequestsBlock");
            // 
            // columnHeaderAvailRequestsRunOrder
            // 
            resources.ApplyResources(this.columnHeaderAvailRequestsRunOrder, "columnHeaderAvailRequestsRunOrder");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textWellplate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBlock);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBatchID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.checkBoxUnAssignedOnly);
            this.groupBox1.Controls.Add(this.buttonFind);
            this.groupBox1.Controls.Add(this.comboBoxCarts);
            this.groupBox1.Controls.Add(this.textRequestNumMax);
            this.groupBox1.Controls.Add(this.textRequestNumMin);
            this.groupBox1.Controls.Add(this.textRequestName);
            this.groupBox1.Controls.Add(this.labelCarts);
            this.groupBox1.Controls.Add(this.labelRequestsTo);
            this.groupBox1.Controls.Add(this.labelRequestsFrom);
            this.groupBox1.Controls.Add(this.labelRequestName);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textWellplate
            // 
            resources.ApplyResources(this.textWellplate, "textWellplate");
            this.textWellplate.Name = "textWellplate";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBlock
            // 
            resources.ApplyResources(this.textBlock, "textBlock");
            this.textBlock.Name = "textBlock";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBatchID
            // 
            resources.ApplyResources(this.textBatchID, "textBatchID");
            this.textBatchID.Name = "textBatchID";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // checkBoxUnAssignedOnly
            // 
            resources.ApplyResources(this.checkBoxUnAssignedOnly, "checkBoxUnAssignedOnly");
            this.checkBoxUnAssignedOnly.Name = "checkBoxUnAssignedOnly";
            this.checkBoxUnAssignedOnly.UseVisualStyleBackColor = true;
            this.checkBoxUnAssignedOnly.Click += new System.EventHandler(this.checkBoxUnAssignedOnly_CheckedChanged);
            // 
            // buttonFind
            // 
            this.buttonFind.Image = global::LcmsNet.Properties.Resources.Search;
            resources.ApplyResources(this.buttonFind, "buttonFind");
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // comboBoxCarts
            // 
            this.comboBoxCarts.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCarts, "comboBoxCarts");
            this.comboBoxCarts.Name = "comboBoxCarts";
            // 
            // textRequestNumMax
            // 
            resources.ApplyResources(this.textRequestNumMax, "textRequestNumMax");
            this.textRequestNumMax.Name = "textRequestNumMax";
            // 
            // textRequestNumMin
            // 
            resources.ApplyResources(this.textRequestNumMin, "textRequestNumMin");
            this.textRequestNumMin.Name = "textRequestNumMin";
            // 
            // textRequestName
            // 
            resources.ApplyResources(this.textRequestName, "textRequestName");
            this.textRequestName.Name = "textRequestName";
            // 
            // labelCarts
            // 
            resources.ApplyResources(this.labelCarts, "labelCarts");
            this.labelCarts.Name = "labelCarts";
            // 
            // labelRequestsTo
            // 
            resources.ApplyResources(this.labelRequestsTo, "labelRequestsTo");
            this.labelRequestsTo.Name = "labelRequestsTo";
            // 
            // labelRequestsFrom
            // 
            resources.ApplyResources(this.labelRequestsFrom, "labelRequestsFrom");
            this.labelRequestsFrom.Name = "labelRequestsFrom";
            // 
            // labelRequestName
            // 
            resources.ApplyResources(this.labelRequestName, "labelRequestName");
            this.labelRequestName.Name = "labelRequestName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonMoveDown
            // 
            resources.ApplyResources(this.buttonMoveDown, "buttonMoveDown");
            this.buttonMoveDown.Image = global::LcmsNet.Properties.Resources.Button_Down_16;
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveUp
            // 
            resources.ApplyResources(this.buttonMoveUp, "buttonMoveUp");
            this.buttonMoveUp.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // labelLCCart
            // 
            resources.ApplyResources(this.labelLCCart, "labelLCCart");
            this.labelLCCart.Name = "labelLCCart";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listViewRequestsToRun);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // listViewRequestsToRun
            // 
            resources.ApplyResources(this.listViewRequestsToRun, "listViewRequestsToRun");
            this.listViewRequestsToRun.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderRequestsToRunReqName,
            this.columnHeaderRequestsToRunReqNum,
            this.columnHeaderRequestsToRunCart,
            this.columnHeaderRequestsToRunUser,
            this.columnHeaderRequestsToRunUsageType,
            this.columnHeaderRequestsToRunBatch,
            this.columnHeaderRequestsToRunBlock,
            this.columnHeaderRequestsToRunRunOrder});
            this.listViewRequestsToRun.FullRowSelect = true;
            this.listViewRequestsToRun.GridLines = true;
            this.listViewRequestsToRun.Name = "listViewRequestsToRun";
            this.listViewRequestsToRun.TabStop = false;
            this.listViewRequestsToRun.UseCompatibleStateImageBehavior = false;
            this.listViewRequestsToRun.View = System.Windows.Forms.View.Details;
            this.listViewRequestsToRun.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewRequestsToRun_ColumnClick);
            // 
            // columnHeaderRequestsToRunReqName
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunReqName, "columnHeaderRequestsToRunReqName");
            // 
            // columnHeaderRequestsToRunReqNum
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunReqNum, "columnHeaderRequestsToRunReqNum");
            // 
            // columnHeaderRequestsToRunCart
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunCart, "columnHeaderRequestsToRunCart");
            // 
            // columnHeaderRequestsToRunUser
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunUser, "columnHeaderRequestsToRunUser");
            // 
            // columnHeaderRequestsToRunUsageType
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunUsageType, "columnHeaderRequestsToRunUsageType");
            // 
            // columnHeaderRequestsToRunBatch
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunBatch, "columnHeaderRequestsToRunBatch");
            // 
            // columnHeaderRequestsToRunBlock
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunBlock, "columnHeaderRequestsToRunBlock");
            // 
            // columnHeaderRequestsToRunRunOrder
            // 
            resources.ApplyResources(this.columnHeaderRequestsToRunRunOrder, "columnHeaderRequestsToRunRunOrder");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // formDMSView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.splitContainer1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "formDMSView";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel panel2;
        private ComboBox comboBoxSelectCart;
        private ListView listviewAvailableRequests;
        private ColumnHeader columnHeaderAvailRequestsReqName;
        private ColumnHeader columnHeaderAvailRequestsReqNum;
        private ColumnHeader columnHeaderAvailRequestsCart;
        private ColumnHeader columnHeaderAvailRequestsUser;
        private ColumnHeader columnHeaderAvailRequestsUsageType;
        private GroupBox groupBox1;
        private CheckBox checkBoxUnAssignedOnly;
        private Button buttonFind;
        private ComboBox comboBoxCarts;
        private TextBox textRequestNumMax;
        private TextBox textRequestNumMin;
        private TextBox textRequestName;
        private Label labelCarts;
        private Label labelRequestsTo;
        private Label labelRequestsFrom;
        private Label labelRequestName;
        private Label label1;
        private Button buttonMoveDown;
        private Button buttonMoveUp;
        private Label labelLCCart;
        private Panel panel1;
        private ListView listViewRequestsToRun;
        private ColumnHeader columnHeaderRequestsToRunReqName;
        private ColumnHeader columnHeaderRequestsToRunReqNum;
        private ColumnHeader columnHeaderRequestsToRunCart;
        private ColumnHeader columnHeaderRequestsToRunUser;
        private ColumnHeader columnHeaderRequestsToRunUsageType;
        private Label label2;
        private Button buttonCancel;
        private Button buttonOK;
        private Label labelPleaseWait;
        private Button buttonUpdateCartList;
        private Label labelRequestCount;
        private ColumnHeader columnHeaderAvailRequestsBlock;
        private ColumnHeader columnHeaderAvailRequestsRunOrder;
        private ColumnHeader columnHeaderRequestsToRunBlock;
        private ColumnHeader columnHeaderRequestsToRunRunOrder;
        private ColumnHeader columnHeaderAvailRequestsBatch;
        private ColumnHeader columnHeaderRequestsToRunBatch;
        private TextBox textBatchID;
        private Label label3;
        private TextBox textBlock;
        private Label label4;
        private TextBox textWellplate;
        private Label label5;
        private ComboBox comboBoxSelectCartConfig;
        private Label labelLCCartConfig;
    }
}