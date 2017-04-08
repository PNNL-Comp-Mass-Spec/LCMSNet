using System.ComponentModel;
using System.Windows.Forms;
using LcmsNet.Method.Forms;

namespace LcmsNet.Simulator
{
    partial class SimControlsAndChartsControl
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SimControlsAndChartsControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlSimulator = new System.Windows.Forms.TabControl();
            this.tabPageMethodSimulatorControls = new System.Windows.Forms.TabPage();
            this.btnUntackChart = new System.Windows.Forms.Button();
            this.tabControlCharts = new System.Windows.Forms.TabControl();
            this.tabPageGantt = new System.Windows.Forms.TabPage();
            this.controlLCMethodTimeline1 = new LcmsNet.Method.Forms.controlLCMethodTimeline();
            this.tabPagConversation = new System.Windows.Forms.TabPage();
            this.controlLCMethodTimeline2 = new LcmsNet.Method.Forms.controlLCMethodTimeline();
            this.tabPageErrors = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.mcontrol_selectedMethods = new LcmsNet.Method.Forms.controlLCMethodSelection();
            this.tabSimulatorSettings = new System.Windows.Forms.TabPage();
            this.mgroupBox_update = new System.Windows.Forms.GroupBox();
            this.mnum_delay = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTack = new System.Windows.Forms.Button();
            this.tabEditor = new System.Windows.Forms.TabControl();
            this.tabMethods = new System.Windows.Forms.TabPage();
            this.controlLCMethodStage2 = new LcmsNet.Method.Forms.controlLCMethodStage();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlSimulator.SuspendLayout();
            this.tabPageMethodSimulatorControls.SuspendLayout();
            this.tabControlCharts.SuspendLayout();
            this.tabPageGantt.SuspendLayout();
            this.tabPagConversation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabSimulatorSettings.SuspendLayout();
            this.mgroupBox_update.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_delay)).BeginInit();
            this.tabEditor.SuspendLayout();
            this.tabMethods.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer1
            //
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.tabControlSimulator);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.Controls.Add(this.btnTack);
            this.splitContainer1.Panel2.Controls.Add(this.tabEditor);
            this.splitContainer1.Size = new System.Drawing.Size(830, 528);
            this.splitContainer1.SplitterDistance = 287;
            this.splitContainer1.TabIndex = 0;
            //
            // tabControlSimulator
            //
            this.tabControlSimulator.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlSimulator.Controls.Add(this.tabPageMethodSimulatorControls);
            this.tabControlSimulator.Controls.Add(this.tabSimulatorSettings);
            this.tabControlSimulator.Location = new System.Drawing.Point(2, 3);
            this.tabControlSimulator.Name = "tabControlSimulator";
            this.tabControlSimulator.SelectedIndex = 0;
            this.tabControlSimulator.Size = new System.Drawing.Size(823, 279);
            this.tabControlSimulator.TabIndex = 9;
            //
            // tabPageMethodSimulatorControls
            //
            this.tabPageMethodSimulatorControls.Controls.Add(this.btnUntackChart);
            this.tabPageMethodSimulatorControls.Controls.Add(this.tabControlCharts);
            this.tabPageMethodSimulatorControls.Controls.Add(this.groupBox1);
            this.tabPageMethodSimulatorControls.Controls.Add(this.mcontrol_selectedMethods);
            this.tabPageMethodSimulatorControls.Location = new System.Drawing.Point(4, 22);
            this.tabPageMethodSimulatorControls.Name = "tabPageMethodSimulatorControls";
            this.tabPageMethodSimulatorControls.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMethodSimulatorControls.Size = new System.Drawing.Size(815, 253);
            this.tabPageMethodSimulatorControls.TabIndex = 0;
            this.tabPageMethodSimulatorControls.Text = "Method";
            this.tabPageMethodSimulatorControls.UseVisualStyleBackColor = true;
            //
            // btnUntackChart
            //
            this.btnUntackChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUntackChart.Location = new System.Drawing.Point(7, 226);
            this.btnUntackChart.Name = "btnUntackChart";
            this.btnUntackChart.Size = new System.Drawing.Size(32, 23);
            this.btnUntackChart.TabIndex = 1;
            this.btnUntackChart.UseVisualStyleBackColor = true;
            this.btnUntackChart.Click += new System.EventHandler(this.btnUntackChart_Click);
            //
            // tabControlCharts
            //
            this.tabControlCharts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlCharts.Controls.Add(this.tabPageGantt);
            this.tabControlCharts.Controls.Add(this.tabPagConversation);
            this.tabControlCharts.Controls.Add(this.tabPageErrors);
            this.tabControlCharts.Location = new System.Drawing.Point(0, 0);
            this.tabControlCharts.Name = "tabControlCharts";
            this.tabControlCharts.SelectedIndex = 0;
            this.tabControlCharts.Size = new System.Drawing.Size(583, 227);
            this.tabControlCharts.TabIndex = 1;
            //
            // tabPageGantt
            //
            this.tabPageGantt.Controls.Add(this.controlLCMethodTimeline1);
            this.tabPageGantt.Location = new System.Drawing.Point(4, 22);
            this.tabPageGantt.Name = "tabPageGantt";
            this.tabPageGantt.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGantt.Size = new System.Drawing.Size(401, 201);
            this.tabPageGantt.TabIndex = 0;
            this.tabPageGantt.Text = "Gantt";
            this.tabPageGantt.UseVisualStyleBackColor = true;
            //
            // controlLCMethodTimeline1
            //
            this.controlLCMethodTimeline1.AutoScroll = true;
            this.controlLCMethodTimeline1.BackColor = System.Drawing.Color.White;
            this.controlLCMethodTimeline1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlLCMethodTimeline1.Location = new System.Drawing.Point(3, 3);
            this.controlLCMethodTimeline1.Name = "controlLCMethodTimeline1";
            this.controlLCMethodTimeline1.RenderMode = LcmsNet.Method.enumLCMethodRenderMode.Column;
            this.controlLCMethodTimeline1.Size = new System.Drawing.Size(395, 195);
            this.controlLCMethodTimeline1.StartEventIndex = 0;
            this.controlLCMethodTimeline1.TabIndex = 0;
            //
            // tabPagConversation
            //
            this.tabPagConversation.Controls.Add(this.controlLCMethodTimeline2);
            this.tabPagConversation.Location = new System.Drawing.Point(4, 22);
            this.tabPagConversation.Name = "tabPagConversation";
            this.tabPagConversation.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagConversation.Size = new System.Drawing.Size(575, 201);
            this.tabPagConversation.TabIndex = 1;
            this.tabPagConversation.Text = "Conversation";
            this.tabPagConversation.UseVisualStyleBackColor = true;
            //
            // controlLCMethodTimeline2
            //
            this.controlLCMethodTimeline2.AutoScroll = true;
            this.controlLCMethodTimeline2.BackColor = System.Drawing.Color.White;
            this.controlLCMethodTimeline2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlLCMethodTimeline2.Location = new System.Drawing.Point(3, 3);
            this.controlLCMethodTimeline2.Name = "controlLCMethodTimeline2";
            this.controlLCMethodTimeline2.RenderMode = LcmsNet.Method.enumLCMethodRenderMode.Conversation;
            this.controlLCMethodTimeline2.Size = new System.Drawing.Size(569, 195);
            this.controlLCMethodTimeline2.StartEventIndex = 0;
            this.controlLCMethodTimeline2.TabIndex = 0;
            //
            // tabPageErrors
            //
            this.tabPageErrors.Location = new System.Drawing.Point(4, 22);
            this.tabPageErrors.Name = "tabPageErrors";
            this.tabPageErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageErrors.Size = new System.Drawing.Size(401, 201);
            this.tabPageErrors.TabIndex = 2;
            this.tabPageErrors.Text = "Errors";
            this.tabPageErrors.UseVisualStyleBackColor = true;
            //
            // groupBox1
            //
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnStep);
            this.groupBox1.Controls.Add(this.btnPause);
            this.groupBox1.Controls.Add(this.btnPlay);
            this.groupBox1.Location = new System.Drawing.Point(581, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(231, 51);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simulator Controls";
            //
            // btnReset
            //
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.Location = new System.Drawing.Point(8, 13);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(37, 24);
            this.btnReset.TabIndex = 11;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnStop
            //
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.Location = new System.Drawing.Point(180, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(37, 24);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "button1";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // btnStep
            //
            this.btnStep.Image = ((System.Drawing.Image)(resources.GetObject("btnStep.Image")));
            this.btnStep.Location = new System.Drawing.Point(51, 13);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(37, 24);
            this.btnStep.TabIndex = 8;
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            //
            // btnPause
            //
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.Location = new System.Drawing.Point(137, 12);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(37, 24);
            this.btnPause.TabIndex = 7;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            //
            // btnPlay
            //
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.Location = new System.Drawing.Point(94, 12);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(37, 24);
            this.btnPlay.TabIndex = 6;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            //
            // mcontrol_selectedMethods
            //
            this.mcontrol_selectedMethods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_selectedMethods.Location = new System.Drawing.Point(581, 49);
            this.mcontrol_selectedMethods.Name = "mcontrol_selectedMethods";
            this.mcontrol_selectedMethods.Size = new System.Drawing.Size(231, 216);
            this.mcontrol_selectedMethods.TabIndex = 0;
            //
            // tabSimulatorSettings
            //
            this.tabSimulatorSettings.AutoScroll = true;
            this.tabSimulatorSettings.Controls.Add(this.mgroupBox_update);
            this.tabSimulatorSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSimulatorSettings.Name = "tabSimulatorSettings";
            this.tabSimulatorSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSimulatorSettings.Size = new System.Drawing.Size(815, 253);
            this.tabSimulatorSettings.TabIndex = 1;
            this.tabSimulatorSettings.Text = "Simulator Settings";
            this.tabSimulatorSettings.UseVisualStyleBackColor = true;
            //
            // mgroupBox_update
            //
            this.mgroupBox_update.AutoSize = true;
            this.mgroupBox_update.Controls.Add(this.mnum_delay);
            this.mgroupBox_update.Controls.Add(this.label4);
            this.mgroupBox_update.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mgroupBox_update.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_update.Location = new System.Drawing.Point(3, 3);
            this.mgroupBox_update.Name = "mgroupBox_update";
            this.mgroupBox_update.Size = new System.Drawing.Size(809, 247);
            this.mgroupBox_update.TabIndex = 14;
            this.mgroupBox_update.TabStop = false;
            this.mgroupBox_update.Text = "Settings";
            //
            // mnum_delay
            //
            this.mnum_delay.Location = new System.Drawing.Point(199, 37);
            this.mnum_delay.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.mnum_delay.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.mnum_delay.Name = "mnum_delay";
            this.mnum_delay.Size = new System.Drawing.Size(60, 20);
            this.mnum_delay.TabIndex = 13;
            this.mnum_delay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.mnum_delay.ValueChanged += new System.EventHandler(this.mnum_delay_ValueChanged);
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(179, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Delay between event simulation (ms)";
            //
            // btnTack
            //
            this.btnTack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTack.Location = new System.Drawing.Point(782, 205);
            this.btnTack.Name = "btnTack";
            this.btnTack.Size = new System.Drawing.Size(39, 23);
            this.btnTack.TabIndex = 1;
            this.btnTack.UseVisualStyleBackColor = true;
            this.btnTack.Click += new System.EventHandler(this.btnTack_Click);
            //
            // tabEditor
            //
            this.tabEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabEditor.Controls.Add(this.tabMethods);
            this.tabEditor.Location = new System.Drawing.Point(6, 15);
            this.tabEditor.Name = "tabEditor";
            this.tabEditor.SelectedIndex = 0;
            this.tabEditor.Size = new System.Drawing.Size(819, 184);
            this.tabEditor.TabIndex = 0;
            //
            // tabMethods
            //
            this.tabMethods.Controls.Add(this.controlLCMethodStage2);
            this.tabMethods.Location = new System.Drawing.Point(4, 22);
            this.tabMethods.Name = "tabMethods";
            this.tabMethods.Padding = new System.Windows.Forms.Padding(3);
            this.tabMethods.Size = new System.Drawing.Size(811, 158);
            this.tabMethods.TabIndex = 2;
            this.tabMethods.Text = "Methods";
            this.tabMethods.UseVisualStyleBackColor = true;
            //
            // controlLCMethodStage2
            //
            this.controlLCMethodStage2.AllowPostOverlap = false;
            this.controlLCMethodStage2.AllowPreOverlap = false;
            this.controlLCMethodStage2.BackColor = System.Drawing.Color.White;
            this.controlLCMethodStage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controlLCMethodStage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlLCMethodStage2.Location = new System.Drawing.Point(3, 3);
            this.controlLCMethodStage2.MethodFolderPath = "LCMethods";
            this.controlLCMethodStage2.Name = "controlLCMethodStage2";
            this.controlLCMethodStage2.Size = new System.Drawing.Size(805, 152);
            this.controlLCMethodStage2.TabIndex = 0;
            //
            // SimControlsAndChartsControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SimControlsAndChartsControl";
            this.Size = new System.Drawing.Size(830, 528);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlSimulator.ResumeLayout(false);
            this.tabPageMethodSimulatorControls.ResumeLayout(false);
            this.tabControlCharts.ResumeLayout(false);
            this.tabPageGantt.ResumeLayout(false);
            this.tabPagConversation.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabSimulatorSettings.ResumeLayout(false);
            this.tabSimulatorSettings.PerformLayout();
            this.mgroupBox_update.ResumeLayout(false);
            this.mgroupBox_update.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_delay)).EndInit();
            this.tabEditor.ResumeLayout(false);
            this.tabMethods.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private GroupBox groupBox1;
        private Button btnTack;
        private TabControl tabEditor;
        private TabPage tabMethods;
        private Button btnReset;
        private Button btnStop;
        private Button btnStep;
        private Button btnPause;
        private Button btnPlay;
        private TabControl tabControlSimulator;
        private TabPage tabPageMethodSimulatorControls;
        private controlLCMethodSelection mcontrol_selectedMethods;
        private TabPage tabSimulatorSettings;
        private GroupBox mgroupBox_update;
        private NumericUpDown mnum_delay;
        private Label label4;
        private TabControl tabControlCharts;
        private TabPage tabPageGantt;
        private TabPage tabPagConversation;
        private controlLCMethodStage controlLCMethodStage2;
        private controlLCMethodTimeline controlLCMethodTimeline1;
        private controlLCMethodTimeline controlLCMethodTimeline2;
        private Button btnUntackChart;
        private TabPage tabPageErrors;
    }
}
