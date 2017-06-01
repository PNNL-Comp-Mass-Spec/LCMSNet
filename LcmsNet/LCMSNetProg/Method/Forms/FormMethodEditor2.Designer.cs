using System.ComponentModel;

namespace LcmsNet.Method.Forms
{
    partial class formMethodEditor2
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
            this.lcMethodEditorViewHost = new System.Windows.Forms.Integration.ElementHost();
            this.lcMethodEditorView = new LcmsNet.Method.Views.LCMethodEditorView();
            this.SuspendLayout();
            // 
            // lcMethodEditorViewHost
            // 
            this.lcMethodEditorViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcMethodEditorViewHost.Location = new System.Drawing.Point(0, 0);
            this.lcMethodEditorViewHost.Name = "lcMethodEditorViewHost";
            this.lcMethodEditorViewHost.Size = new System.Drawing.Size(818, 840);
            this.lcMethodEditorViewHost.TabIndex = 0;
            this.lcMethodEditorViewHost.Text = "lcMethodEditorViewHost";
            this.lcMethodEditorViewHost.Child = lcMethodEditorView;
            // 
            // formMethodEditor2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 840);
            this.Controls.Add(this.lcMethodEditorViewHost);
            this.Name = "formMethodEditor2";
            this.Text = "LC Method Editor";
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Integration.ElementHost lcMethodEditorViewHost;
        private LcmsNet.Method.Views.LCMethodEditorView lcMethodEditorView;
    }
}