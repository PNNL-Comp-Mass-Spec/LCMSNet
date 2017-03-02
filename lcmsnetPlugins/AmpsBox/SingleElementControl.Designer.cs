namespace AmpsBox
{
    partial class SingleElementControl
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
            this.mgroupbox_name = new System.Windows.Forms.GroupBox();
            this.mbutton_read = new System.Windows.Forms.Button();
            this.mlabel_value = new System.Windows.Forms.Label();
            this.mbutton_set = new System.Windows.Forms.Button();
            this.mnum_setpointValue = new System.Windows.Forms.NumericUpDown();
            this.mgroupbox_name.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_setpointValue)).BeginInit();
            this.SuspendLayout();
            // 
            // mgroupbox_name
            // 
            this.mgroupbox_name.Controls.Add(this.mbutton_read);
            this.mgroupbox_name.Controls.Add(this.mlabel_value);
            this.mgroupbox_name.Controls.Add(this.mbutton_set);
            this.mgroupbox_name.Controls.Add(this.mnum_setpointValue);
            this.mgroupbox_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupbox_name.Location = new System.Drawing.Point(3, 3);
            this.mgroupbox_name.Name = "mgroupbox_name";
            this.mgroupbox_name.Size = new System.Drawing.Size(226, 108);
            this.mgroupbox_name.TabIndex = 46;
            this.mgroupbox_name.TabStop = false;
            this.mgroupbox_name.Text = "RF Frequency";
            // 
            // mbutton_read
            // 
            this.mbutton_read.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_read.Location = new System.Drawing.Point(152, 68);
            this.mbutton_read.Name = "mbutton_read";
            this.mbutton_read.Size = new System.Drawing.Size(63, 31);
            this.mbutton_read.TabIndex = 44;
            this.mbutton_read.Text = "Read";
            this.mbutton_read.UseVisualStyleBackColor = true;
            this.mbutton_read.Click += new System.EventHandler(this.mbutton_read_Click);
            // 
            // mlabel_value
            // 
            this.mlabel_value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mlabel_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_value.Location = new System.Drawing.Point(6, 68);
            this.mlabel_value.Name = "mlabel_value";
            this.mlabel_value.Size = new System.Drawing.Size(140, 31);
            this.mlabel_value.TabIndex = 43;
            this.mlabel_value.Text = "---";
            this.mlabel_value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mbutton_set
            // 
            this.mbutton_set.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_set.Location = new System.Drawing.Point(152, 32);
            this.mbutton_set.Name = "mbutton_set";
            this.mbutton_set.Size = new System.Drawing.Size(63, 31);
            this.mbutton_set.TabIndex = 42;
            this.mbutton_set.Text = "Set";
            this.mbutton_set.UseVisualStyleBackColor = true;
            this.mbutton_set.Click += new System.EventHandler(this.mbutton_set_Click);
            // 
            // mnum_setpointValue
            // 
            this.mnum_setpointValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnum_setpointValue.Location = new System.Drawing.Point(6, 37);
            this.mnum_setpointValue.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.mnum_setpointValue.Name = "mnum_setpointValue";
            this.mnum_setpointValue.Size = new System.Drawing.Size(140, 22);
            this.mnum_setpointValue.TabIndex = 40;
            this.mnum_setpointValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_setpointValue.ValueChanged += new System.EventHandler(this.mnum_setpointValue_ValueChanged);
            // 
            // SingleElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mgroupbox_name);
            this.Name = "SingleElementControl";
            this.Size = new System.Drawing.Size(234, 117);
            this.mgroupbox_name.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_setpointValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox mgroupbox_name;
        private System.Windows.Forms.Button mbutton_read;
        private System.Windows.Forms.Label mlabel_value;
        private System.Windows.Forms.Button mbutton_set;
        private System.Windows.Forms.NumericUpDown mnum_setpointValue;

    }
}
