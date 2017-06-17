using System;
using System.Collections.Generic;
using System.Reactive;
using System.Windows;
using FluidicsSDK;
using FluidicsSimulator;
using LcmsNet.Devices.ViewModels;
using LcmsNet.Method;
using LcmsNet.Method.ViewModels;
using LcmsNet.Simulator.Views;
using LcmsNetCommonControls.ViewModels;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSDK;
using ReactiveUI;

namespace LcmsNet.Simulator.ViewModels
{
    public class ChartViewModel : ReactiveObject
    {
        public ChartViewModel()
        {
            simInstance = FluidicsSimulator.FluidicsSimulator.GetInstance;
            simInstance.EventExecuting += SimInstance_EventExecuting;
            simInstance.EventSimulated += SimInstance_EventExecuting;
            simInstance.SimulationComplete += SimInstance_SimulationComplete;

            selectedMethods = new LCMethodSelectionViewModel();
            selectedMethods.MethodAdded += SelectedMethods_MethodAdded;
            selectedMethods.MethodDeleted += SelectedMethods_MethodDeleted;
            selectedMethods.MethodUpdated += SelectedMethods_MethodUpdated;

            reporter = new ModelCheckReportsViewModel(classFluidicsModerator.Moderator);
            ganttChartTimelineVm = new LCMethodTimelineViewModel();
            ganttChartTimelineVm.RenderMode = enumLCMethodRenderMode.Column;
            conversationChartTimelineVm = new LCMethodTimelineViewModel();
            conversationChartTimelineVm.RenderMode = enumLCMethodRenderMode.Conversation;

            reporterPopoutVm = new PopoutViewModel(reporter);
            ganttChartTimelinePopoutVm = new PopoutViewModel(ganttChartTimelineVm);
            conversationChartTimelinePopoutVm = new PopoutViewModel(conversationChartTimelineVm);
        }

        private DateTime startTime;
        private readonly FluidicsSimulator.FluidicsSimulator simInstance;
        private readonly LCMethodSelectionViewModel selectedMethods;
        private readonly ModelCheckReportsViewModel reporter;
        private readonly LCMethodTimelineViewModel ganttChartTimelineVm;
        private readonly LCMethodTimelineViewModel conversationChartTimelineVm;
        private readonly PopoutViewModel reporterPopoutVm;
        private readonly PopoutViewModel ganttChartTimelinePopoutVm;
        private readonly PopoutViewModel conversationChartTimelinePopoutVm;

        public LCMethodSelectionViewModel SelectedMethods => selectedMethods;
        public ModelCheckReportsViewModel Reporter => reporter;
        public LCMethodTimelineViewModel GanttChartTimelineVm => ganttChartTimelineVm;
        public LCMethodTimelineViewModel ConversationChartTimelineVm => conversationChartTimelineVm;
        public PopoutViewModel ReporterPopoutVm => reporterPopoutVm;
        public PopoutViewModel GanttChartTimelinePopoutVm => ganttChartTimelinePopoutVm;
        public PopoutViewModel ConversationChartTimelinePopoutVm => conversationChartTimelinePopoutVm;

        public event EventHandler<TackEventArgs> Tack;

        /// <summary>
        /// Aligns then renders the methods selected from the user interface.
        /// </summary>
        private void RenderAlignMethods()
        {
            // Align methods - blindly!
            try
            {
                var optimizer = new classLCMethodOptimizer();
                var methods = selectedMethods.SelectedMethods;

                if (methods.Count > 0)
                {
                    methods[0].SetStartTime(startTime);
                }
                optimizer.AlignMethods(methods);
                RenderThroughput(methods);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        private void SelectedMethods_MethodDeleted(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        private void SelectedMethods_MethodAdded(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        private void SelectedMethods_MethodUpdated(object sender)
        {
            RenderAlignMethods();
        }

        private void Reset()
        {
            GanttChartTimelineVm.StartEventIndex = 0;
            PrepSim();
        }

        private void Play()
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
                    startTime = TimeKeeper.Instance.Now;
                    // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
                    simInstance.Simulate();
                }
            }
        }

        private void Step()
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
                    startTime = TimeKeeper.Instance.Now;
                    // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
                    simInstance.Step();
                }
            }
        }

        private void Pause()
        {
            simInstance.StopSimulation();
        }

        private void Stop()
        {
            simInstance.StopSimulation();
            simInstance.ClearSimulator();
        }

        private void SimInstance_EventExecuting(object sender, SimulatedEventArgs e)
        {
            var methods = selectedMethods.SelectedMethods;
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
            GanttChartTimelineVm.StartEventIndex = index;
            RenderAlignMethods();
        }

        private void SimInstance_SimulationComplete(object sender, ModelStatusChangeEventArgs e)
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
        }

        /// <summary>
        /// Renders the throughput timelines.
        /// </summary>
        /// <param name="methods">Methods to render for showing throughput.</param>
        private void RenderThroughput(List<classLCMethod> methods)
        {
            if (methods == null)
                return;

            ConversationChartTimelineVm.RenderLCMethod(methods);
            ConversationChartTimelineVm.Refresh();

            GanttChartTimelineVm.RenderLCMethod(methods);
            GanttChartTimelineVm.Refresh();
        }

        private void PrepSim()
        {
            var optimizer = new classLCMethodOptimizer();
            var methodsToRun = selectedMethods.SelectedMethods;
            optimizer.AlignMethods(methodsToRun);
            RenderThroughput(methodsToRun);
            simInstance.PrepSimulation(methodsToRun);
        }

        private bool ConfirmNoEmulation()
        {
            var runSimulation = true;
            var dialog = new EmulationConfirmDialog();
            dialog.ShowDialog();

            switch (dialog.Result)
            {
                case MessageBoxResult.Yes:
                    classLCMSSettings.SetParameter(classLCMSSettings.PARAM_EMULATIONENABLED, "True");
                    return runSimulation;

                case MessageBoxResult.No:
                    return runSimulation;

                case MessageBoxResult.Cancel:
                    runSimulation = false;
                    return runSimulation;
            }
            return runSimulation;
        }

        private bool ConfirmEmulation()
        {
            if (!classLCMSSettings.GetParameter(classLCMSSettings.PARAM_EMULATIONENABLED, false))
            {
                return ConfirmNoEmulation();
            }
            return true;
        }

        public ReactiveCommand<Unit, Unit> ResetCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PlayCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> StepCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UntackChartCommand { get; private set; }

        private void SetupCommands()
        {
            ResetCommand = ReactiveCommand.Create(() => this.Reset());
            PauseCommand = ReactiveCommand.Create(() => this.Pause());
            PlayCommand = ReactiveCommand.Create(() => this.Play());
            StepCommand = ReactiveCommand.Create(() => this.Step());
            StopCommand = ReactiveCommand.Create(() => this.Stop());
        }
    }
}
