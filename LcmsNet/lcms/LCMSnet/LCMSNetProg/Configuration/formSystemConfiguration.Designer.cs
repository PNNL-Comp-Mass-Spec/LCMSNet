namespace LcmsNet.Configuration
{
    partial class formSystemConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.mlabel_cartName = new System.Windows.Forms.Label();
            this.mlabel_Cart = new System.Windows.Forms.Label();
            this.lblSeparationType = new System.Windows.Forms.Label();
            this.mcombo_SepType = new System.Windows.Forms.ComboBox();
            this.mbutton_Reload = new System.Windows.Forms.Button();
            this.mbutton_accept = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxAvailInstruments = new System.Windows.Forms.ComboBox();
            this.mgroupBox_instrument = new System.Windows.Forms.GroupBox();
            this.mgroupBox_cart = new System.Windows.Forms.GroupBox();
            this.mcombo_CartConfigName = new System.Windows.Forms.ComboBox();
            this.lblCartConfigName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mbutton_acceptOperator = new System.Windows.Forms.Button();
            this.mcombo_Operator = new System.Windows.Forms.ComboBox();
            this.mgroupBox_autoUploads = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mtextbox_triggerLocation = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblTimeZone = new System.Windows.Forms.Label();
            this.comboTimeZone = new System.Windows.Forms.ComboBox();
            this.txtPdfPath = new System.Windows.Forms.TextBox();
            this.lblWritePDFTo = new System.Windows.Forms.Label();
            this.lblMinimumSampleVolume = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.comboDmsTools = new System.Windows.Forms.ComboBox();
            this.mcontrol_columnTwo = new LcmsNet.Configuration.controlColumn();
            this.mcontrol_columnOne = new LcmsNet.Configuration.controlColumn();
            this.mcontrol_columnFour = new LcmsNet.Configuration.controlColumn();
            this.mcontrol_columnThree = new LcmsNet.Configuration.controlColumn();
            this.mgroupBox_instrument.SuspendLayout();
            this.mgroupBox_cart.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mgroupBox_autoUploads.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // mlabel_cartName
            // 
            this.mlabel_cartName.AutoSize = true;
            this.mlabel_cartName.Location = new System.Drawing.Point(19, 27);
            this.mlabel_cartName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mlabel_cartName.Name = "mlabel_cartName";
            this.mlabel_cartName.Size = new System.Drawing.Size(112, 25);
            this.mlabel_cartName.TabIndex = 26;
            this.mlabel_cartName.Text = "Cart Name:";
            // 
            // mlabel_Cart
            // 
            this.mlabel_Cart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.mlabel_Cart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mlabel_Cart.Location = new System.Drawing.Point(261, 19);
            this.mlabel_Cart.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mlabel_Cart.Name = "mlabel_Cart";
            this.mlabel_Cart.Size = new System.Drawing.Size(402, 34);
            this.mlabel_Cart.TabIndex = 29;
            this.mlabel_Cart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSeparationType
            // 
            this.lblSeparationType.AutoSize = true;
            this.lblSeparationType.Location = new System.Drawing.Point(20, 101);
            this.lblSeparationType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSeparationType.Name = "lblSeparationType";
            this.lblSeparationType.Size = new System.Drawing.Size(163, 25);
            this.lblSeparationType.TabIndex = 30;
            this.lblSeparationType.Text = "Separation Type:";
            // 
            // mcombo_SepType
            // 
            this.mcombo_SepType.FormattingEnabled = true;
            this.mcombo_SepType.Location = new System.Drawing.Point(261, 98);
            this.mcombo_SepType.Margin = new System.Windows.Forms.Padding(4);
            this.mcombo_SepType.Name = "mcombo_SepType";
            this.mcombo_SepType.Size = new System.Drawing.Size(401, 33);
            this.mcombo_SepType.TabIndex = 31;
            this.mcombo_SepType.SelectedIndexChanged += new System.EventHandler(this.mcombo_SepType_SelectedIndexChanged);
            // 
            // mbutton_Reload
            // 
            this.mbutton_Reload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_Reload.Location = new System.Drawing.Point(8, 348);
            this.mbutton_Reload.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_Reload.Name = "mbutton_Reload";
            this.mbutton_Reload.Size = new System.Drawing.Size(269, 33);
            this.mbutton_Reload.TabIndex = 32;
            this.mbutton_Reload.Text = "Reload Column Names";
            this.mbutton_Reload.UseVisualStyleBackColor = true;
            this.mbutton_Reload.Click += new System.EventHandler(this.mbutton_Reload_Click);
            // 
            // mbutton_accept
            // 
            this.mbutton_accept.Location = new System.Drawing.Point(463, 62);
            this.mbutton_accept.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_accept.Name = "mbutton_accept";
            this.mbutton_accept.Size = new System.Drawing.Size(127, 34);
            this.mbutton_accept.TabIndex = 35;
            this.mbutton_accept.Text = "Set";
            this.mbutton_accept.UseVisualStyleBackColor = true;
            this.mbutton_accept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 25);
            this.label2.TabIndex = 34;
            this.label2.Text = "Available Instruments";
            // 
            // comboBoxAvailInstruments
            // 
            this.comboBoxAvailInstruments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAvailInstruments.FormattingEnabled = true;
            this.comboBoxAvailInstruments.Location = new System.Drawing.Point(23, 62);
            this.comboBoxAvailInstruments.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxAvailInstruments.Name = "comboBoxAvailInstruments";
            this.comboBoxAvailInstruments.Size = new System.Drawing.Size(413, 33);
            this.comboBoxAvailInstruments.TabIndex = 33;
            this.comboBoxAvailInstruments.SelectedIndexChanged += new System.EventHandler(this.comboBoxAvailInstruments_SelectedIndexChanged);
            // 
            // mgroupBox_instrument
            // 
            this.mgroupBox_instrument.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_instrument.Controls.Add(this.comboBoxAvailInstruments);
            this.mgroupBox_instrument.Controls.Add(this.mbutton_accept);
            this.mgroupBox_instrument.Controls.Add(this.label2);
            this.mgroupBox_instrument.Location = new System.Drawing.Point(8, 7);
            this.mgroupBox_instrument.Margin = new System.Windows.Forms.Padding(4);
            this.mgroupBox_instrument.Name = "mgroupBox_instrument";
            this.mgroupBox_instrument.Padding = new System.Windows.Forms.Padding(4);
            this.mgroupBox_instrument.Size = new System.Drawing.Size(1109, 148);
            this.mgroupBox_instrument.TabIndex = 36;
            this.mgroupBox_instrument.TabStop = false;
            this.mgroupBox_instrument.Text = "Mass Spectrometer Instrument";
            // 
            // mgroupBox_cart
            // 
            this.mgroupBox_cart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_cart.Controls.Add(this.mcombo_CartConfigName);
            this.mgroupBox_cart.Controls.Add(this.lblCartConfigName);
            this.mgroupBox_cart.Controls.Add(this.mcontrol_columnTwo);
            this.mgroupBox_cart.Controls.Add(this.mcontrol_columnOne);
            this.mgroupBox_cart.Controls.Add(this.mcontrol_columnFour);
            this.mgroupBox_cart.Controls.Add(this.mbutton_Reload);
            this.mgroupBox_cart.Controls.Add(this.mcontrol_columnThree);
            this.mgroupBox_cart.Controls.Add(this.mlabel_cartName);
            this.mgroupBox_cart.Controls.Add(this.mcombo_SepType);
            this.mgroupBox_cart.Controls.Add(this.lblSeparationType);
            this.mgroupBox_cart.Controls.Add(this.mlabel_Cart);
            this.mgroupBox_cart.Location = new System.Drawing.Point(8, 7);
            this.mgroupBox_cart.Margin = new System.Windows.Forms.Padding(4);
            this.mgroupBox_cart.Name = "mgroupBox_cart";
            this.mgroupBox_cart.Padding = new System.Windows.Forms.Padding(4);
            this.mgroupBox_cart.Size = new System.Drawing.Size(1109, 389);
            this.mgroupBox_cart.TabIndex = 37;
            this.mgroupBox_cart.TabStop = false;
            this.mgroupBox_cart.Text = "LC-Cart";
            // 
            // mcombo_CartConfigName
            // 
            this.mcombo_CartConfigName.FormattingEnabled = true;
            this.mcombo_CartConfigName.Location = new System.Drawing.Point(261, 57);
            this.mcombo_CartConfigName.Margin = new System.Windows.Forms.Padding(4);
            this.mcombo_CartConfigName.Name = "mcombo_CartConfigName";
            this.mcombo_CartConfigName.Size = new System.Drawing.Size(674, 33);
            this.mcombo_CartConfigName.TabIndex = 34;
            this.mcombo_CartConfigName.SelectedIndexChanged += new System.EventHandler(this.mcombo_CartConfigName_SelectedIndexChanged);
            // 
            // lblCartConfigName
            // 
            this.lblCartConfigName.AutoSize = true;
            this.lblCartConfigName.Location = new System.Drawing.Point(19, 60);
            this.lblCartConfigName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCartConfigName.Name = "lblCartConfigName";
            this.lblCartConfigName.Size = new System.Drawing.Size(233, 25);
            this.lblCartConfigName.TabIndex = 33;
            this.lblCartConfigName.Text = "Cart Configuration Name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.mbutton_acceptOperator);
            this.groupBox1.Controls.Add(this.mcombo_Operator);
            this.groupBox1.Location = new System.Drawing.Point(23, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1075, 68);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operator";
            // 
            // mbutton_acceptOperator
            // 
            this.mbutton_acceptOperator.Location = new System.Drawing.Point(333, 21);
            this.mbutton_acceptOperator.Margin = new System.Windows.Forms.Padding(4);
            this.mbutton_acceptOperator.Name = "mbutton_acceptOperator";
            this.mbutton_acceptOperator.Size = new System.Drawing.Size(105, 39);
            this.mbutton_acceptOperator.TabIndex = 36;
            this.mbutton_acceptOperator.Text = "Set";
            this.mbutton_acceptOperator.UseVisualStyleBackColor = true;
            this.mbutton_acceptOperator.Click += new System.EventHandler(this.mbutton_acceptOperator_Click);
            // 
            // mcombo_Operator
            // 
            this.mcombo_Operator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcombo_Operator.FormattingEnabled = true;
            this.mcombo_Operator.Location = new System.Drawing.Point(23, 23);
            this.mcombo_Operator.Margin = new System.Windows.Forms.Padding(4);
            this.mcombo_Operator.Name = "mcombo_Operator";
            this.mcombo_Operator.Size = new System.Drawing.Size(301, 33);
            this.mcombo_Operator.TabIndex = 33;
            this.mcombo_Operator.SelectedIndexChanged += new System.EventHandler(this.mcombo_Operator_SelectedIndexChanged);
            // 
            // mgroupBox_autoUploads
            // 
            this.mgroupBox_autoUploads.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_autoUploads.Controls.Add(this.label3);
            this.mgroupBox_autoUploads.Controls.Add(this.mtextbox_triggerLocation);
            this.mgroupBox_autoUploads.Location = new System.Drawing.Point(4, 4);
            this.mgroupBox_autoUploads.Margin = new System.Windows.Forms.Padding(4);
            this.mgroupBox_autoUploads.Name = "mgroupBox_autoUploads";
            this.mgroupBox_autoUploads.Padding = new System.Windows.Forms.Padding(4);
            this.mgroupBox_autoUploads.Size = new System.Drawing.Size(1099, 159);
            this.mgroupBox_autoUploads.TabIndex = 39;
            this.mgroupBox_autoUploads.TabStop = false;
            this.mgroupBox_autoUploads.Text = "Data Auto-Upload DMS";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 58);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(261, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "Remote Trigger File Location";
            // 
            // mtextbox_triggerLocation
            // 
            this.mtextbox_triggerLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextbox_triggerLocation.Enabled = false;
            this.mtextbox_triggerLocation.Location = new System.Drawing.Point(21, 86);
            this.mtextbox_triggerLocation.Margin = new System.Windows.Forms.Padding(4);
            this.mtextbox_triggerLocation.Name = "mtextbox_triggerLocation";
            this.mtextbox_triggerLocation.Size = new System.Drawing.Size(1013, 30);
            this.mtextbox_triggerLocation.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(16, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1136, 711);
            this.tabControl1.TabIndex = 40;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 34);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1128, 673);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Operator";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblTimeZone);
            this.tabPage1.Controls.Add(this.comboTimeZone);
            this.tabPage1.Controls.Add(this.txtPdfPath);
            this.tabPage1.Controls.Add(this.lblWritePDFTo);
            this.tabPage1.Controls.Add(this.lblMinimumSampleVolume);
            this.tabPage1.Controls.Add(this.numericUpDown1);
            this.tabPage1.Controls.Add(this.mgroupBox_cart);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1128, 673);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "LC Cart";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblTimeZone
            // 
            this.lblTimeZone.AutoSize = true;
            this.lblTimeZone.Location = new System.Drawing.Point(515, 432);
            this.lblTimeZone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimeZone.Name = "lblTimeZone";
            this.lblTimeZone.Size = new System.Drawing.Size(112, 25);
            this.lblTimeZone.TabIndex = 45;
            this.lblTimeZone.Text = "Time Zone:";
            // 
            // comboTimeZone
            // 
            this.comboTimeZone.FormattingEnabled = true;
            this.comboTimeZone.Location = new System.Drawing.Point(640, 427);
            this.comboTimeZone.Margin = new System.Windows.Forms.Padding(4);
            this.comboTimeZone.Name = "comboTimeZone";
            this.comboTimeZone.Size = new System.Drawing.Size(321, 33);
            this.comboTimeZone.TabIndex = 44;
            this.comboTimeZone.SelectedValueChanged += new System.EventHandler(this.comboTimeZone_SelectedValueChanged);
            // 
            // txtPdfPath
            // 
            this.txtPdfPath.Enabled = false;
            this.txtPdfPath.Location = new System.Drawing.Point(173, 486);
            this.txtPdfPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtPdfPath.Name = "txtPdfPath";
            this.txtPdfPath.Size = new System.Drawing.Size(569, 30);
            this.txtPdfPath.TabIndex = 43;
            // 
            // lblWritePDFTo
            // 
            this.lblWritePDFTo.AutoSize = true;
            this.lblWritePDFTo.Location = new System.Drawing.Point(27, 490);
            this.lblWritePDFTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWritePDFTo.Name = "lblWritePDFTo";
            this.lblWritePDFTo.Size = new System.Drawing.Size(129, 25);
            this.lblWritePDFTo.TabIndex = 40;
            this.lblWritePDFTo.Text = "Write PDF to:";
            // 
            // lblMinimumSampleVolume
            // 
            this.lblMinimumSampleVolume.AutoSize = true;
            this.lblMinimumSampleVolume.Location = new System.Drawing.Point(27, 427);
            this.lblMinimumSampleVolume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMinimumSampleVolume.Name = "lblMinimumSampleVolume";
            this.lblMinimumSampleVolume.Size = new System.Drawing.Size(276, 25);
            this.lblMinimumSampleVolume.TabIndex = 39;
            this.lblMinimumSampleVolume.Text = "Minimum Sample Volume (uL)";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(343, 425);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(145, 30);
            this.numericUpDown1.TabIndex = 38;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.mgroupBox_instrument);
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1128, 673);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mass Spectrometer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.comboDmsTools);
            this.tabPage3.Controls.Add(this.mgroupBox_autoUploads);
            this.tabPage3.Location = new System.Drawing.Point(4, 34);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1128, 673);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "DMS";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 204);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 25);
            this.label7.TabIndex = 41;
            this.label7.Text = "DMS Tool:";
            // 
            // comboDmsTools
            // 
            this.comboDmsTools.FormattingEnabled = true;
            this.comboDmsTools.Location = new System.Drawing.Point(139, 201);
            this.comboDmsTools.Margin = new System.Windows.Forms.Padding(4);
            this.comboDmsTools.Name = "comboDmsTools";
            this.comboDmsTools.Size = new System.Drawing.Size(352, 33);
            this.comboDmsTools.TabIndex = 40;
            this.comboDmsTools.SelectedIndexChanged += new System.EventHandler(this.comboDmsTools_SelectedIndexChanged);
            // 
            // mcontrol_columnTwo
            // 
            this.mcontrol_columnTwo.ColumnData = null;
            this.mcontrol_columnTwo.ColumnID = 2;
            this.mcontrol_columnTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcontrol_columnTwo.Location = new System.Drawing.Point(567, 153);
            this.mcontrol_columnTwo.Margin = new System.Windows.Forms.Padding(5);
            this.mcontrol_columnTwo.Name = "mcontrol_columnTwo";
            this.mcontrol_columnTwo.Size = new System.Drawing.Size(419, 100);
            this.mcontrol_columnTwo.TabIndex = 12;
            // 
            // mcontrol_columnOne
            // 
            this.mcontrol_columnOne.ColumnData = null;
            this.mcontrol_columnOne.ColumnID = 1;
            this.mcontrol_columnOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcontrol_columnOne.Location = new System.Drawing.Point(9, 153);
            this.mcontrol_columnOne.Margin = new System.Windows.Forms.Padding(5);
            this.mcontrol_columnOne.Name = "mcontrol_columnOne";
            this.mcontrol_columnOne.Size = new System.Drawing.Size(472, 100);
            this.mcontrol_columnOne.TabIndex = 0;
            // 
            // mcontrol_columnFour
            // 
            this.mcontrol_columnFour.ColumnData = null;
            this.mcontrol_columnFour.ColumnID = 4;
            this.mcontrol_columnFour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcontrol_columnFour.Location = new System.Drawing.Point(565, 254);
            this.mcontrol_columnFour.Margin = new System.Windows.Forms.Padding(5);
            this.mcontrol_columnFour.Name = "mcontrol_columnFour";
            this.mcontrol_columnFour.Size = new System.Drawing.Size(431, 78);
            this.mcontrol_columnFour.TabIndex = 16;
            // 
            // mcontrol_columnThree
            // 
            this.mcontrol_columnThree.ColumnData = null;
            this.mcontrol_columnThree.ColumnID = 3;
            this.mcontrol_columnThree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcontrol_columnThree.Location = new System.Drawing.Point(9, 254);
            this.mcontrol_columnThree.Margin = new System.Windows.Forms.Padding(5);
            this.mcontrol_columnThree.Name = "mcontrol_columnThree";
            this.mcontrol_columnThree.Size = new System.Drawing.Size(472, 85);
            this.mcontrol_columnThree.TabIndex = 14;
            // 
            // formSystemConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1161, 743);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(689, 307);
            this.Name = "formSystemConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cart Configuration";
            this.mgroupBox_instrument.ResumeLayout(false);
            this.mgroupBox_instrument.PerformLayout();
            this.mgroupBox_cart.ResumeLayout(false);
            this.mgroupBox_cart.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.mgroupBox_autoUploads.ResumeLayout(false);
            this.mgroupBox_autoUploads.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private controlColumn mcontrol_columnOne;
        private controlColumn mcontrol_columnTwo;
        private controlColumn mcontrol_columnThree;
        private controlColumn mcontrol_columnFour;
        private System.Windows.Forms.Label mlabel_cartName;
          private System.Windows.Forms.Label mlabel_Cart;
          private System.Windows.Forms.Label lblSeparationType;
          private System.Windows.Forms.ComboBox mcombo_SepType;
          private System.Windows.Forms.Button mbutton_Reload;
          private System.Windows.Forms.Button mbutton_accept;
          private System.Windows.Forms.Label label2;
          private System.Windows.Forms.ComboBox comboBoxAvailInstruments;
          private System.Windows.Forms.GroupBox mgroupBox_instrument;
          private System.Windows.Forms.GroupBox mgroupBox_cart;
          private System.Windows.Forms.GroupBox groupBox1;
          private System.Windows.Forms.ComboBox mcombo_Operator;
          private System.Windows.Forms.Button mbutton_acceptOperator;
          private System.Windows.Forms.GroupBox mgroupBox_autoUploads;
          private System.Windows.Forms.TextBox mtextbox_triggerLocation;
          private System.Windows.Forms.TabControl tabControl1;
          private System.Windows.Forms.TabPage tabPage1;
          private System.Windows.Forms.TabPage tabPage2;
          private System.Windows.Forms.TabPage tabPage3;
          private System.Windows.Forms.TabPage tabPage4;
          private System.Windows.Forms.Label lblMinimumSampleVolume;
          private System.Windows.Forms.NumericUpDown numericUpDown1;
          private System.Windows.Forms.Label label3;
          private System.Windows.Forms.TextBox txtPdfPath;
          private System.Windows.Forms.Label lblWritePDFTo;
          private System.Windows.Forms.Label lblTimeZone;
          private System.Windows.Forms.ComboBox comboTimeZone;
          private System.Windows.Forms.Label label7;
          private System.Windows.Forms.ComboBox comboDmsTools;
        private System.Windows.Forms.ComboBox mcombo_CartConfigName;
        private System.Windows.Forms.Label lblCartConfigName;
    }
}