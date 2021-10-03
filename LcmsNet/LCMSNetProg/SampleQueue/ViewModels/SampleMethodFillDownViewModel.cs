using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleMethodFillDownViewModel : ReactiveObject
    {
        #region Property Backing Fields

        private bool applyGroup1 = true;
        private bool applyGroup2;
        private bool applyGroup3;
        private bool applyGroup4;
        private string lcMethodGroup1;
        private string lcMethodGroup2;
        private string lcMethodGroup3;
        private string lcMethodGroup4;
        private string instrumentMethodGroup1;
        private string instrumentMethodGroup2;
        private string instrumentMethodGroup3;
        private string instrumentMethodGroup4;
        private double volumeGroup1 = 7.0;
        private double volumeGroup2 = 7.0;
        private double volumeGroup3 = 7.0;
        private double volumeGroup4 = 7.0;

        #endregion

        #region UI interface binding properties

        public bool ApplyGroup1
        {
            get => applyGroup1;
            set => this.RaiseAndSetIfChanged(ref applyGroup1, value);
        }

        public bool ApplyGroup2
        {
            get => applyGroup2;
            set => this.RaiseAndSetIfChanged(ref applyGroup2, value);
        }

        public bool ApplyGroup3
        {
            get => applyGroup3;
            set => this.RaiseAndSetIfChanged(ref applyGroup3, value);
        }

        public bool ApplyGroup4
        {
            get => applyGroup4;
            set => this.RaiseAndSetIfChanged(ref applyGroup4, value);
        }

        public string LCMethodGroup1
        {
            get => lcMethodGroup1;
            set => this.RaiseAndSetIfChanged(ref lcMethodGroup1, value);
        }

        public string LCMethodGroup2
        {
            get => lcMethodGroup2;
            set => this.RaiseAndSetIfChanged(ref lcMethodGroup2, value);
        }

        public string LCMethodGroup3
        {
            get => lcMethodGroup3;
            set => this.RaiseAndSetIfChanged(ref lcMethodGroup3, value);
        }

        public string LCMethodGroup4
        {
            get => lcMethodGroup4;
            set => this.RaiseAndSetIfChanged(ref lcMethodGroup4, value);
        }

        public string InstrumentMethodGroup1
        {
            get => instrumentMethodGroup1;
            set => this.RaiseAndSetIfChanged(ref instrumentMethodGroup1, value);
        }

        public string InstrumentMethodGroup2
        {
            get => instrumentMethodGroup2;
            set => this.RaiseAndSetIfChanged(ref instrumentMethodGroup2, value);
        }

        public string InstrumentMethodGroup3
        {
            get => instrumentMethodGroup3;
            set => this.RaiseAndSetIfChanged(ref instrumentMethodGroup3, value);
        }

        public string InstrumentMethodGroup4
        {
            get => instrumentMethodGroup4;
            set => this.RaiseAndSetIfChanged(ref instrumentMethodGroup4, value);
        }

        public double VolumeGroup1
        {
            get => volumeGroup1;
            set => this.RaiseAndSetIfChanged(ref volumeGroup1, value);
        }

        public double VolumeGroup2
        {
            get => volumeGroup2;
            set => this.RaiseAndSetIfChanged(ref volumeGroup2, value);
        }

        public double VolumeGroup3
        {
            get => volumeGroup3;
            set => this.RaiseAndSetIfChanged(ref volumeGroup3, value);
        }

        public double VolumeGroup4
        {
            get => volumeGroup4;
            set => this.RaiseAndSetIfChanged(ref volumeGroup4, value);
        }

        // Local "wrappers" around the static class options, for data binding purposes
        public ReadOnlyObservableCollection<string> LcMethodComboBoxOptions => SampleDataManager.LcMethodNameOptions;
        public ReadOnlyObservableCollection<string> InstrumentMethodComboBoxOptions => SampleDataManager.InstrumentMethodOptions;

        public double VolumeMinimum => CartConfiguration.MinimumVolume;

        #endregion

        #region ReactiveCommands

        public ReactiveCommand<Unit, Unit> ApplyLCMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> ApplyInstrumentMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> ApplyVolumeCommand { get; }
        public ReactiveCommand<Unit, Unit> ApplyAllCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseWindowCommand { get; }

        #endregion

        #region Properties

        public List<SampleData> Samples { get; set; }

        #endregion

        #region Constructors

        public SampleMethodFillDownViewModel()
        {
            // Avoid exceptions caused from not being able to access program settings
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            ApplyLCMethodCommand = ReactiveCommand.Create(LCMethodFillDown);
            ApplyInstrumentMethodCommand = ReactiveCommand.Create(InstrumentMethodFillDown);
            ApplyVolumeCommand = ReactiveCommand.Create(VolumeFillDown);
            ApplyAllCommand = ReactiveCommand.Create(ApplyAllFillDown);
            CloseWindowCommand = ReactiveCommand.Create(CloseWindow);

            // Set initial dropdown values
            EnsureItemsAreSelected();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set a selected value for each combo box, if one hasn't already been set.
        /// </summary>
        private void EnsureLCMethodComboBoxValues()
        {
            if (LcMethodComboBoxOptions.Count > 0 && LCMethodGroup1 == null)
            {
                LCMethodGroup1 = LcMethodComboBoxOptions[0];
            }
            if (LcMethodComboBoxOptions.Count > 1 && LCMethodGroup2 == null)
            {
                LCMethodGroup2 = LcMethodComboBoxOptions[1];
            }
            if (LcMethodComboBoxOptions.Count > 2 && LCMethodGroup3 == null)
            {
                LCMethodGroup3 = LcMethodComboBoxOptions[2];
            }
            if (LcMethodComboBoxOptions.Count > 3 && LCMethodGroup4 == null)
            {
                LCMethodGroup4 = LcMethodComboBoxOptions[3];
            }
        }

        private void EnsureInstrumentMethodComboBoxValues()
        {
            if (InstrumentMethodComboBoxOptions.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(InstrumentMethodGroup1))
                {
                    InstrumentMethodGroup1 = InstrumentMethodComboBoxOptions[0];
                }
                if (string.IsNullOrWhiteSpace(InstrumentMethodGroup2))
                {
                    InstrumentMethodGroup2 = InstrumentMethodComboBoxOptions[0];
                }
                if (string.IsNullOrWhiteSpace(InstrumentMethodGroup3))
                {
                    InstrumentMethodGroup3 = InstrumentMethodComboBoxOptions[0];
                }
                if (string.IsNullOrWhiteSpace(InstrumentMethodGroup4))
                {
                    InstrumentMethodGroup4 = InstrumentMethodComboBoxOptions[0];
                }
            }
        }

        /// <summary>
        /// Make sure a selected value will be shown for each combobox. Can be used for defaults and error checking (doesn't replace set values)
        /// </summary>
        public void EnsureItemsAreSelected()
        {
            EnsureLCMethodComboBoxValues();
            //EnsureInstrumentMethodComboBoxValues(); // commented out in the view
        }

        /// <summary>
        /// Fill down the LC Method
        /// </summary>
        private void LCMethodFillDown()
        {
            EnsureItemsAreSelected();
            var methods = new List<LCMethod>();
            if (ApplyGroup1)
            {
                var method = LCMethodManager.Manager.GetLCMethodByName(LCMethodGroup1);
                methods.Add(method);
            }
            if (ApplyGroup2)
            {
                var method = LCMethodManager.Manager.GetLCMethodByName(LCMethodGroup2);
                methods.Add(method);
            }
            if (ApplyGroup3)
            {
                var method = LCMethodManager.Manager.GetLCMethodByName(LCMethodGroup3);
                methods.Add(method);
            }
            if (ApplyGroup4)
            {
                var method = LCMethodManager.Manager.GetLCMethodByName(LCMethodGroup4);
                methods.Add(method);
            }
            if (methods.Count < 1)
                return;

            var i = 0;
            foreach (var sample in Samples)
            {
                var tempMethod = methods[i];

                if (tempMethod.Column != sample.ColumnIndex)
                {
                    if (tempMethod.Column >= 0)
                    {
                        sample.ColumnIndex = CartConfiguration.Columns[tempMethod.Column].ID;
                    }
                }
                sample.LCMethodName = tempMethod.Name;

                i++;
                // mod?
                if (i >= methods.Count)
                    i = 0;
            }
        }

        /// <summary>
        /// Fill down the instrument method
        /// </summary>
        private void InstrumentMethodFillDown()
        {
            if (InstrumentMethodComboBoxOptions.Count < 1)
                return;

            EnsureItemsAreSelected();

            var methods = new List<string>();
            if (ApplyGroup1)
            {
                methods.Add(InstrumentMethodGroup1);
            }
            if (ApplyGroup2)
            {
                methods.Add(InstrumentMethodGroup2);
            }
            if (ApplyGroup3)
            {
                methods.Add(InstrumentMethodGroup3);
            }
            if (ApplyGroup4)
            {
                methods.Add(InstrumentMethodGroup4);
            }
            if (methods.Count < 1)
                return;

            var i = 0;
            foreach (var sample in Samples)
            {
                sample.InstrumentMethod = methods[i];
                i++;
                // mod?
                if (i >= methods.Count)
                    i = 0;
            }
        }

        /// <summary>
        /// Fill down the volume
        /// </summary>
        private void VolumeFillDown()
        {
            EnsureItemsAreSelected();
            var volumes = new List<double>();
            // Do a round to 2 decimal places
            if (ApplyGroup1)
            {
                volumes.Add(Math.Round(VolumeGroup1, 2));
            }
            if (ApplyGroup2)
            {
                volumes.Add(Math.Round(VolumeGroup2, 2));
            }
            if (ApplyGroup3)
            {
                volumes.Add(Math.Round(VolumeGroup3, 2));
            }
            if (ApplyGroup4)
            {
                volumes.Add(Math.Round(VolumeGroup4, 2));
            }
            if (volumes.Count < 1)
                return;

            var i = 0;
            foreach (var sample in Samples)
            {
                sample.Volume = volumes[i];
                i++;
                // mod?
                if (i >= volumes.Count)
                    i = 0;
            }
        }

        /// <summary>
        /// Apply all filldown settings
        /// </summary>
        private void ApplyAllFillDown()
        {
            // This is lazy programming....
            //InstrumentMethodFillDown(); // commented out in the view.
            EnsureItemsAreSelected();
            LCMethodFillDown();
            VolumeFillDown();
        }

        private void CloseWindow()
        {
        }

        #endregion
    }
}
