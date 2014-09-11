namespace LcmsNetSampleQueue.Forms
{
    partial class controlColumnList
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
			  this.mlistview_samples = new System.Windows.Forms.ListView();
			  this.mcolumnHeader_sampleID = new System.Windows.Forms.ColumnHeader();
			  this.mcolumnHeader_name = new System.Windows.Forms.ColumnHeader();
			  this.mcolumnHeader_palMethod = new System.Windows.Forms.ColumnHeader();
			  this.mcolumnHeader_experimentName = new System.Windows.Forms.ColumnHeader();
			  this.mcolumnHeader_emslRequest = new System.Windows.Forms.ColumnHeader();
			  this.mbutton_dmsImport = new System.Windows.Forms.Button();
			  this.SuspendLayout();
			  // 
			  // mlistview_samples
			  // 
			  this.mlistview_samples.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							  | System.Windows.Forms.AnchorStyles.Left)
							  | System.Windows.Forms.AnchorStyles.Right)));
			  this.mlistview_samples.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mcolumnHeader_sampleID,
            this.mcolumnHeader_name,
            this.mcolumnHeader_palMethod,
            this.mcolumnHeader_experimentName,
            this.mcolumnHeader_emslRequest});
			  this.mlistview_samples.GridLines = true;
			  this.mlistview_samples.Location = new System.Drawing.Point(3, 45);
			  this.mlistview_samples.Name = "mlistview_samples";
			  this.mlistview_samples.Size = new System.Drawing.Size(481, 491);
			  this.mlistview_samples.TabIndex = 2;
			  this.mlistview_samples.UseCompatibleStateImageBehavior = false;
			  this.mlistview_samples.View = System.Windows.Forms.View.Details;
			  // 
			  // mcolumnHeader_sampleID
			  // 
			  this.mcolumnHeader_sampleID.Text = "ID";
			  // 
			  // mcolumnHeader_name
			  // 
			  this.mcolumnHeader_name.Text = "Name";
			  this.mcolumnHeader_name.Width = 68;
			  // 
			  // mcolumnHeader_palMethod
			  // 
			  this.mcolumnHeader_palMethod.Text = "PAL Method";
			  // 
			  // mcolumnHeader_experimentName
			  // 
			  this.mcolumnHeader_experimentName.Text = "Experiment Name";
			  // 
			  // mcolumnHeader_emslRequest
			  // 
			  this.mcolumnHeader_emslRequest.Text = "Request #";
			  // 
			  // mbutton_dmsImport
			  // 
			  this.mbutton_dmsImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			  this.mbutton_dmsImport.Location = new System.Drawing.Point(325, 3);
			  this.mbutton_dmsImport.Name = "mbutton_dmsImport";
			  this.mbutton_dmsImport.Size = new System.Drawing.Size(159, 36);
			  this.mbutton_dmsImport.TabIndex = 3;
			  this.mbutton_dmsImport.Text = "DMS Import";
			  this.mbutton_dmsImport.UseVisualStyleBackColor = true;
			  this.mbutton_dmsImport.Click += new System.EventHandler(this.mbutton_dmsImport_Click);
			  // 
			  // controlColumnList
			  // 
			  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			  this.Controls.Add(this.mbutton_dmsImport);
			  this.Controls.Add(this.mlistview_samples);
			  this.Name = "controlColumnList";
			  this.Size = new System.Drawing.Size(487, 539);
			  this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mlistview_samples;
        private System.Windows.Forms.ColumnHeader mcolumnHeader_sampleID;
        private System.Windows.Forms.ColumnHeader mcolumnHeader_name;
        private System.Windows.Forms.ColumnHeader mcolumnHeader_palMethod;
        private System.Windows.Forms.ColumnHeader mcolumnHeader_experimentName;
        private System.Windows.Forms.ColumnHeader mcolumnHeader_emslRequest;
        private System.Windows.Forms.Button mbutton_dmsImport;
    }
}
