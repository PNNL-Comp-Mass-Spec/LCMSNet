
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/08/2010
//
// Last modified 02/08/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LogViewer
{
	public partial class formLogViewerMain : Form
	{
		//*********************************************************************************************************
		// Main for for SQLite log viewer
		//**********************************************************************************************************

		#region "Constants"
		#endregion

		#region "Class variables"
			classViewerSqlTools mobj_SqlTools;
		#endregion

		#region "Delegates"
		#endregion

		#region "Events"
		#endregion

		#region "Properties"
		#endregion

		#region "Constructors"
			public formLogViewerMain()
			{
				InitializeComponent();

				mobj_SqlTools = new classViewerSqlTools();

                dateTimePickStart.Value = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).AddHours(-2);
                dateTimePickStop.Value = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).AddHours(2);
			}
		#endregion

		#region "Methods"
			/// <summary>
			/// Displays all log entries from the log file
			/// </summary>
			private void GetLogEntries()
			{
				// Verify a log file has been specified
				if (textLogFile.Text == "")
				{
					string msg = "Log file must be specified";
					MessageBox.Show(msg);
					return;
				}

				// Verify specified log file exists (in case file string has been manually edited)
				if (!File.Exists(textLogFile.Text))
				{
					string msg = "Log file " + textLogFile.Text + " not found";
					MessageBox.Show(msg);
				}

				// Fill a query data object from the selected fields in the screen
				classLogQueryData queryData = new classLogQueryData();
				queryData.Column = textColumn.Text;
				queryData.Device = textDevice.Text;
				queryData.Message = textMessage.Text;
				queryData.Sample = textSampleName.Text;
				queryData.StartTime = dateTimePickStart.Value.ToString("MM/dd/yyyy HH:mm:ss");
				queryData.StopTime = dateTimePickStop.Value.ToString("MM/dd/yyyy HH:mm:ss");
				queryData.Type = textType.Text;

				DataTable logData = null;
				try
				{
					logData = mobj_SqlTools.GetLogEntries(queryData);
				}
				catch (Exception ex)
				{
					string msg = "";
					UnwrapExceptionMsgs(ex, out msg);
					MessageBox.Show(msg);
				}

				if (logData == null)
				{
					string msg = "Returned log data list is NULL";
					MessageBox.Show(msg);
					return;
				}

				datagridLogContents.DataSource = bindingSource_LogData;
				bindingSource_LogData.DataSource = logData;
				datagridLogContents.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
			}	

			/// <summary>
			/// Unwraps nested exceptions to retrieve innermost exception message
			/// </summary>
			/// <param name="ex">Outermost exception</param>
			/// <param name="msg">Input/Output message</param>
			private static void UnwrapExceptionMsgs(Exception ex, out string msg)
			{
				msg = ex.Message;
				if (!(ex.InnerException == null))
				{
					string innerMsg;
					UnwrapExceptionMsgs(ex.InnerException, out innerMsg);
					msg += " Inner exception: " + innerMsg;
				}
			}	
		#endregion

		#region "Event handlers"
		/// <summary>
		/// Selects the database file to view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLogFileSelect_Click(object sender, EventArgs e)
		{
			OpenFileDialog fileOpenDlg = new OpenFileDialog();
			fileOpenDlg.Filter = "Log files (*.db3)|*.db3|All files(*.*)|*.*";
			fileOpenDlg.FilterIndex = 0;
			if (fileOpenDlg.ShowDialog() == DialogResult.OK)
			{
				textLogFile.Text = fileOpenDlg.FileName;
				mobj_SqlTools.DataFileNamePath = textLogFile.Text;
			}

		}	

		/// <summary>
		/// Updates the display of the selected log
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonFindLogEntries_Click(object sender, EventArgs e)
		{
			GetLogEntries();
		}
		#endregion
	}	
}	// End namespace
