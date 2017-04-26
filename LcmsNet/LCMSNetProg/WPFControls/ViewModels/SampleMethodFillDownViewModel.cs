using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LcmsNet.SampleQueue;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Method;
using ReactiveUI;

namespace LcmsNet.WPFControls.ViewModels
{
    public class SampleMethodFillDownViewModel : ReactiveObject
    {
        #region Property Backing Fields

        private bool applyGroup1 = true;
        private bool applyGroup2;
        private bool applyGroup3;
        private bool applyGroup4;
        private classLCMethod lcMethodGroup1;
        private classLCMethod lcMethodGroup2;
        private classLCMethod lcMethodGroup3;
        private classLCMethod lcMethodGroup4;
        private string instrumentMethodGroup1;
        private string instrumentMethodGroup2;
        private string instrumentMethodGroup3;
        private string instrumentMethodGroup4;
        private double volumeGroup1 = 7.0;
        private double volumeGroup2 = 7.0;
        private double volumeGroup3 = 7.0;
        private double volumeGroup4 = 7.0;
        private string datasetTypeGroup1;
        private string datasetTypeGroup2;
        private string datasetTypeGroup3;
        private string datasetTypeGroup4;
        private string cartConfigGroup1;
        private string cartConfigGroup2;
        private string cartConfigGroup3;
        private string cartConfigGroup4;

        #endregion

        #region UI interface binding properties

        public bool ApplyGroup1
        {
            get { return applyGroup1; }
            set { this.RaiseAndSetIfChanged(ref applyGroup1, value); }
        }

        public bool ApplyGroup2
        {
            get { return applyGroup2; }
            set { this.RaiseAndSetIfChanged(ref applyGroup2, value); }
        }

        public bool ApplyGroup3
        {
            get { return applyGroup3; }
            set { this.RaiseAndSetIfChanged(ref applyGroup3, value); }
        }

        public bool ApplyGroup4
        {
            get { return applyGroup4; }
            set { this.RaiseAndSetIfChanged(ref applyGroup4, value); }
        }

        public classLCMethod LCMethodGroup1
        {
            get { return lcMethodGroup1; }
            set { this.RaiseAndSetIfChanged(ref lcMethodGroup1, value); }
        }

        public classLCMethod LCMethodGroup2
        {
            get { return lcMethodGroup2; }
            set { this.RaiseAndSetIfChanged(ref lcMethodGroup2, value); }
        }

        public classLCMethod LCMethodGroup3
        {
            get { return lcMethodGroup3; }
            set { this.RaiseAndSetIfChanged(ref lcMethodGroup3, value); }
        }

        public classLCMethod LCMethodGroup4
        {
            get { return lcMethodGroup4; }
            set { this.RaiseAndSetIfChanged(ref lcMethodGroup4, value); }
        }

        public string InstrumentMethodGroup1
        {
            get { return instrumentMethodGroup1; }
            set { this.RaiseAndSetIfChanged(ref instrumentMethodGroup1, value); }
        }

        public string InstrumentMethodGroup2
        {
            get { return instrumentMethodGroup2; }
            set { this.RaiseAndSetIfChanged(ref instrumentMethodGroup2, value); }
        }

        public string InstrumentMethodGroup3
        {
            get { return instrumentMethodGroup3; }
            set { this.RaiseAndSetIfChanged(ref instrumentMethodGroup3, value); }
        }

        public string InstrumentMethodGroup4
        {
            get { return instrumentMethodGroup4; }
            set { this.RaiseAndSetIfChanged(ref instrumentMethodGroup4, value); }
        }

        public double VolumeGroup1
        {
            get { return volumeGroup1; }
            set { this.RaiseAndSetIfChanged(ref volumeGroup1, value); }
        }

        public double VolumeGroup2
        {
            get { return volumeGroup2; }
            set { this.RaiseAndSetIfChanged(ref volumeGroup2, value); }
        }

        public double VolumeGroup3
        {
            get { return volumeGroup3; }
            set { this.RaiseAndSetIfChanged(ref volumeGroup3, value); }
        }

        public double VolumeGroup4
        {
            get { return volumeGroup4; }
            set { this.RaiseAndSetIfChanged(ref volumeGroup4, value); }
        }

        public string DatasetTypeGroup1
        {
            get { return datasetTypeGroup1; }
            set { this.RaiseAndSetIfChanged(ref datasetTypeGroup1, value); }
        }

