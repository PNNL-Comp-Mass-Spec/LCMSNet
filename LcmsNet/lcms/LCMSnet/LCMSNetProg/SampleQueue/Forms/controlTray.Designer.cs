namespace LcmsNet.SampleQueue.Forms
{
    partial class controlTray
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonAutoAssignVials = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonUnassignTray = new System.Windows.Forms.Button();
            this.buttonMoveToTray6 = new System.Windows.Forms.Button();
            this.buttonMoveToTray5 = new System.Windows.Forms.Button();
            this.buttonMoveToTray4 = new System.Windows.Forms.Button();
            this.buttonMoveToTray3 = new System.Windows.Forms.Button();
            this.buttonMoveToTray2 = new System.Windows.Forms.Button();
            this.buttonMoveToTray1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mnum_maxVials = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.mnum_specificVial = new System.Windows.Forms.NumericUpDown();
            this.mbutton_assignToVial = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maxVials)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_specificVial)).BeginInit();
            this.SuspendLayout();
            //
            // dataGridView1
            //
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 18);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(619, 307);
            this.dataGridView1.TabIndex = 0;
            //
            // buttonAutoAssignVials
            //
            this.buttonAutoAssignVials.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAutoAssignVials.Location = new System.Drawing.Point(332, 339);
            this.buttonAutoAssignVials.Name = "buttonAutoAssignVials";
            this.buttonAutoAssignVials.Size = new System.Drawing.Size(141, 23);
            this.buttonAutoAssignVials.TabIndex = 1;
            this.buttonAutoAssignVials.Text = "Auto Assign Vials";
            this.buttonAutoAssignVials.UseVisualStyleBackColor = true;
            this.buttonAutoAssignVials.Click += new System.EventHandler(this.buttonAutoAssignVials_Click);
            //
            // panel1
            //
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.buttonUnassignTray);
            this.panel1.Controls.Add(this.buttonMoveToTray6);
            this.panel1.Controls.Add(this.buttonMoveToTray5);
            this.panel1.Controls.Add(this.buttonMoveToTray4);
            this.panel1.Controls.Add(this.buttonMoveToTray3);
            this.panel1.Controls.Add(this.buttonMoveToTray2);
            this.panel1.Controls.Add(this.buttonMoveToTray1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(651, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(112, 348);
            this.panel1.TabIndex = 6;
            //
            // buttonUnassignTray
            //
            this.buttonUnassignTray.Location = new System.Drawing.Point(17, 257);
            this.buttonUnassignTray.Name = "buttonUnassignTray";
            this.buttonUnassignTray.Size = new System.Drawing.Size(70, 50);
            this.buttonUnassignTray.TabIndex = 14;
            this.buttonUnassignTray.Text = "No Assign";
            this.buttonUnassignTray.UseVisualStyleBackColor = true;
            this.buttonUnassignTray.Click += new System.EventHandler(this.buttonUnassignTray_Click);
            //
            // buttonMoveToTray6
            //
            this.buttonMoveToTray6.Location = new System.Drawing.Point(17, 220);
            this.buttonMoveToTray6.Name = "buttonMoveToTray6";
            this.buttonMoveToTray6.Size = new System.Drawing.Size(70, 35);
            this.buttonMoveToTray6.TabIndex = 10;
            this.buttonMoveToTray6.Text = "Tray 6";
            this.buttonMoveToTray6.UseVisualStyleBackColor = true;
            this.buttonMoveToTray6.Click += new System.EventHandler(this.buttonMoveToTray6_Click);
            //
            // buttonMoveToTray5
            //
            this.buttonMoveToTray5.Location = new System.Drawing.Point(17, 183);
            this.buttonMoveToTray5.Name = "buttonMoveToTray5";
            this.buttonMoveToTray5.Size = new System.Drawing.Size(70, 35);
            this.buttonMoveToTray5.TabIndex = 13;
            this.buttonMoveToTray5.Text = "Tray 5";
            this.buttonMoveToTray5.UseVisualStyleBackColor = true;
            this.buttonMoveToTray5.Click += new System.EventHandler(this.buttonMoveToTray5_Click);
            //
            // buttonMoveToTray4
            //
            this.buttonMoveToTray4.Location = new System.Drawing.Point(17, 146);
            this.buttonMoveToTray4.Name = "buttonMoveToTray4";
            this.buttonMoveToTray4.Size = new System.Drawing.Size(70, 35);
            this.buttonMoveToTray4.TabIndex = 12;
            this.buttonMoveToTray4.Text = "Tray 4";
            this.buttonMoveToTray4.UseVisualStyleBackColor = true;
            this.buttonMoveToTray4.Click += new System.EventHandler(this.buttonMoveToTray4_Click);
            //
            // buttonMoveToTray3
            //
            this.buttonMoveToTray3.Location = new System.Drawing.Point(17, 109);
            this.buttonMoveToTray3.Name = "buttonMoveToTray3";
            this.buttonMoveToTray3.Size = new System.Drawing.Size(70, 35);
            this.buttonMoveToTray3.TabIndex = 11;
            this.buttonMoveToTray3.Text = "Tray 3";
            this.buttonMoveToTray3.UseVisualStyleBackColor = true;
            this.buttonMoveToTray3.Click += new System.EventHandler(this.buttonMoveToTray3_Click);
            //
            // buttonMoveToTray2
            //
            this.buttonMoveToTray2.Location = new System.Drawing.Point(17, 72);
            this.buttonMoveToTray2.Name = "buttonMoveToTray2";
            this.buttonMoveToTray2.Size = new System.Drawing.Size(70, 35);
            this.buttonMoveToTray2.TabIndex = 10;
            this.buttonMoveToTray2.Text = "Tray 2";
            this.buttonMoveToTray2.UseVisualStyleBackColor = true;
            this.buttonMoveToTray2.Click += new System.EventHandler(this.buttonMoveToTray2_Click);
            //
            // buttonMoveToTray1
            //
            this.buttonMoveToTray1.Location = new System.Drawing.Point(17, 35);
            this.buttonMoveToTray1.Name = "buttonMoveToTray1";
            this.buttonMoveToTray1.Size = new System.Drawing.Size(70, 35);
            this.buttonMoveToTray1.TabIndex = 9;
            this.buttonMoveToTray1.Text = "Tray 1";
            this.buttonMoveToTray1.UseVisualStyleBackColor = true;
            this.buttonMoveToTray1.Click += new System.EventHandler(this.buttonMoveToTray1_Click);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Move To:";
            //
            // mnum_maxVials
            //
            this.mnum_maxVials.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_maxVials.Location = new System.Drawing.Point(532, 342);
            this.mnum_maxVials.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.mnum_maxVials.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_maxVials.Name = "mnum_maxVials";
            this.mnum_maxVials.Size = new System.Drawing.Size(99, 20);
            this.mnum_maxVials.TabIndex = 7;
            this.mnum_maxVials.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_maxVials.Value = new decimal(new int[] {
            54,
            0,
            0,
            0});
            //
            // label2
            //
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(479, 345);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Max Vials";
            //
            // mnum_specificVial
            //
            this.mnum_specificVial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mnum_specificVial.Location = new System.Drawing.Point(159, 342);
            this.mnum_specificVial.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.mnum_specificVial.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_specificVial.Name = "mnum_specificVial";
            this.mnum_specificVial.Size = new System.Drawing.Size(99, 20);
            this.mnum_specificVial.TabIndex = 10;
            this.mnum_specificVial.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_specificVial.Value = new decimal(new int[] {
            54,
            0,
            0,
            0});
            //
            // mbutton_assignToVial
            //
            this.mbutton_assignToVial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_assignToVial.Location = new System.Drawing.Point(12, 340);
            this.mbutton_assignToVial.Name = "mbutton_assignToVial";
            this.mbutton_assignToVial.Size = new System.Drawing.Size(141, 23);
            this.mbutton_assignToVial.TabIndex = 9;
            this.mbutton_assignToVial.Text = "Assign All Selected To Vial";
            this.mbutton_assignToVial.UseVisualStyleBackColor = true;
            this.mbutton_assignToVial.Click += new System.EventHandler(this.mbutton_assignToVial_Click);
            //
            // controlTray
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mnum_specificVial);
            this.Controls.Add(this.mbutton_assignToVial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mnum_maxVials);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonAutoAssignVials);
            this.Controls.Add(this.dataGridView1);
            this.Name = "controlTray";
            this.Size = new System.Drawing.Size(775, 375);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maxVials)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_specificVial)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonAutoAssignVials;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonMoveToTray6;
        private System.Windows.Forms.Button buttonMoveToTray5;
        private System.Windows.Forms.Button buttonMoveToTray4;
        private System.Windows.Forms.Button buttonMoveToTray3;
        private System.Windows.Forms.Button buttonMoveToTray2;
        private System.Windows.Forms.Button buttonMoveToTray1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonUnassignTray;
        private System.Windows.Forms.NumericUpDown mnum_maxVials;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown mnum_specificVial;
        private System.Windows.Forms.Button mbutton_assignToVial;
    }
}
