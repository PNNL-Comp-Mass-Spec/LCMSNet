using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LcmsNet.Method;
using LcmsNet.Method.Forms;
using LcmsNetDataClasses.Method;
using FluidicsSimulator;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses;

namespace LcmsNet.Simulator
{
    public partial class SimControlsAndChartsControl : UserControl
    {
        private static SimControlsAndChartsControl m_instance;

        private bool m_tacked;
        private DateTime mdatetime_startTime;
        private readonly FluidicsSimulator.FluidicsSimulator simInstance;

        private SimControlsAndChartsControl()
        {
            InitializeComponent();
            m_tacked = true;
            simInstance = FluidicsSimulator.FluidicsSimulator.GetInstance;
            simInstance.EventExecuting += simInstance_EventExecuting;
            simInstance.EventSimulated += simInstance_EventExecuting;
            simInstance.SimulationComplete += simInstance_SimulationComplete;
            var reporter = new ModelCheckReportViewer(FluidicsSDK.classFluidicsModerator.Moderator);
            reporter.Name = "errorReporter";
            reporter.Dock = DockStyle.Fill;
            tabControlCharts.TabPages["tabPageErrors"].Controls.Add(reporter);
            mcontrol_selectedMethods.MethodAdded +=
                mcontrol_selectedMethods_MethodAdded;
            mcontrol_selectedMethods.MethodDeleted +=
                mcontrol_selectedMethods_MethodDeleted;
            mcontrol_selectedMethods.MethodUpdated +=
                mcontrol_selectedMethods_MethodUpdated;
            var settings = tabControlSimulator.TabPages["tabSimulatorSettings"];


            var checkList = new ModelCheckListControl(FluidicsSDK.classFluidicsModerator.Moderator,
                FluidicsSDK.classFluidicsModerator.Moderator.GetModelCheckers());
            checkList.Location = new Point(10, 75);
            settings.Controls["mgroupBox_update"].Controls.Add(checkList);
        }

