using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    partial class controlParameterNumericUpDown
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mnum_value = new System.Windows.Forms.NumericUpDown();
            this.mbutton_conversion = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_value)).BeginInit();
            this.SuspendLayout();
            //
            // mnum_value
            //
            this.mnum_value.DecimalPlaces = 1;
            this.mnum_value.Location = new System.Drawing.Point(2, 0);
            this.mnum_value.Name = "mnum_value";
            this.mnum_value.Size = new System.Drawing.Size(108, 20);
            this.mnum_value.TabIndex = 0;
            this.mnum_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // mbutton_conversion
            //
            this.mbutton_conversion.Location = new System.Drawing.Point(113, 1);
            this.mbutton_conversion.Name = "mbutton_conversion";
            this.mbutton_conversion.Size = new System.Drawing.Size(32, 19);
            this.mbutton_conversion.TabIndex = 1;
            this.mbutton_conversion.Text = "..";
            this.mbutton_conversion.UseVisualStyleBackColor = true;
            this.mbutton_conversion.Click += new System.EventHandler(this.mbutton_conversion_Click);
            //
            // controlParameterNumericUpDown
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mbutton_conversion);
            this.Controls.Add(this.mnum_value);
            this.Name = "controlParameterNumericUpDown";
            this.Size = new System.Drawing.Size(148, 20);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_value)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NumericUpDown mnum_value;
        private Button mbutton_conversion;
    }
}