        public string DatasetTypeGroup2
        {
            get { return datasetTypeGroup2; }
            set { this.RaiseAndSetIfChanged(ref datasetTypeGroup2, value); }
        }

        public string DatasetTypeGroup3
        {
            get { return datasetTypeGroup3; }
            set { this.RaiseAndSetIfChanged(ref datasetTypeGroup3, value); }
        }

        public string DatasetTypeGroup4
        {
            get { return datasetTypeGroup4; }
            set { this.RaiseAndSetIfChanged(ref datasetTypeGroup4, value); }
        }

        public string CartConfigGroup1
        {
            get { return cartConfigGroup1; }
            set { this.RaiseAndSetIfChanged(ref cartConfigGroup1, value); }
        }

        public string CartConfigGroup2
        {
            get { return cartConfigGroup2; }
            set { this.RaiseAndSetIfChanged(ref cartConfigGroup2, value); }
        }

        public string CartConfigGroup3
        {
            get { return cartConfigGroup3; }
            set { this.RaiseAndSetIfChanged(ref cartConfigGroup3, value); }
        }

        public string CartConfigGroup4
        {
            get { return cartConfigGroup4; }
            set { this.RaiseAndSetIfChanged(ref cartConfigGroup4, value); }
        }

        // Local "wrappers" around the static class options, for data binding purposes
        public ReactiveList<classLCMethod> LcMethodComboBoxOptions => SampleQueueComboBoxOptions.LcMethodOptions;
        public ReactiveList<string> InstrumentMethodComboBoxOptions => SampleQueueComboBoxOptions.InstrumentMethodOptions;
        public ReactiveList<string> DatasetTypeComboBoxOptions => SampleQueueComboBoxOptions.DatasetTypeOptions;
        public ReactiveList<string> CartConfigComboBoxOptions => SampleQueueComboBoxOptions.CartConfigOptions;
        public string CartConfigError => SampleQueueComboBoxOptions.CartConfigOptionsError;

        public double VolumeMinimum
        {
            get { return classSampleData.CONST_MIN_SAMPLE_VOLUME; }
        }

        #endregion

        #region ReactiveCommands

        public ReactiveCommand ApplyLCMethodCommand { get; private set; }
        public ReactiveCommand ApplyInstrumentMethodCommand { get; private set; }
        public ReactiveCommand ApplyVolumeCommand { get; private set; }
        public ReactiveCommand ApplyDatasetTypeCommand { get; private set; }
        public ReactiveCommand ApplyCartConfigCommand { get; private set; }
        public ReactiveCommand ApplyAllCommand { get; private set; }
        public ReactiveCommand CloseWindowCommand { get; private set; }

        private void SetupCommands()
        {
            ApplyLCMethodCommand = ReactiveCommand.Create(() => this.LCMethodFillDown());
            ApplyInstrumentMethodCommand = ReactiveCommand.Create(() => this.InstrumentMethodFillDown());
            ApplyVolumeCommand = ReactiveCommand.Create(() => this.VolumeFillDown());
            ApplyDatasetTypeCommand = ReactiveCommand.Create(() => this.DatasetTypeFillDown());
            ApplyCartConfigCommand = ReactiveCommand.Create(() => this.CartConfigFillDown());
            ApplyAllCommand = ReactiveCommand.Create(() => this.ApplyAllFillDown());
            CloseWindowCommand = ReactiveCommand.Create(() => this.CloseWindow());
        }

        #endregion

        #region Properties

        public List<classSampleData> Samples { get; set; }

        #endregion

        #region Constructors

