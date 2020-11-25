using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    /// <summary>
    /// Form for choosing sample queue randomization technique and randomizing
    /// </summary>
    public class SampleRandomizerViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleRandomizerViewModel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SampleRandomizerViewModel(List<SampleData> inputList)
        {
            InputSampleList = new ReactiveList<SampleData>(inputList);

            RandomizationPerformed = false;
            RandomizeCommand = ReactiveCommand.Create(Randomize, this.WhenAnyValue(x => x.SelectedRandomizer).Select(x => !string.IsNullOrWhiteSpace(SelectedRandomizer)));

            // Load list of randomizer types
            RandomizerNameList = new ReactiveList<string>(RandomizerPluginTools.GetRandomizerPlugins().Keys);
            SelectedRandomizer = "";
        }

        #region "Class variables"

        private readonly ReactiveList<SampleData> outputSampleList = new ReactiveList<SampleData>();
        private Dictionary<string, Type> randomizersDict = new Dictionary<string, Type>();
        private string currentStatus = "Ready.";
        private string selectedRandomizer = "";
        private bool randomizationPerformed = false;

        #endregion

        #region "Properties"

        public IReadOnlyReactiveList<SampleData> InputSampleList { get; }
        public IReadOnlyReactiveList<SampleData> OutputSampleList => outputSampleList;
        public IReadOnlyReactiveList<string> RandomizerNameList { get; }

        public string CurrentStatus
        {
            get => currentStatus;
            set => this.RaiseAndSetIfChanged(ref currentStatus, value);
        }

        public string SelectedRandomizer
        {
            get => selectedRandomizer;
            set => this.RaiseAndSetIfChanged(ref selectedRandomizer, value);
        }

        public bool RandomizationPerformed
        {
            get => randomizationPerformed;
            set => this.RaiseAndSetIfChanged(ref randomizationPerformed, value);
        }

        public ReactiveCommand<Unit, Unit> RandomizeCommand { get; }

        #endregion

        #region "Event Handlers"

        /// <summary>
        /// Handles Randomize button event to randomize a list of samples
        /// </summary>
        private void Randomize()
        {
            // Verify a randomization method has been chosen
            if (string.IsNullOrWhiteSpace(SelectedRandomizer))
            {
                CurrentStatus = "Randomization type must be selected.";
                return;
            }

            CurrentStatus = "Randomizing input list.";
            RandomizeSamples();
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Randomizes the sequence numbers for a list of samples;
        /// stores randomized samples for retrieval via OutputSampleList
        /// </summary>
        private void RandomizeSamples()
        {
            var randomizerObject = Activator.CreateInstance(randomizersDict[SelectedRandomizer]);
            var randomizer = randomizerObject as IRandomizerInterface;
            var randomized = randomizer.RandomizeSamples(InputSampleList.ToList());

            using (outputSampleList.SuppressChangeNotifications())
            {
                outputSampleList.AddRange(randomized);
            }

            CurrentStatus = "Randomization Complete.";

            RandomizationPerformed = true;
        }

        #endregion
    }
}