        public static SimControlsAndChartsControl GetInstance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SimControlsAndChartsControl();
                }
                return m_instance;
            }
        }

        public bool Tacked
        {
            get { return m_tacked; }
            private set
            {
                m_tacked = value;
                Tack?.Invoke(this, new TackEventArgs(m_tacked));
            }
        }

        /// <summary>
        /// Gets or sets the method animation preview options.
        /// </summary>
        public classMethodPreviewOptions MethodPreviewOptions { get; set; }

        public event EventHandler<TackEventArgs> Tack;
        public event EventHandler EventChanged;

        private void simInstance_EventExecuting(object sender, SimulatedEventArgs e)
        {
            var methods = mcontrol_selectedMethods.SelectedMethods;
            var index = -1;
            var count = 0;
            var optimizer = new classLCMethodOptimizer();
            optimizer.AlignMethods(methods);
            var queue = FluidicsSimulator.FluidicsSimulator.BuildEventList(methods, methods[0].Start);
            foreach (var lst in queue)
            {
                if (lst.Exists(evnt => evnt.Name == e.Event.Name && evnt.Start == e.Event.Start))
                {
                    index = count;
                }
                else
                {
                    ++count;
                }
            }
            controlLCMethodTimeline2.StartEventIndex = index;
            RenderAlignMethods();
            Refresh();
        }

        private void simInstance_SimulationComplete(object sender, ModelStatusChangeEventArgs e)
        {
            if (e.StatusList.Count != 0)
            {
                var statusMessages = "";
                foreach (var status in e.StatusList)
                {
                    classApplicationLogger.LogError(0, status.Description);
                    statusMessages += status.Description + Environment.NewLine;
                }
                MessageBox.Show(statusMessages);
            }
            Refresh();
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            EventChanged?.Invoke(this, null);
        }

        public void TackOnRequest()
        {
            // we don't used Tacked here to avoid recursion.
            m_tacked = true;
        }


        private void btnTack_Click(object sender, EventArgs e)
        {
            Tacked = !Tacked;
        }

        /// <summary>
        /// Renders the throughput timelines.
        /// </summary>
        /// <param name="methods">Methods to render for showing throughput.</param>
        private void RenderThroughput(List<classLCMethod> methods)
        {
            if (methods == null)
                return;

            controlLCMethodTimeline1.RenderLCMethod(methods);
            controlLCMethodTimeline1.Invalidate();

            //TODO: CHange the name to say conversation...
            controlLCMethodTimeline2.RenderLCMethod(methods);
            controlLCMethodTimeline2.Invalidate();
        }


        /// <summary>
        /// Aligns then renders the methods selected from the user interface.
        /// </summary>
        private void RenderAlignMethods()
        {
            //
            // Align methods - blindly!
            //
            try
            {
                var optimizer = new classLCMethodOptimizer();
                //optimizer.UpdateRequired += new classLCMethodOptimizer.DelegateUpdateUserInterface(optimizer_UpdateRequired);
                var methods = mcontrol_selectedMethods.SelectedMethods;
                //m_renderUpdateCount = 0;

                if (methods.Count > 0)
                {
                    if (mdatetime_startTime == null)
                    {
                        mdatetime_startTime = LcmsNetSDK.TimeKeeper.Instance.Now;
                            // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
                    }
                    else
                    {
                        methods[0].SetStartTime(mdatetime_startTime);
                    }
                }
                optimizer.AlignMethods(methods);
                RenderThroughput(methods);

                //optimizer.UpdateRequired -= optimizer_UpdateRequired;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// On an update, the optimizer will tell us the status of the methods here.
        /// </summary>
        void optimizer_UpdateRequired(classLCMethodOptimizer sender)
        {
            //
            // Render the updates if we animating and past the rendering threshold.
            //
            //if (MethodPreviewOptions.Animate == true && //m_renderUpdateCount++ >= MethodPreviewOptions.FrameDelay)
            //{
            //    RenderThroughput(sender.Methods);
            //    Refresh();
            //    Application.DoEvents();

            //    ///
            //    /// reset the number of update calls we have seen.
            //    ///
            //    //m_renderUpdateCount = 0;

            //    System.Threading.Thread.Sleep(MethodPreviewOptions.AnimateDelay);
            //}
        }


        private void PrepSim()
        {
            var optimizer = new classLCMethodOptimizer();
            var methodsToRun = mcontrol_selectedMethods.SelectedMethods;
            optimizer.AlignMethods(methodsToRun);
            RenderThroughput(methodsToRun);
            simInstance.PrepSimulation(methodsToRun);
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            if (ConfirmEmulation())
            {
                // if the simulator reports ready, take a step.
                if (simInstance.IsReady)
                {
                    simInstance.Step();
                }
                else
                {
                    PrepSim();
                    mdatetime_startTime = LcmsNetSDK.TimeKeeper.Instance.Now;
                        // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
                    simInstance.Step();
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            controlLCMethodTimeline2.StartEventIndex = 0;
            PrepSim();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (ConfirmEmulation())
            {
                if (simInstance.IsReady && !simInstance.InProgress)
                {
                    simInstance.Simulate();
                }
                else
                {
                    PrepSim();
                    mdatetime_startTime = LcmsNetSDK.TimeKeeper.Instance.Now;
                        // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
                    simInstance.Simulate();
                }
            }
        }

        private bool ConfirmNoEmulation()
        {
            var runSimulation = true;
            var dialog = new frmEmulationDialog();
            var result = dialog.ShowDialog();

            switch (result)
            {
                case DialogResult.Yes:
                    classLCMSSettings.SetParameter(classLCMSSettings.PARAM_EMULATIONENABLED, "True");
                    return runSimulation;

                case DialogResult.No:
                    return runSimulation;

                case DialogResult.Cancel:
                    runSimulation = false;
                    return runSimulation;
            }
            return runSimulation;
        }

        private bool ConfirmEmulation()
        {
            if (!Convert.ToBoolean(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_EMULATIONENABLED)))
            {
                return ConfirmNoEmulation();
            }
            return true;
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void mcontrol_selectedMethods_MethodDeleted(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void mcontrol_selectedMethods_MethodAdded(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void mcontrol_selectedMethods_MethodUpdated(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Finds the associated method from the list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private classLCMethod FindMethods(string name)
        {
            classLCMethod method = null;
            if (classLCMethodManager.Manager.Methods.ContainsKey(name))
            {
                method = classLCMethodManager.Manager.Methods[name];
            }
            return method;
        }

        /// <summary>
        /// Saves the given method to file.
        /// </summary>
        /// <param name="method"></param>
        public bool SaveMethod(classLCMethod method)
        {
            //
            // Method is null!!! OH MAN - that isn't my fault so we'll ignore it!
            //
            if (method == null)
                return false;

            //
            // Create a new writer.
            //
            var writer = new classLCMethodWriter();

            //
            // Construct the path
            //
            var path = System.IO.Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH),
                classLCMethodFactory.CONST_LC_METHOD_FOLDER);
            path = System.IO.Path.Combine(path, method.Name + classLCMethodFactory.CONST_LC_METHOD_EXTENSION);

            //
            // Write the method out!
            //
            return writer.WriteMethod(path, method);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            simInstance.StopSimulation();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            simInstance.StopSimulation();
            simInstance.ClearSimulator();
        }

        private void mnum_delay_ValueChanged(object sender, EventArgs e)
        {
            var s = sender as NumericUpDown;
            simInstance.SimulationSpeed = Convert.ToInt32(s.Value);
        }

        private void btnUntackChart_Click(object sender, EventArgs e)
        {
            // if there are tabs to the tab control, a chart must exist, popout the one that's currently visible to the user
            if (tabControlCharts.TabCount > 0)
            {
                //grab the first chart from the list
                var untackThisControl = tabControlCharts.SelectedTab.Controls.OfType<UserControl>().First();
                var popout = new formChartPopoutWindow(untackThisControl, tabControlCharts);
                popout.Text = tabControlCharts.SelectedTab.Text;
                tabControlCharts.SelectedTab.Controls.Remove(untackThisControl);
                tabControlCharts.Controls.Remove(tabControlCharts.SelectedTab);
                popout.Show();
            }
        }
    }
}