        public SampleMethodFillDownViewModel()
        {
            // Avoid exceptions caused from not being able to access program settings
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            SetupCommands();

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

        private void EnsureDatasetTypeComboBoxValues()
        {
            if (DatasetTypeComboBoxOptions.Count == 0)
            {
                return;
            }
            var defaultDatasetType = DatasetTypeComboBoxOptions.FirstOrDefault(x => x.StartsWith("HMS-HCD-HMSn", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(defaultDatasetType))
            {
                defaultDatasetType = DatasetTypeComboBoxOptions[0];
            }
            if (string.IsNullOrWhiteSpace(DatasetTypeGroup1))
            {
                DatasetTypeGroup1 = defaultDatasetType;
            }
            if (string.IsNullOrWhiteSpace(DatasetTypeGroup2))
            {
                DatasetTypeGroup2 = defaultDatasetType;
            }
            if (string.IsNullOrWhiteSpace(DatasetTypeGroup3))
            {
                DatasetTypeGroup3 = defaultDatasetType;
            }
            if (string.IsNullOrWhiteSpace(DatasetTypeGroup4))
            {
                DatasetTypeGroup4 = defaultDatasetType;
            }
        }

        private void EnsureCartConfigComboBoxValues()
        {
            if (CartConfigComboBoxOptions.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(CartConfigGroup1))
                {
                    CartConfigGroup1 = CartConfigComboBoxOptions[0];
                }
                if (string.IsNullOrWhiteSpace(CartConfigGroup2))
                {
                    CartConfigGroup2 = CartConfigComboBoxOptions[0];
                }
                if (string.IsNullOrWhiteSpace(CartConfigGroup3))
                {
                    CartConfigGroup3 = CartConfigComboBoxOptions[0];
                }
                if (string.IsNullOrWhiteSpace(CartConfigGroup4))
                {
                    CartConfigGroup4 = CartConfigComboBoxOptions[0];
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
            EnsureDatasetTypeComboBoxValues();
            EnsureCartConfigComboBoxValues();
        }

        /// <summary>
        /// Fill down the LC Method
        /// </summary>
        private void LCMethodFillDown()
        {
            EnsureItemsAreSelected();
            var methods = new List<classLCMethod>();
            if (ApplyGroup1)
            {
                methods.Add(LCMethodGroup1);
            }
            if (ApplyGroup2)
            {
                methods.Add(LCMethodGroup2);
            }
            if (ApplyGroup3)
            {
                methods.Add(LCMethodGroup3);
            }
            if (ApplyGroup4)
            {
                methods.Add(LCMethodGroup4);
            }
            if (methods.Count < 1)
                return;

            var i = 0;
            foreach (var samples in Samples)
            {
                var tempMethod = methods[i];

                if (tempMethod.Column != samples.ColumnData.ID)
                {
                    if (tempMethod.Column >= 0)
                    {
                        samples.ColumnData = classCartConfiguration.Columns[tempMethod.Column];
                    }
                }
                samples.LCMethod = tempMethod;

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
                sample.InstrumentData = new classInstrumentInfo();
                sample.InstrumentData.MethodName = methods[i];
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
            if (ApplyGroup1)
            {
                volumes.Add(VolumeGroup1);
            }
            if (ApplyGroup2)
            {
                volumes.Add(VolumeGroup2);
            }
            if (ApplyGroup3)
            {
                volumes.Add(VolumeGroup3);
            }
            if (ApplyGroup4)
            {
                volumes.Add(VolumeGroup4);
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
        /// Fill down the dataset type
        /// </summary>
        private void DatasetTypeFillDown()
        {
            EnsureItemsAreSelected();
            var datasetTypes = new List<string>();
            if (ApplyGroup1)
            {
                datasetTypes.Add(DatasetTypeGroup1);
            }
            if (ApplyGroup2)
            {
                datasetTypes.Add(DatasetTypeGroup2);
            }
            if (ApplyGroup3)
            {
                datasetTypes.Add(DatasetTypeGroup3);
            }
            if (ApplyGroup4)
            {
                datasetTypes.Add(DatasetTypeGroup4);
            }
            if (datasetTypes.Count < 1)
                return;

            var i = 0;
            foreach (var sample in Samples)
            {
                sample.DmsData.DatasetType = datasetTypes[i];
                i++;
                // mod?
                if (i >= datasetTypes.Count)
                    i = 0;
            }
        }

        /// <summary>
        /// Fill down the dataset type
        /// </summary>
        private void CartConfigFillDown()
        {
            EnsureItemsAreSelected();
            var cartConfigs = new List<string>();
            if (ApplyGroup1)
            {
                cartConfigs.Add(CartConfigGroup1);
            }
            if (ApplyGroup2)
            {
                cartConfigs.Add(CartConfigGroup2);
            }
            if (ApplyGroup3)
            {
                cartConfigs.Add(CartConfigGroup3);
            }
            if (ApplyGroup4)
            {
                cartConfigs.Add(CartConfigGroup4);
            }
            if (cartConfigs.Count < 1)
                return;

            var i = 0;
            foreach (var sample in Samples)
            {
                sample.DmsData.CartConfigName = cartConfigs[i];
                i++;
                // mod?
                if (i >= cartConfigs.Count)
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
            DatasetTypeFillDown();
            CartConfigFillDown();
        }

        private void CloseWindow()
        {
        }

        #endregion
    }
}
