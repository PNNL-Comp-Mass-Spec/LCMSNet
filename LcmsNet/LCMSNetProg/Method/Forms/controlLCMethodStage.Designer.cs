using System.ComponentModel;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    partial class controlLCMethodStage
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
            this.mpanel_stage = new System.Windows.Forms.Panel();
            this.mbutton_moveUp = new System.Windows.Forms.Button();
            this.mbutton_moveDown = new System.Windows.Forms.Button();
            this.mbutton_deleteEvent = new System.Windows.Forms.Button();
            this.mbutton_addEvent = new System.Windows.Forms.Button();
            this.mbutton_selectAll = new System.Windows.Forms.Button();
            this.mbutton_deselectAll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mcomboBox_column = new System.Windows.Forms.ComboBox();
            this.mcheckBox_postOverlap = new System.Windows.Forms.CheckBox();
            this.mcheckBox_preOverlap = new System.Windows.Forms.CheckBox();
            this.mpanel_control = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.mbutton_save = new System.Windows.Forms.Button();
            this.mbutton_buildUpdate = new System.Windows.Forms.Button();
            this.mbutton_load = new System.Windows.Forms.Button();
            this.mbutton_saveAll = new System.Windows.Forms.Button();
            this.mlabel_methodNameDescription = new System.Windows.Forms.Label();
            this.mbutton_build = new System.Windows.Forms.Button();
            this.mtextBox_methodName = new System.Windows.Forms.TextBox();
            this.mcomboBox_savedMethods = new System.Windows.Forms.ComboBox();
            this.mpanel_control.SuspendLayout();
            this.SuspendLayout();
            //
            // mpanel_stage
            //
            this.mpanel_stage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mpanel_stage.AutoScroll = true;
            this.mpanel_stage.BackColor = System.Drawing.Color.White;
            this.mpanel_stage.Location = new System.Drawing.Point(3, 82);
            this.mpanel_stage.Name = "mpanel_stage";
            this.mpanel_stage.Size = new System.Drawing.Size(794, 427);
            this.mpanel_stage.TabIndex = 11;
            //
            // mbutton_moveUp
            //
            this.mbutton_moveUp.Image = global::LcmsNet.Properties.Resources.Button_Up_16;
            this.mbutton_moveUp.Location = new System.Drawing.Point(184, 47);
            this.mbutton_moveUp.Name = "mbutton_moveUp";
            this.mbutton_moveUp.Size = new System.Drawing.Size(55, 27);
            this.mbutton_moveUp.TabIndex = 5;
            this.mbutton_moveUp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_moveUp.UseVisualStyleBackColor = true;
            this.mbutton_moveUp.Click += new System.EventHandler(this.mbutton_moveUp_Click);
            //
            // mbutton_moveDown
            //
            this.mbutton_moveDown.Image = global::LcmsNet.Properties.Resources.Button_Down_16;
            this.mbutton_moveDown.Location = new System.Drawing.Point(128, 47);
            this.mbutton_moveDown.Name = "mbutton_moveDown";
            this.mbutton_moveDown.Size = new System.Drawing.Size(54, 27);
            this.mbutton_moveDown.TabIndex = 4;
            this.mbutton_moveDown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_moveDown.UseVisualStyleBackColor = true;
            this.mbutton_moveDown.Click += new System.EventHandler(this.mbutton_moveDown_Click);
            //
            // mbutton_deleteEvent
            //
            this.mbutton_deleteEvent.Image = global::LcmsNet.Properties.Resources.Button_Delete_16;
            this.mbutton_deleteEvent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_deleteEvent.Location = new System.Drawing.Point(64, 47);
            this.mbutton_deleteEvent.Name = "mbutton_deleteEvent";
            this.mbutton_deleteEvent.Size = new System.Drawing.Size(58, 27);
            this.mbutton_deleteEvent.TabIndex = 2;
            this.mbutton_deleteEvent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_deleteEvent.UseVisualStyleBackColor = true;
            this.mbutton_deleteEvent.Click += new System.EventHandler(this.mbutton_deleteEvent_Click);
            //
            // mbutton_addEvent
            //
            this.mbutton_addEvent.Image = global::LcmsNet.Properties.Resources.add;
            this.mbutton_addEvent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_addEvent.Location = new System.Drawing.Point(3, 47);
            this.mbutton_addEvent.Name = "mbutton_addEvent";
            this.mbutton_addEvent.Size = new System.Drawing.Size(59, 27);
            this.mbutton_addEvent.TabIndex = 1;
            this.mbutton_addEvent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_addEvent.UseVisualStyleBackColor = true;
            this.mbutton_addEvent.Click += new System.EventHandler(this.mbutton_addEvent_Click);
            //
            // mbutton_selectAll
            //
            this.mbutton_selectAll.Location = new System.Drawing.Point(642, 47);
            this.mbutton_selectAll.Name = "mbutton_selectAll";
            this.mbutton_selectAll.Size = new System.Drawing.Size(66, 27);
            this.mbutton_selectAll.TabIndex = 9;
            this.mbutton_selectAll.Text = "Select All";
            this.mbutton_selectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_selectAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.mbutton_selectAll.UseVisualStyleBackColor = true;
            this.mbutton_selectAll.Click += new System.EventHandler(this.mbutton_selectDeselectAll_Click);
            //
            // mbutton_deselectAll
            //
            this.mbutton_deselectAll.Location = new System.Drawing.Point(714, 47);
            this.mbutton_deselectAll.Name = "mbutton_deselectAll";
            this.mbutton_deselectAll.Size = new System.Drawing.Size(72, 27);
            this.mbutton_deselectAll.TabIndex = 10;
            this.mbutton_deselectAll.Text = "Deselect All";
            this.mbutton_deselectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_deselectAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.mbutton_deselectAll.UseVisualStyleBackColor = true;
            this.mbutton_deselectAll.Click += new System.EventHandler(this.mbutton_deselectAll_Click);
            //
            // label1
            //
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(255, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 18);
            this.label1.TabIndex = 26;
            this.label1.Text = "Column:";
            //
            // mcomboBox_column
            //
            this.mcomboBox_column.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_column.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcomboBox_column.FormattingEnabled = true;
            this.mcomboBox_column.Location = new System.Drawing.Point(303, 55);
            this.mcomboBox_column.Name = "mcomboBox_column";
            this.mcomboBox_column.Size = new System.Drawing.Size(148, 21);
            this.mcomboBox_column.TabIndex = 6;
            this.mcomboBox_column.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_column_SelectedIndexChanged);
            //
            // mcheckBox_postOverlap
            //
            this.mcheckBox_postOverlap.AutoSize = true;
            this.mcheckBox_postOverlap.Location = new System.Drawing.Point(546, 58);
            this.mcheckBox_postOverlap.Name = "mcheckBox_postOverlap";
            this.mcheckBox_postOverlap.Size = new System.Drawing.Size(87, 17);
            this.mcheckBox_postOverlap.TabIndex = 8;
            this.mcheckBox_postOverlap.Text = "Post-Overlap";
            this.mcheckBox_postOverlap.UseVisualStyleBackColor = true;
            this.mcheckBox_postOverlap.CheckedChanged += new System.EventHandler(this.mcheckBox_postOverlap_CheckedChanged);
            //
            // mcheckBox_preOverlap
            //
            this.mcheckBox_preOverlap.AutoSize = true;
            this.mcheckBox_preOverlap.Location = new System.Drawing.Point(463, 57);
            this.mcheckBox_preOverlap.Name = "mcheckBox_preOverlap";
            this.mcheckBox_preOverlap.Size = new System.Drawing.Size(82, 17);
            this.mcheckBox_preOverlap.TabIndex = 7;
            this.mcheckBox_preOverlap.Text = "Pre-Overlap";
            this.mcheckBox_preOverlap.UseVisualStyleBackColor = true;
            this.mcheckBox_preOverlap.CheckedChanged += new System.EventHandler(this.mcheckBox_preOverlap_CheckedChanged);
            //
            // mpanel_control
            //
            this.mpanel_control.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mpanel_control.Controls.Add(this.button3);
            this.mpanel_control.Controls.Add(this.button2);
            this.mpanel_control.Controls.Add(this.button1);
            this.mpanel_control.Location = new System.Drawing.Point(-1, 515);
            this.mpanel_control.Name = "mpanel_control";
            this.mpanel_control.Size = new System.Drawing.Size(802, 36);
            this.mpanel_control.TabIndex = 27;
            //
            // button3
            //
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(752, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(44, 30);
            this.button3.TabIndex = 2;
            this.button3.Text = "Stop";
            this.button3.UseVisualStyleBackColor = true;
            //
            // button2
            //
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(703, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(44, 30);
            this.button2.TabIndex = 1;
            this.button2.Text = "Back";
            this.button2.UseVisualStyleBackColor = true;
            //
            // button1
            //
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(653, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Step";
            this.button1.UseVisualStyleBackColor = true;
            //
            // mbutton_save
            //
            this.mbutton_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_save.Enabled = false;
            this.mbutton_save.Image = global::LcmsNet.Properties.Resources.SaveWithIndicator;
            this.mbutton_save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_save.Location = new System.Drawing.Point(642, 11);
            this.mbutton_save.Name = "mbutton_save";
            this.mbutton_save.Size = new System.Drawing.Size(77, 30);
            this.mbutton_save.TabIndex = 35;
            this.mbutton_save.Text = "Save";
            this.mbutton_save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_save.UseVisualStyleBackColor = true;
            this.mbutton_save.Click += new System.EventHandler(this.mbutton_save_Click);
            //
            // mbutton_buildUpdate
            //
            this.mbutton_buildUpdate.BackColor = System.Drawing.Color.White;
            this.mbutton_buildUpdate.Enabled = false;
            this.mbutton_buildUpdate.ForeColor = System.Drawing.Color.Black;
            this.mbutton_buildUpdate.Image = global::LcmsNet.Properties.Resources.BuildUpdate;
            this.mbutton_buildUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_buildUpdate.Location = new System.Drawing.Point(448, 10);
            this.mbutton_buildUpdate.Name = "mbutton_buildUpdate";
            this.mbutton_buildUpdate.Size = new System.Drawing.Size(72, 33);
            this.mbutton_buildUpdate.TabIndex = 34;
            this.mbutton_buildUpdate.Text = "Update";
            this.mbutton_buildUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_buildUpdate.UseVisualStyleBackColor = false;
            this.mbutton_buildUpdate.Click += new System.EventHandler(this.mbutton_buildUpdate_Click);
            //
            // mbutton_load
            //
            this.mbutton_load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_load.Image = global::LcmsNet.Properties.Resources.Open;
            this.mbutton_load.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_load.Location = new System.Drawing.Point(574, 11);
            this.mbutton_load.Name = "mbutton_load";
            this.mbutton_load.Size = new System.Drawing.Size(62, 30);
            this.mbutton_load.TabIndex = 32;
            this.mbutton_load.Text = "Load ";
            this.mbutton_load.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_load.UseVisualStyleBackColor = true;
            this.mbutton_load.Click += new System.EventHandler(this.mbutton_load_Click);
            //
            // mbutton_saveAll
            //
            this.mbutton_saveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_saveAll.Enabled = false;
            this.mbutton_saveAll.Image = global::LcmsNet.Properties.Resources.SaveWithIndicator;
            this.mbutton_saveAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_saveAll.Location = new System.Drawing.Point(722, 10);
            this.mbutton_saveAll.Name = "mbutton_saveAll";
            this.mbutton_saveAll.Size = new System.Drawing.Size(77, 30);
            this.mbutton_saveAll.TabIndex = 33;
            this.mbutton_saveAll.Text = "Save All";
            this.mbutton_saveAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_saveAll.UseVisualStyleBackColor = true;
            this.mbutton_saveAll.Click += new System.EventHandler(this.mbutton_saveAll_Click);
            //
            // mlabel_methodNameDescription
            //
            this.mlabel_methodNameDescription.ForeColor = System.Drawing.Color.Blue;
            this.mlabel_methodNameDescription.Location = new System.Drawing.Point(9, 16);
            this.mlabel_methodNameDescription.Name = "mlabel_methodNameDescription";
            this.mlabel_methodNameDescription.Size = new System.Drawing.Size(92, 17);
            this.mlabel_methodNameDescription.TabIndex = 31;
            this.mlabel_methodNameDescription.Text = "Method Name:";
            //
            // mbutton_build
            //
            this.mbutton_build.BackColor = System.Drawing.Color.White;
            this.mbutton_build.ForeColor = System.Drawing.Color.Black;
            this.mbutton_build.Image = global::LcmsNet.Properties.Resources.Build;
            this.mbutton_build.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_build.Location = new System.Drawing.Point(370, 10);
            this.mbutton_build.Name = "mbutton_build";
            this.mbutton_build.Size = new System.Drawing.Size(72, 33);
            this.mbutton_build.TabIndex = 30;
            this.mbutton_build.Text = "Build";
            this.mbutton_build.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_build.UseVisualStyleBackColor = false;
            this.mbutton_build.Click += new System.EventHandler(this.mbutton_build_Click);
            //
            // mtextBox_methodName
            //
            this.mtextBox_methodName.Location = new System.Drawing.Point(245, 14);
            this.mtextBox_methodName.Name = "mtextBox_methodName";
            this.mtextBox_methodName.Size = new System.Drawing.Size(119, 20);
            this.mtextBox_methodName.TabIndex = 29;
            this.mtextBox_methodName.Text = "lcMethod1";
            this.mtextBox_methodName.TextChanged += new System.EventHandler(this.mtextBox_methodName_TextChanged);
            //
            // mcomboBox_savedMethods
            //
            this.mcomboBox_savedMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_savedMethods.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcomboBox_savedMethods.FormattingEnabled = true;
            this.mcomboBox_savedMethods.Location = new System.Drawing.Point(107, 13);
            this.mcomboBox_savedMethods.Name = "mcomboBox_savedMethods";
            this.mcomboBox_savedMethods.Size = new System.Drawing.Size(132, 21);
            this.mcomboBox_savedMethods.TabIndex = 28;
            this.mcomboBox_savedMethods.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_savedMethods_SelectedIndexChanged);
            //
            // controlLCMethodStage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mbutton_save);
            this.Controls.Add(this.mbutton_buildUpdate);
            this.Controls.Add(this.mbutton_load);
            this.Controls.Add(this.mbutton_saveAll);
            this.Controls.Add(this.mlabel_methodNameDescription);
            this.Controls.Add(this.mbutton_build);
            this.Controls.Add(this.mtextBox_methodName);
            this.Controls.Add(this.mcomboBox_savedMethods);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mcomboBox_column);
            this.Controls.Add(this.mcheckBox_postOverlap);
            this.Controls.Add(this.mcheckBox_preOverlap);
            this.Controls.Add(this.mbutton_deselectAll);
            this.Controls.Add(this.mbutton_selectAll);
            this.Controls.Add(this.mbutton_moveUp);
            this.Controls.Add(this.mbutton_moveDown);
            this.Controls.Add(this.mbutton_deleteEvent);
            this.Controls.Add(this.mbutton_addEvent);
            this.Controls.Add(this.mpanel_stage);
            this.Controls.Add(this.mpanel_control);
            this.Name = "controlLCMethodStage";
            this.Size = new System.Drawing.Size(800, 551);
            this.mpanel_control.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel mpanel_stage;
        private Button mbutton_addEvent;
        private Button mbutton_deleteEvent;
        private Button mbutton_moveUp;
        private Button mbutton_moveDown;
        private Button mbutton_selectAll;
        private Button mbutton_deselectAll;
        private Label label1;
        private ComboBox mcomboBox_column;
        private CheckBox mcheckBox_postOverlap;
        private CheckBox mcheckBox_preOverlap;
        private Panel mpanel_control;
        private Button button3;
        private Button button2;
        private Button button1;
        private Button mbutton_save;
        private Button mbutton_buildUpdate;
        private Button mbutton_load;
        private Button mbutton_saveAll;
        private Label mlabel_methodNameDescription;
        private Button mbutton_build;
        private TextBox mtextBox_methodName;
        private ComboBox mcomboBox_savedMethods;
    }
}
