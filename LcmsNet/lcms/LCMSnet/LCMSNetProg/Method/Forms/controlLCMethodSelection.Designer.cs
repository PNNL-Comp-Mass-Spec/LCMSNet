namespace LcmsNet.Method.Forms
{
    partial class controlLCMethodSelection
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
            this.mcomboBox_methods = new System.Windows.Forms.ComboBox();
            this.mbutton_remove = new System.Windows.Forms.Button();
            this.mbutton_up = new System.Windows.Forms.Button();
            this.mbutton_down = new System.Windows.Forms.Button();
            this.mbutton_add = new System.Windows.Forms.Button();
            this.mlistBox_methods = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            //
            // mcomboBox_methods
            //
            this.mcomboBox_methods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_methods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_methods.FormattingEnabled = true;
            this.mcomboBox_methods.Location = new System.Drawing.Point(3, 9);
            this.mcomboBox_methods.Name = "mcomboBox_methods";
            this.mcomboBox_methods.Size = new System.Drawing.Size(179, 21);
            this.mcomboBox_methods.TabIndex = 0;
            //
            // mbutton_remove
            //
            this.mbutton_remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_remove.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.mbutton_remove.Location = new System.Drawing.Point(187, 113);
            this.mbutton_remove.Name = "mbutton_remove";
            this.mbutton_remove.Size = new System.Drawing.Size(54, 32);
            this.mbutton_remove.TabIndex = 5;
            this.mbutton_remove.UseVisualStyleBackColor = true;
            this.mbutton_remove.Click += new System.EventHandler(this.mbutton_remove_Click);
            //
            // mbutton_up
            //
            this.mbutton_up.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_up.Image = global::LcmsNet.Properties.Resources.Button_Up_16;
            this.mbutton_up.Location = new System.Drawing.Point(188, 43);
            this.mbutton_up.Name = "mbutton_up";
            this.mbutton_up.Size = new System.Drawing.Size(54, 30);
            this.mbutton_up.TabIndex = 3;
            this.mbutton_up.UseVisualStyleBackColor = true;
            this.mbutton_up.Click += new System.EventHandler(this.mbutton_up_Click);
            //
            // mbutton_down
            //
            this.mbutton_down.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_down.Image = global::LcmsNet.Properties.Resources.Button_Down_16;
            this.mbutton_down.Location = new System.Drawing.Point(188, 79);
            this.mbutton_down.Name = "mbutton_down";
            this.mbutton_down.Size = new System.Drawing.Size(54, 28);
            this.mbutton_down.TabIndex = 4;
            this.mbutton_down.UseVisualStyleBackColor = true;
            this.mbutton_down.Click += new System.EventHandler(this.mbutton_down_Click);
            //
            // mbutton_add
            //
            this.mbutton_add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_add.Image = global::LcmsNet.Properties.Resources.add;
            this.mbutton_add.Location = new System.Drawing.Point(188, 5);
            this.mbutton_add.Name = "mbutton_add";
            this.mbutton_add.Size = new System.Drawing.Size(54, 30);
            this.mbutton_add.TabIndex = 1;
            this.mbutton_add.UseVisualStyleBackColor = true;
            this.mbutton_add.Click += new System.EventHandler(this.mbutton_add_Click);
            //
            // mlistBox_methods
            //
            this.mlistBox_methods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistBox_methods.FormattingEnabled = true;
            this.mlistBox_methods.Location = new System.Drawing.Point(4, 34);
            this.mlistBox_methods.Name = "mlistBox_methods";
            this.mlistBox_methods.ScrollAlwaysVisible = true;
            this.mlistBox_methods.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.mlistBox_methods.Size = new System.Drawing.Size(178, 225);
            this.mlistBox_methods.TabIndex = 2;
            //
            // controlLCMethodSelection
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mlistBox_methods);
            this.Controls.Add(this.mbutton_down);
            this.Controls.Add(this.mbutton_up);
            this.Controls.Add(this.mbutton_remove);
            this.Controls.Add(this.mbutton_add);
            this.Controls.Add(this.mcomboBox_methods);
            this.Name = "controlLCMethodSelection";
            this.Size = new System.Drawing.Size(249, 269);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox mcomboBox_methods;
        private System.Windows.Forms.Button mbutton_add;
        private System.Windows.Forms.Button mbutton_remove;
        private System.Windows.Forms.Button mbutton_up;
        private System.Windows.Forms.Button mbutton_down;
        private System.Windows.Forms.ListBox mlistBox_methods;
    }
}
