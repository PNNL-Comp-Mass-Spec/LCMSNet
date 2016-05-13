namespace WindowsFormsApplication2
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlValveVICI2Pos1 = new WindowsFormsApplication2.controlValveVICI2Pos();
            this.SuspendLayout();
            // 
            // controlValveVICI2Pos1
            // 
            this.controlValveVICI2Pos1.Location = new System.Drawing.Point(12, 23);
            this.controlValveVICI2Pos1.Name = "controlValveVICI2Pos1";
            this.controlValveVICI2Pos1.Size = new System.Drawing.Size(294, 312);
            this.controlValveVICI2Pos1.TabIndex = 0;
            this.controlValveVICI2Pos1.Valve = null;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(411, 515);
            this.Controls.Add(this.controlValveVICI2Pos1);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private controlValveVICI2Pos controlValveVICI2Pos1;
    }
}

