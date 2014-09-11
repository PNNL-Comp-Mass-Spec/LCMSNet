
namespace ExternalFileWatcher.Watchers
{
    partial class controlExternalFileWatcher
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
            this.mbutton_setPath = new System.Windows.Forms.Button();
            this.mtextBox_path = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mutton_browse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.mnum_minutesToWait = new System.Windows.Forms.NumericUpDown();
            this.mbutton_setMinutes = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.mtextBox_extension = new System.Windows.Forms.TextBox();
            this.mbutton_setExtension = new System.Windows.Forms.Button();
            this.mbutton_setDirectoryExtension = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.mtext_directoryExtension = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minutesToWait)).BeginInit();
            this.SuspendLayout();
            
            // 
            // mbutton_setPath
            // 
            this.mbutton_setPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_setPath.Location = new System.Drawing.Point(388, 57);
            this.mbutton_setPath.Name = "mbutton_setPath";
            this.mbutton_setPath.Size = new System.Drawing.Size(63, 31);
            this.mbutton_setPath.TabIndex = 0;
            this.mbutton_setPath.Text = "Set";
            this.mbutton_setPath.UseVisualStyleBackColor = true;
            this.mbutton_setPath.Click += new System.EventHandler(this.mbutton_setPath_Click);
            // 
            // mtextBox_path
            // 
            this.mtextBox_path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_path.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.mtextBox_path.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.mtextBox_path.Location = new System.Drawing.Point(10, 23);
            this.mtextBox_path.Name = "mtextBox_path";
            this.mtextBox_path.Size = new System.Drawing.Size(358, 20);
            this.mtextBox_path.TabIndex = 1;
            this.mtextBox_path.Text = "c:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Path";
            // 
            // mutton_browse
            // 
            this.mutton_browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mutton_browse.Location = new System.Drawing.Point(388, 17);
            this.mutton_browse.Name = "mutton_browse";
            this.mutton_browse.Size = new System.Drawing.Size(63, 31);
            this.mutton_browse.TabIndex = 3;
            this.mutton_browse.Text = "...";
            this.mutton_browse.UseVisualStyleBackColor = true;
            this.mutton_browse.Click += new System.EventHandler(this.mutton_browse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Minutes to wait after file size has not changed.";
            // 
            // mnum_minutesToWait
            // 
            this.mnum_minutesToWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_minutesToWait.Location = new System.Drawing.Point(251, 111);
            this.mnum_minutesToWait.Name = "mnum_minutesToWait";
            this.mnum_minutesToWait.Size = new System.Drawing.Size(117, 20);
            this.mnum_minutesToWait.TabIndex = 6;
            this.mnum_minutesToWait.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_setMinutes
            // 
            this.mbutton_setMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_setMinutes.Location = new System.Drawing.Point(388, 104);
            this.mbutton_setMinutes.Name = "mbutton_setMinutes";
            this.mbutton_setMinutes.Size = new System.Drawing.Size(63, 31);
            this.mbutton_setMinutes.TabIndex = 7;
            this.mbutton_setMinutes.Text = "Set";
            this.mbutton_setMinutes.UseVisualStyleBackColor = true;
            this.mbutton_setMinutes.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "File extension for data file";
            // 
            // mtextBox_extension
            // 
            this.mtextBox_extension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtextBox_extension.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.mtextBox_extension.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.mtextBox_extension.Location = new System.Drawing.Point(249, 149);
            this.mtextBox_extension.Name = "mtextBox_extension";
            this.mtextBox_extension.Size = new System.Drawing.Size(119, 20);
            this.mtextBox_extension.TabIndex = 8;
            this.mtextBox_extension.Text = ".txt";
            this.mtextBox_extension.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mbutton_setExtension
            // 
            this.mbutton_setExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_setExtension.Location = new System.Drawing.Point(388, 143);
            this.mbutton_setExtension.Name = "mbutton_setExtension";
            this.mbutton_setExtension.Size = new System.Drawing.Size(63, 31);
            this.mbutton_setExtension.TabIndex = 10;
            this.mbutton_setExtension.Text = "Set";
            this.mbutton_setExtension.UseVisualStyleBackColor = true;
            this.mbutton_setExtension.Click += new System.EventHandler(this.mbutton_setExtension_Click);
            // 
            // mbutton_setDirectoryExtension
            // 
            this.mbutton_setDirectoryExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_setDirectoryExtension.Location = new System.Drawing.Point(388, 181);
            this.mbutton_setDirectoryExtension.Name = "mbutton_setDirectoryExtension";
            this.mbutton_setDirectoryExtension.Size = new System.Drawing.Size(63, 31);
            this.mbutton_setDirectoryExtension.TabIndex = 13;
            this.mbutton_setDirectoryExtension.Text = "Set";
            this.mbutton_setDirectoryExtension.UseVisualStyleBackColor = true;
            this.mbutton_setDirectoryExtension.Click += new System.EventHandler(this.mbutton_setDirectoryExtension_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Directory Extension";
            // 
            // mtext_directoryExtension
            // 
            this.mtext_directoryExtension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtext_directoryExtension.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.mtext_directoryExtension.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.mtext_directoryExtension.Location = new System.Drawing.Point(249, 187);
            this.mtext_directoryExtension.Name = "mtext_directoryExtension";
            this.mtext_directoryExtension.Size = new System.Drawing.Size(119, 20);
            this.mtext_directoryExtension.TabIndex = 11;
            this.mtext_directoryExtension.Text = ".d";
            this.mtext_directoryExtension.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // controlExternalFileWatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mbutton_setDirectoryExtension);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mtext_directoryExtension);
            this.Controls.Add(this.mbutton_setExtension);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mtextBox_extension);
            this.Controls.Add(this.mbutton_setMinutes);
            this.Controls.Add(this.mnum_minutesToWait);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mutton_browse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mtextBox_path);
            this.Controls.Add(this.mbutton_setPath);
            this.Name = "controlExternalFileWatcher";
            this.Size = new System.Drawing.Size(454, 319);
            this.Controls.SetChildIndex(this.mbutton_setPath, 0);
            this.Controls.SetChildIndex(this.mtextBox_path, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.mutton_browse, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.mnum_minutesToWait, 0);
            this.Controls.SetChildIndex(this.mbutton_setMinutes, 0);
            this.Controls.SetChildIndex(this.mtextBox_extension, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.mbutton_setExtension, 0);
            this.Controls.SetChildIndex(this.mtext_directoryExtension, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.mbutton_setDirectoryExtension, 0);
            
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minutesToWait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mbutton_setPath;
        private System.Windows.Forms.TextBox mtextBox_path;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mutton_browse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown mnum_minutesToWait;
        private System.Windows.Forms.Button mbutton_setMinutes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mtextBox_extension;
        private System.Windows.Forms.Button mbutton_setExtension;
        private System.Windows.Forms.Button mbutton_setDirectoryExtension;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox mtext_directoryExtension;
    }
}
