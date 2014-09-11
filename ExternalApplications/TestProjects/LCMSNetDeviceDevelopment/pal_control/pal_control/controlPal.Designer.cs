namespace pal_control
{
    partial class controlPal
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mButton_Initialize = new System.Windows.Forms.Button();
            this.mgroupBox_Methods = new System.Windows.Forms.GroupBox();
            this.mLabel_MicroLiters = new System.Windows.Forms.Label();
            this.mLabel_Volume = new System.Windows.Forms.Label();
            this.mLabel_Vial = new System.Windows.Forms.Label();
            this.mLabel_Tray = new System.Windows.Forms.Label();
            this.mTextBox_Volume = new System.Windows.Forms.TextBox();
            this.mTextBox_Vial = new System.Windows.Forms.TextBox();
            this.mTextBox_Tray = new System.Windows.Forms.TextBox();
            this.mButton_RefreshMethods = new System.Windows.Forms.Button();
            this.mButton_RunMethod = new System.Windows.Forms.Button();
            this.mcomboBox_MethodList = new System.Windows.Forms.ComboBox();
            this.mTextBox_Status = new System.Windows.Forms.TextBox();
            this.mButton_StatusRefresh = new System.Windows.Forms.Button();
            this.mGroupBox_Status = new System.Windows.Forms.GroupBox();
            this.mgroupBox_Methods.SuspendLayout();
            this.mGroupBox_Status.SuspendLayout();
            this.SuspendLayout();
            // 
            // mButton_Initialize
            // 
            this.mButton_Initialize.Location = new System.Drawing.Point(9, 211);
            this.mButton_Initialize.Name = "mButton_Initialize";
            this.mButton_Initialize.Size = new System.Drawing.Size(243, 23);
            this.mButton_Initialize.TabIndex = 3;
            this.mButton_Initialize.Text = "Initialize PAL";
            this.mButton_Initialize.UseVisualStyleBackColor = true;
            this.mButton_Initialize.Click += new System.EventHandler(this.mButton_Initialize_Click);
            // 
            // mgroupBox_Methods
            // 
            this.mgroupBox_Methods.Controls.Add(this.mLabel_MicroLiters);
            this.mgroupBox_Methods.Controls.Add(this.mLabel_Volume);
            this.mgroupBox_Methods.Controls.Add(this.mLabel_Vial);
            this.mgroupBox_Methods.Controls.Add(this.mLabel_Tray);
            this.mgroupBox_Methods.Controls.Add(this.mTextBox_Volume);
            this.mgroupBox_Methods.Controls.Add(this.mTextBox_Vial);
            this.mgroupBox_Methods.Controls.Add(this.mTextBox_Tray);
            this.mgroupBox_Methods.Controls.Add(this.mButton_RefreshMethods);
            this.mgroupBox_Methods.Controls.Add(this.mButton_RunMethod);
            this.mgroupBox_Methods.Controls.Add(this.mcomboBox_MethodList);
            this.mgroupBox_Methods.Location = new System.Drawing.Point(9, 3);
            this.mgroupBox_Methods.Name = "mgroupBox_Methods";
            this.mgroupBox_Methods.Size = new System.Drawing.Size(243, 128);
            this.mgroupBox_Methods.TabIndex = 4;
            this.mgroupBox_Methods.TabStop = false;
            this.mgroupBox_Methods.Text = "Methods";
            // 
            // mLabel_MicroLiters
            // 
            this.mLabel_MicroLiters.AutoSize = true;
            this.mLabel_MicroLiters.Location = new System.Drawing.Point(95, 103);
            this.mLabel_MicroLiters.Name = "mLabel_MicroLiters";
            this.mLabel_MicroLiters.Size = new System.Drawing.Size(19, 13);
            this.mLabel_MicroLiters.TabIndex = 9;
            this.mLabel_MicroLiters.Text = "uL";
            // 
            // mLabel_Volume
            // 
            this.mLabel_Volume.AutoSize = true;
            this.mLabel_Volume.Location = new System.Drawing.Point(16, 103);
            this.mLabel_Volume.Name = "mLabel_Volume";
            this.mLabel_Volume.Size = new System.Drawing.Size(42, 13);
            this.mLabel_Volume.TabIndex = 8;
            this.mLabel_Volume.Text = "Volume";
            // 
            // mLabel_Vial
            // 
            this.mLabel_Vial.AutoSize = true;
            this.mLabel_Vial.Location = new System.Drawing.Point(16, 76);
            this.mLabel_Vial.Name = "mLabel_Vial";
            this.mLabel_Vial.Size = new System.Drawing.Size(24, 13);
            this.mLabel_Vial.TabIndex = 7;
            this.mLabel_Vial.Text = "Vial";
            // 
            // mLabel_Tray
            // 
            this.mLabel_Tray.AutoSize = true;
            this.mLabel_Tray.Location = new System.Drawing.Point(14, 50);
            this.mLabel_Tray.Name = "mLabel_Tray";
            this.mLabel_Tray.Size = new System.Drawing.Size(28, 13);
            this.mLabel_Tray.TabIndex = 6;
            this.mLabel_Tray.Text = "Tray";
            // 
            // mTextBox_Volume
            // 
            this.mTextBox_Volume.Location = new System.Drawing.Point(64, 100);
            this.mTextBox_Volume.Name = "mTextBox_Volume";
            this.mTextBox_Volume.Size = new System.Drawing.Size(28, 20);
            this.mTextBox_Volume.TabIndex = 5;
            this.mTextBox_Volume.Text = "25";
            // 
            // mTextBox_Vial
            // 
            this.mTextBox_Vial.Location = new System.Drawing.Point(64, 73);
            this.mTextBox_Vial.Name = "mTextBox_Vial";
            this.mTextBox_Vial.Size = new System.Drawing.Size(28, 20);
            this.mTextBox_Vial.TabIndex = 4;
            this.mTextBox_Vial.Text = "1";
            // 
            // mTextBox_Tray
            // 
            this.mTextBox_Tray.Location = new System.Drawing.Point(64, 47);
            this.mTextBox_Tray.Name = "mTextBox_Tray";
            this.mTextBox_Tray.Size = new System.Drawing.Size(28, 20);
            this.mTextBox_Tray.TabIndex = 3;
            this.mTextBox_Tray.Text = "1";
            // 
            // mButton_RefreshMethods
            // 
            this.mButton_RefreshMethods.Location = new System.Drawing.Point(121, 44);
            this.mButton_RefreshMethods.Name = "mButton_RefreshMethods";
            this.mButton_RefreshMethods.Size = new System.Drawing.Size(112, 23);
            this.mButton_RefreshMethods.TabIndex = 2;
            this.mButton_RefreshMethods.Text = "Refresh";
            this.mButton_RefreshMethods.UseVisualStyleBackColor = true;
            this.mButton_RefreshMethods.Click += new System.EventHandler(this.mButton_RefreshMethods_Click);
            // 
            // mButton_RunMethod
            // 
            this.mButton_RunMethod.Location = new System.Drawing.Point(121, 73);
            this.mButton_RunMethod.Name = "mButton_RunMethod";
            this.mButton_RunMethod.Size = new System.Drawing.Size(112, 23);
            this.mButton_RunMethod.TabIndex = 1;
            this.mButton_RunMethod.Text = "Run Method";
            this.mButton_RunMethod.UseVisualStyleBackColor = true;
            this.mButton_RunMethod.Click += new System.EventHandler(this.mButton_RunMethod_Click);
            // 
            // mcomboBox_MethodList
            // 
            this.mcomboBox_MethodList.FormattingEnabled = true;
            this.mcomboBox_MethodList.Location = new System.Drawing.Point(7, 20);
            this.mcomboBox_MethodList.Name = "mcomboBox_MethodList";
            this.mcomboBox_MethodList.Size = new System.Drawing.Size(226, 21);
            this.mcomboBox_MethodList.TabIndex = 0;
            // 
            // mTextBox_Status
            // 
            this.mTextBox_Status.Location = new System.Drawing.Point(11, 16);
            this.mTextBox_Status.Name = "mTextBox_Status";
            this.mTextBox_Status.ReadOnly = true;
            this.mTextBox_Status.Size = new System.Drawing.Size(222, 20);
            this.mTextBox_Status.TabIndex = 5;
            // 
            // mButton_StatusRefresh
            // 
            this.mButton_StatusRefresh.Location = new System.Drawing.Point(11, 39);
            this.mButton_StatusRefresh.Name = "mButton_StatusRefresh";
            this.mButton_StatusRefresh.Size = new System.Drawing.Size(222, 23);
            this.mButton_StatusRefresh.TabIndex = 6;
            this.mButton_StatusRefresh.Text = "Refresh";
            this.mButton_StatusRefresh.UseVisualStyleBackColor = true;
            this.mButton_StatusRefresh.Click += new System.EventHandler(this.mButton_StatusRefresh_Click);
            // 
            // mGroupBox_Status
            // 
            this.mGroupBox_Status.Controls.Add(this.mButton_StatusRefresh);
            this.mGroupBox_Status.Controls.Add(this.mTextBox_Status);
            this.mGroupBox_Status.Location = new System.Drawing.Point(9, 137);
            this.mGroupBox_Status.Name = "mGroupBox_Status";
            this.mGroupBox_Status.Size = new System.Drawing.Size(243, 68);
            this.mGroupBox_Status.TabIndex = 7;
            this.mGroupBox_Status.TabStop = false;
            this.mGroupBox_Status.Text = "Status";
            // 
            // controlPal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mGroupBox_Status);
            this.Controls.Add(this.mgroupBox_Methods);
            this.Controls.Add(this.mButton_Initialize);
            this.Name = "controlPal";
            this.Controls.SetChildIndex(this.mButton_Initialize, 0);
            this.Controls.SetChildIndex(this.mgroupBox_Methods, 0);
            this.Controls.SetChildIndex(this.mGroupBox_Status, 0);
            this.mgroupBox_Methods.ResumeLayout(false);
            this.mgroupBox_Methods.PerformLayout();
            this.mGroupBox_Status.ResumeLayout(false);
            this.mGroupBox_Status.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mButton_Initialize;
        private System.Windows.Forms.GroupBox mgroupBox_Methods;
        private System.Windows.Forms.Button mButton_RunMethod;
        private System.Windows.Forms.ComboBox mcomboBox_MethodList;
        private System.Windows.Forms.Button mButton_RefreshMethods;
        private System.Windows.Forms.TextBox mTextBox_Vial;
        private System.Windows.Forms.TextBox mTextBox_Tray;
        private System.Windows.Forms.Label mLabel_Volume;
        private System.Windows.Forms.Label mLabel_Vial;
        private System.Windows.Forms.Label mLabel_Tray;
        private System.Windows.Forms.TextBox mTextBox_Volume;
        private System.Windows.Forms.Label mLabel_MicroLiters;
        private System.Windows.Forms.TextBox mTextBox_Status;
        private System.Windows.Forms.Button mButton_StatusRefresh;
        private System.Windows.Forms.GroupBox mGroupBox_Status;
    }
}
