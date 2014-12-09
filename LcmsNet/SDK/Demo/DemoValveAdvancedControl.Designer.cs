namespace DemoPluginLibrary
{
    partial class DemoValveAdvancedControl
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
            this.btnStateA = new System.Windows.Forms.Button();
            this.btnStateB = new System.Windows.Forms.Button();
            this.txtState = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStateA
            // 
            this.btnStateA.Location = new System.Drawing.Point(35, 20);
            this.btnStateA.Name = "btnStateA";
            this.btnStateA.Size = new System.Drawing.Size(75, 23);
            this.btnStateA.TabIndex = 0;
            this.btnStateA.Text = "A";
            this.btnStateA.UseVisualStyleBackColor = true;
            this.btnStateA.Click += new System.EventHandler(this.btnStateA_Click);
            // 
            // btnStateB
            // 
            this.btnStateB.Location = new System.Drawing.Point(35, 49);
            this.btnStateB.Name = "btnStateB";
            this.btnStateB.Size = new System.Drawing.Size(75, 23);
            this.btnStateB.TabIndex = 1;
            this.btnStateB.Text = "B";
            this.btnStateB.UseVisualStyleBackColor = true;
            this.btnStateB.Click += new System.EventHandler(this.btnStateB_Click);
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(137, 23);
            this.txtState.Name = "txtState";
            this.txtState.ReadOnly = true;
            this.txtState.Size = new System.Drawing.Size(49, 20);
            this.txtState.TabIndex = 2;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(137, 49);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(54, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(144, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "State:";
            // 
            // TestValveAdvancedControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtState);
            this.Controls.Add(this.btnStateB);
            this.Controls.Add(this.btnStateA);
            this.Name = "TestValveAdvancedControl";
            this.Size = new System.Drawing.Size(208, 91);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStateA;
        private System.Windows.Forms.Button btnStateB;
        private System.Windows.Forms.TextBox txtState;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label1;
    }
}
