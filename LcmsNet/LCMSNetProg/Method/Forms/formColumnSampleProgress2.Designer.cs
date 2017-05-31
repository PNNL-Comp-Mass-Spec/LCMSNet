using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    partial class formColumnSampleProgress2
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
            this.columnSampleProgressViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.columnSampleProgressView = new LcmsNet.Method.Views.ColumnSampleProgressView();
            this.SuspendLayout();
            //
            // columnSampleProgressViewHost
            //
            this.columnSampleProgressViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnSampleProgressViewHost.Location = new System.Drawing.Point(5, 5);
            this.columnSampleProgressViewHost.Name = "columnSampleProgressViewHost";
            this.columnSampleProgressViewHost.Size = new System.Drawing.Size(1232, 745);
            this.columnSampleProgressViewHost.TabIndex = 0;
            this.columnSampleProgressViewHost.Text = "columnSampleProgressViewHost";
            this.columnSampleProgressViewHost.Child = this.columnSampleProgressView;
            //
            // formColumnSampleProgress2
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1242, 755);
            this.Controls.Add(this.columnSampleProgressViewHost);
            this.Name = "formColumnSampleProgress2";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Separation Progress";
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Integration.ElementHost columnSampleProgressViewHost;
        private Method.Views.ColumnSampleProgressView columnSampleProgressView;
    }
}