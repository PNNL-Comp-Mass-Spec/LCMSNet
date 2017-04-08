using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    partial class formColumnSampleProgress
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
            this.components = new System.ComponentModel.Container();
            this.mlabel_previewMinutes = new System.Windows.Forms.Label();
            this.mlabel_fullPreview = new System.Windows.Forms.Label();
            this.mnum_previewMinutes = new System.Windows.Forms.NumericUpDown();
            this.mlabel_display = new System.Windows.Forms.Label();
            this.m_previewTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.milliseconds = new System.Windows.Forms.NumericUpDown();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.mcontrol_sampleProgressFull = new LcmsNet.Method.Forms.controlSampleProgress();
            this.mcontrol_sampleProgress = new LcmsNet.Method.Forms.controlSampleProgress();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_previewMinutes)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.milliseconds)).BeginInit();
            this.SuspendLayout();
            //
            // mlabel_previewMinutes
            //
            this.mlabel_previewMinutes.AutoSize = true;
            this.mlabel_previewMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_previewMinutes.Location = new System.Drawing.Point(12, 7);
            this.mlabel_previewMinutes.Name = "mlabel_previewMinutes";
            this.mlabel_previewMinutes.Size = new System.Drawing.Size(116, 16);
            this.mlabel_previewMinutes.TabIndex = 2;
            this.mlabel_previewMinutes.Text = "30-minute-preview";
            //
            // mlabel_fullPreview
            //
            this.mlabel_fullPreview.Dock = System.Windows.Forms.DockStyle.Top;
            this.mlabel_fullPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_fullPreview.Location = new System.Drawing.Point(5, 368);
            this.mlabel_fullPreview.Name = "mlabel_fullPreview";
            this.mlabel_fullPreview.Size = new System.Drawing.Size(1232, 13);
            this.mlabel_fullPreview.TabIndex = 3;
            this.mlabel_fullPreview.Text = "Four Column Preview";
            //
            // mnum_previewMinutes
            //
            this.mnum_previewMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_previewMinutes.Location = new System.Drawing.Point(964, 8);
            this.mnum_previewMinutes.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.mnum_previewMinutes.Name = "mnum_previewMinutes";
            this.mnum_previewMinutes.Size = new System.Drawing.Size(70, 20);
            this.mnum_previewMinutes.TabIndex = 4;
            this.mnum_previewMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_previewMinutes.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.mnum_previewMinutes.ValueChanged += new System.EventHandler(this.mnum_previewMinutes_ValueChanged);
            //
            // mlabel_display
            //
            this.mlabel_display.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_display.AutoSize = true;
            this.mlabel_display.Location = new System.Drawing.Point(761, 9);
            this.mlabel_display.Name = "mlabel_display";
            this.mlabel_display.Size = new System.Drawing.Size(197, 13);
            this.mlabel_display.TabIndex = 5;
            this.mlabel_display.Text = "Display (minutes : seconds: milliseconds)";
            //
            // m_previewTimer
            //
            this.m_previewTimer.Enabled = true;
            this.m_previewTimer.Interval = 1000;
            this.m_previewTimer.Tick += new System.EventHandler(this.m_previewTimer_Tick);
            //
            // panel1
            //
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.milliseconds);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.mlabel_previewMinutes);
            this.panel1.Controls.Add(this.mlabel_display);
            this.panel1.Controls.Add(this.mnum_previewMinutes);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1232, 37);
            this.panel1.TabIndex = 7;
            //
            // label1
            //
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1040, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = ":";
            //
            // numericUpDown1
            //
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown1.Location = new System.Drawing.Point(1056, 8);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            //
            // label2
            //
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1133, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = ":";
            //
            // milliseconds
            //
            this.milliseconds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.milliseconds.Location = new System.Drawing.Point(1149, 8);
            this.milliseconds.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.milliseconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.milliseconds.Name = "milliseconds";
            this.milliseconds.Size = new System.Drawing.Size(70, 20);
            this.milliseconds.TabIndex = 9;
            this.milliseconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.milliseconds.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.milliseconds.ValueChanged += new System.EventHandler(this.milliseconds_ValueChanged);
            //
            // splitter1
            //
            this.splitter1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(5, 365);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1232, 3);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            //
            // mcontrol_sampleProgressFull
            //
            this.mcontrol_sampleProgressFull.BackColor = System.Drawing.Color.White;
            this.mcontrol_sampleProgressFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_sampleProgressFull.Location = new System.Drawing.Point(5, 381);
            this.mcontrol_sampleProgressFull.Name = "mcontrol_sampleProgressFull";
            this.mcontrol_sampleProgressFull.PreviewMilliseconds = 0;
            this.mcontrol_sampleProgressFull.PreviewMinutes = 30;
            this.mcontrol_sampleProgressFull.PreviewSeconds = 1;
            this.mcontrol_sampleProgressFull.RenderAllEvents = false;
            this.mcontrol_sampleProgressFull.RenderCurrent = false;
            this.mcontrol_sampleProgressFull.RenderDisplayWindow = false;
            this.mcontrol_sampleProgressFull.Size = new System.Drawing.Size(1232, 369);
            this.mcontrol_sampleProgressFull.TabIndex = 1;
            //
            // mcontrol_sampleProgress
            //
            this.mcontrol_sampleProgress.BackColor = System.Drawing.Color.White;
            this.mcontrol_sampleProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.mcontrol_sampleProgress.Location = new System.Drawing.Point(5, 42);
            this.mcontrol_sampleProgress.Name = "mcontrol_sampleProgress";
            this.mcontrol_sampleProgress.PreviewMilliseconds = 0;
            this.mcontrol_sampleProgress.PreviewMinutes = 30;
            this.mcontrol_sampleProgress.PreviewSeconds = 1;
            this.mcontrol_sampleProgress.RenderAllEvents = false;
            this.mcontrol_sampleProgress.RenderCurrent = false;
            this.mcontrol_sampleProgress.RenderDisplayWindow = false;
            this.mcontrol_sampleProgress.Size = new System.Drawing.Size(1232, 323);
            this.mcontrol_sampleProgress.TabIndex = 0;
            //
            // formColumnSampleProgress
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1242, 755);
            this.Controls.Add(this.mcontrol_sampleProgressFull);
            this.Controls.Add(this.mlabel_fullPreview);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.mcontrol_sampleProgress);
            this.Controls.Add(this.panel1);
            this.Name = "formColumnSampleProgress";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Separation Progress";
            ((System.ComponentModel.ISupportInitialize)(this.mnum_previewMinutes)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.milliseconds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private controlSampleProgress mcontrol_sampleProgress;
        private controlSampleProgress mcontrol_sampleProgressFull;
        private Label mlabel_previewMinutes;
        private Label mlabel_fullPreview;
        private NumericUpDown mnum_previewMinutes;
        private Label mlabel_display;
        private Timer m_previewTimer;
        private Panel panel1;
        private Label label1;
        private NumericUpDown numericUpDown1;
        private Label label2;
        private NumericUpDown milliseconds;
        private Splitter splitter1;


    }
}