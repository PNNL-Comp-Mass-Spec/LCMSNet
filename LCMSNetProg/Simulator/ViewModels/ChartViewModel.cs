using System;
using System.Collections.Generic;
using System.Reactive;
using System.Windows;
using FluidicsSDK;
using FluidicsSDK.Simulator;
using LcmsNet.Devices.ViewModels;
using LcmsNet.Method;
using LcmsNet.Method.ViewModels;
using LcmsNet.Simulator.Views;
using LcmsNetCommonControls.ViewModels;
using LcmsNetSDK;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Simulator.ViewModels
{
    public class ChartViewModel : ReactiveObject
    {
        public ChartViewModel()
        {
            simInstance = FluidicsSimulator.GetInstance;
            simInstance.EventExecuting += SimInstance_EventExecuting;
            simInstance.EventSimulated += SimInstance_EventExecuting;
            simInstance.SimulationComplete += SimInstance_SimulationComplete;

            SelectedMethods = new LCMethodSelectionViewModel();
            SelectedMethods.MethodAdded += SelectedMethods_MethodAdded;
            SelectedMethods.MethodDeleted += SelectedMethods_MethodDeleted;
            SelectedMethods.MethodUpdated += SelectedMethods_MethodUpdated;

            Reporter = new ModelCheckReportsViewModel(FluidicsModerator.Moderator);
            GanttChartTimelineVm = new LCMethodTimelineViewModel();
            GanttChartTimelineVm.RenderMode = LCMethodRenderMode.Column;
            ConversationChartTimelineVm = new LCMethodTimelineViewModel();
            ConversationChartTimelineVm.RenderMode = LCMethodRenderMode.Conversation;

            ReporterPopoutVm = new PopoutViewModel(Reporter);
            GanttChartTimelinePopoutVm = new PopoutViewModel(GanttChartTimelineVm);
            ConversationChartTimelinePopoutVm = new PopoutViewModel(ConversationChartTimelineVm);

            ResetCommand = ReactiveCommand.Create(Reset);
            PauseCommand = ReactiveCommand.Create(Pause);
            PlayCommand = ReactiveCommand.Create(Play);
            StepCommand = ReactiveCommand.Create(Step);
            StopCommand = ReactiveCommand.Create(Stop);
        }

        private DateTime startTime;
        private readonly FluidicsSimulator simInstance;

        public LCMethodSelectionViewModel SelectedMethods { get; }
        public ModelCheckReportsViewModel Reporter { get; }
        public LCMethodTimelineViewModel GanttChartTimelineVm { get; }
        public LCMethodTimelineViewModel ConversationChartTimelineVm { get; }
        public PopoutViewModel ReporterPopoutVm { get; }
        public PopoutViewModel GanttChartTimelinePopoutVm { get; }
        public PopoutViewModel ConversationChartTimelinePopoutVm { get; }

        public ReactiveCommand<Unit, Unit> ResetCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> PlayCommand { get; }
        public ReactiveCommand<Unit, Unit> StepCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        /// <summary>
        /// Aligns then renders the methods selected from the user interface.
        /// </summary>
        private void RenderAlignMethods()
        {
            // Align methods - blindly!
            try
            {
                var optimizer = new LCMethodOptimizer();
                var methods = SelectedMethods.SelectedMethods;

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
            var methods = SelectedMethods.SelectedMethods;
            var index = -1;
            var count = 0;
            var optimizer = new LCMethodOptimizer();
            optimizer.AlignMethods(methods);
            var queue = FluidicsSimulator.BuildEventList(methods, methods[0].Start);
            foreach (var list in queue)
            {
                if (list.Exists(lcEvent => lcEvent.Name == e.Event.Name && lcEvent.Start == e.Event.Start))
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
                    ApplicationLogger.LogError(0, status.Description);
                    statusMessages += status.Description + Environment.NewLine;
                }
                MessageBox.Show(statusMessages);
            }
        }

        /// <summary>
        /// Renders the throughput timelines.
        /// </summary>
        /// <param name="methods">Methods to render for showing throughput.</param>
        private void RenderThroughput(List<LCMethod> methods)
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
            var optimizer = new LCMethodOptimizer();
            var methodsToRun = SelectedMethods.SelectedMethods;
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
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_EMULATIONENABLED, "True");
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
            if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_EMULATIONENABLED, false))
            {
                return ConfirmNoEmulation();
            }
            return true;
        }
    }
}
