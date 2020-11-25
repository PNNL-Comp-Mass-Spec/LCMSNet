using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using FluidicsSDK.ModelCheckers;
using LcmsNetSDK;
using ReactiveUI;

namespace LcmsNet.Simulator.ViewModels
{
    public class ModelCheckListViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ModelCheckListViewModel() : this(new IFluidicsModelChecker[] {new FluidicsCycleCheck(), new MultipleSourcesModelCheck(), new NoSinksModelCheck(), new TestModelCheck()})
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

            StatusCategoryComboBoxOptions = new ReactiveList<ModelStatusCategory>(Enum.GetValues(typeof(ModelStatusCategory)).Cast<ModelStatusCategory>());

            Checkers.ChangeTrackingEnabled = true;

            this.WhenAnyValue(x => x.EnableAll).Subscribe(x => EnableAllChanged());
            Checkers.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.IsEnabled)))
                .Subscribe(x => ModelEnabledChanged(x.Sender.IsEnabled));
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
        private readonly ReactiveList<IFluidicsModelChecker> checkers = new ReactiveList<IFluidicsModelChecker>();
        private bool allChecked = false;
        private bool enableAll = false;

        public bool EnableAll
        {
            get => enableAll;
            set => this.RaiseAndSetIfChanged(ref enableAll, value);
        }

        public IReadOnlyReactiveList<IFluidicsModelChecker> Checkers => checkers;
        public IReadOnlyReactiveList<ModelStatusCategory> StatusCategoryComboBoxOptions { get; }

        private void EnableAllChanged()
        {
            if (EnableAll && !allChecked)
            {
                foreach (var check in checkers)
                {
                    check.IsEnabled = true;
                }
                allChecked = true;
            }
            if (!EnableAll && allChecked)
            {
                foreach (var check in checkers)
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
                if (checkers.All(x => x.IsEnabled))
                {
                    allChecked = true;
                    EnableAll = true;
                }
            }
            else
            {
                if (checkers.Any(x => !x.IsEnabled))
                {
                    allChecked = false;
                    EnableAll = false;
                }
            }
        }

        private void AddCheckerControl(IFluidicsModelChecker check)
        {
            if (!checkers.Any(x => x.Name.Equals(check.Name)))
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
            if (checkers.Any(x => x.Name.Equals(check.Name)))
            {
                using (checkers.SuppressChangeNotifications())
                {
                    checkers.RemoveAll(Checkers.ToList().Where(x => x.Name.Equals(check.Name)));
                }
            }
        }
    }
}
