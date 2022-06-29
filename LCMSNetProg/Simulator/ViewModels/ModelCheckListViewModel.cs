using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using FluidicsSDK;
using FluidicsSDK.ModelCheckers;
using ReactiveUI;

namespace LcmsNet.Simulator.ViewModels
{
    public class ModelCheckListViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ModelCheckListViewModel() : this(new IFluidicsModelChecker[]{ new FluidicsCycleCheck(), new MultipleSourcesModelCheck(), new NoSinksModelCheck(), new TestModelCheck() })
        {
        }

        public ModelCheckListViewModel(IModelCheckController cntrlr, IEnumerable<IFluidicsModelChecker> checks) : this(checks)
        {
            controller = cntrlr;
            controller.ModelCheckAdded += CheckAddedHandler;
            controller.ModelCheckRemoved += CheckRemovedHandler;
        }

        private ModelCheckListViewModel(IEnumerable<IFluidicsModelChecker> checks)
        {
            foreach (var check in checks)
            {
                AddCheckerControl(check);
            }

            StatusCategoryComboBoxOptions = Enum.GetValues(typeof(ModelStatusCategory)).Cast<ModelStatusCategory>().ToList().AsReadOnly();

            this.WhenAnyValue(x => x.EnableAll).Subscribe(x => EnableAllChanged());

            checkers.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var checkersBound).Subscribe();
            Checkers = checkersBound;

            checkers.Connect().ObserveOn(RxApp.MainThreadScheduler).WhenValueChanged(x => x.IsEnabled).Subscribe(ModelEnabledChanged);
        }

        ~ModelCheckListViewModel()
        {
            if (controller != null)
            {
                controller.ModelCheckAdded -= CheckAddedHandler;
                controller.ModelCheckRemoved -= CheckRemovedHandler;
            }
        }

        private readonly IModelCheckController controller;
        private readonly SourceList<IFluidicsModelChecker> checkers = new SourceList<IFluidicsModelChecker>();
        private bool allChecked = false;
        private bool enableAll = false;

        public bool EnableAll
        {
            get => enableAll;
            set => this.RaiseAndSetIfChanged(ref enableAll, value);
        }

        public ReadOnlyObservableCollection<IFluidicsModelChecker> Checkers {get; }
        public ReadOnlyCollection<ModelStatusCategory> StatusCategoryComboBoxOptions { get; }

        private void EnableAllChanged()
        {
            if (EnableAll && !allChecked)
            {
                foreach (var check in checkers.Items)
                {
                    check.IsEnabled = true;
                }
                allChecked = true;
            }
            if (!EnableAll && allChecked)
            {
                foreach (var check in checkers.Items)
                {
                    check.IsEnabled = false;
                }
                allChecked = false;
            }
        }

        private void ModelEnabledChanged(bool newValue)
        {
            if (newValue)
            {
                if (checkers.Items.All(x => x.IsEnabled))
                {
                    allChecked = true;
                    EnableAll = true;
                }
            }
            else
            {
                if (checkers.Items.Any(x => !x.IsEnabled))
                {
                    allChecked = false;
                    EnableAll = false;
                }
            }
        }

        private void AddCheckerControl(IFluidicsModelChecker check)
        {
            if (!checkers.Items.Any(x => x.Name.Equals(check.Name)))
            {
                checkers.Add(check);
            }
        }

        private void CheckAddedHandler(object sender, ModelCheckControllerEventArgs e)
        {
            AddCheckerControl(e.ModelChecker);
        }

        private void CheckRemovedHandler(object sender, ModelCheckControllerEventArgs e)
        {
            var check = e.ModelChecker;
            if (checkers.Items.Any(x => x.Name.Equals(check.Name)))
            {
                checkers.Edit(sourceList =>
                {
                    var toRemove = sourceList.Where(x => x.Name.Equals(check.Name)).ToList();
                    foreach (var item in toRemove)
                    {
                        sourceList.Remove(item);
                    }
                });
            }
        }
    }
}
