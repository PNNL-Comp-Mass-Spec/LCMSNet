namespace LcmsNet.Method.Forms
{
    partial class formMethodEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private LcmsNet.Method.Forms.controlLCMethodEditor mcontrol_methodEditor;

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
            LcmsNet.Method.classMethodPreviewOptions classMethodPreviewOptions1 = new LcmsNet.Method.classMethodPreviewOptions();
            this.mcontrol_methodEditor = new LcmsNet.Method.Forms.controlLCMethodEditor();
            this.SuspendLayout();
            //
            // mcontrol_methodEditor
            //
            this.mcontrol_methodEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mcontrol_methodEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_methodEditor.Location = new System.Drawing.Point(0, 0);
            this.mcontrol_methodEditor.Margin = new System.Windows.Forms.Padding(5);
            this.mcontrol_methodEditor.MethodFolderPath = "LCMethods";
            classMethodPreviewOptions1.Animate = false;
            classMethodPreviewOptions1.AnimateDelay = 50;
            classMethodPreviewOptions1.FrameDelay = 5;
            this.mcontrol_methodEditor.MethodPreviewOptions = classMethodPreviewOptions1;
            this.mcontrol_methodEditor.Name = "mcontrol_methodEditor";
            this.mcontrol_methodEditor.Padding = new System.Windows.Forms.Padding(5);
            this.mcontrol_methodEditor.Size = new System.Drawing.Size(818, 840);
            this.mcontrol_methodEditor.TabIndex = 0;
            //
            // formMethodEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 840);
            this.Controls.Add(this.mcontrol_methodEditor);
            this.Name = "formMethodEditor";
            this.Text = "LC Method Editor";
            this.ResumeLayout(false);

        }

        #endregion


    }
}