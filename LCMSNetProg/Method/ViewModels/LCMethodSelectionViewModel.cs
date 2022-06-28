using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    /// <summary>
    /// User interface class that allows a user to select methods for comparison.
    /// </summary>
    public class LCMethodSelectionViewModel : ReactiveObject
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public LCMethodSelectionViewModel()
        {
            // The stupid designer forces us to check if this guy is null or not.
            if (LCMethodManager.Manager != null)
            {
                LCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
                LCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;
                LCMethodManager.Manager.MethodUpdated += Manager_MethodAdded;
            }

            listSelectedLcMethods.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var lcMethodsListBound).Subscribe();
            methodsComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var methodsComboOptions).Subscribe();
            ListSelectedLCMethods = lcMethodsListBound;
            MethodsComboBoxOptions = methodsComboOptions;

            AddCommand = ReactiveCommand.Create(AddMethodToList, this.WhenAnyValue(x => x.SelectedLCMethod, x => x.ListSelectedLCMethods.Count).Select(x => !this.ListSelectedLCMethods.Contains(SelectedLCMethod)));
            RemoveCommand = ReactiveCommand.Create(RemoveSelectedMethods, this.WhenAnyValue(x => x.SelectedListLCMethods.Count).Select(x => x > 0));
            MoveUpCommand = ReactiveCommand.Create(MoveSelectedItemsUp, this.WhenAnyValue(x => x.SelectedListLCMethods.Count).Select(x => x > 0));
            MoveDownCommand = ReactiveCommand.Create(MoveSelectedItemsDown, this.WhenAnyValue(x => x.SelectedListLCMethods.Count).Select(x => x > 0));
        }

        /// <summary>
        /// Gets the list of selected methods to render.
        /// </summary>
        public List<LCMethod> SelectedMethods
        {
            get { return ListSelectedLCMethods.Select(x => x.Clone() as LCMethod).ToList(); }
        }

        private LCMethod selectedLCMethod = null;
        private readonly SourceList<LCMethod> listSelectedLcMethods = new SourceList<LCMethod>();
        private readonly SourceList<LCMethod> methodsComboBoxOptions = new SourceList<LCMethod>();

        public LCMethod SelectedLCMethod
        {
            get => selectedLCMethod;
            set => this.RaiseAndSetIfChanged(ref selectedLCMethod, value);
        }

        /// <summary>
        /// LC Methods currently selected for display
        /// </summary>
        public ReadOnlyObservableCollection<LCMethod> ListSelectedLCMethods { get; }
        public ReadOnlyObservableCollection<LCMethod> MethodsComboBoxOptions { get; }

        /// <summary>
        /// LC Methods currently selected in the listbox for manipulation
        /// </summary>
        public ObservableCollectionExtended<LCMethod> SelectedListLCMethods { get; } = new ObservableCollectionExtended<LCMethod>();

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveDownCommand { get; }

        /// <summary>
        /// Definition for when a LC method is added or removed from the preview.
        /// </summary>
        /// <param name="sender"></param>
        public delegate void DelegateLCMethodSelected(object sender);

        /// <summary>
        /// Fired when a method is added to the list of selected methods.
        /// </summary>
        public event DelegateLCMethodSelected MethodAdded;

        /// <summary>
        /// Fired when a method is removed from the list of selected methods.
        /// </summary>
        public event DelegateLCMethodSelected MethodDeleted;

        /// <summary>
        /// Fired when the order of the methods have changed.
        /// </summary>
        public event DelegateLCMethodSelected MethodUpdated;

        /// <summary>
        /// Removes the method from the user interface when it's removed from the manager.
        /// </summary>
        /// <param name="sender">Object who sent the method.</param>
        /// <param name="method">Method to remove.</param>
        /// <returns>True always, that the method was removed.</returns>
        private void Manager_MethodRemoved(object sender, LCMethod method)
        {
            // Finds and removes the method in the listbox
            foreach (var removeMethod in ListSelectedLCMethods.Where(lcMethod => lcMethod.Name.Equals(method.Name)).ToList())
            {
                listSelectedLcMethods.Remove(removeMethod);
            }

            // Finds and removes the method in the listbox
            foreach (var removeMethod in MethodsComboBoxOptions.Where(lcMethod => lcMethod.Name.Equals(method.Name)).ToList())
            {
                methodsComboBoxOptions.Remove(removeMethod);
            }

            // For the users benefit, lets make sure the last item added is displayed in the list
            if (methodsComboBoxOptions.Count > 0)
                SelectedLCMethod = MethodsComboBoxOptions.First();
        }

        /// <summary>
        /// Adds the method to the user interface when it's added to the manager.
        /// </summary>
        /// <param name="sender">Object who sent the method.</param>
        /// <param name="method">Method to add.</param>
        /// <returns>True if a method was added, false if the method was null.</returns>
        private void Manager_MethodAdded(object sender, LCMethod method)
        {
            if (method == null)
                return;

            // Update the combo box so that the method has the right number of events,
            // OR add the method if it does not exist.

            // If the method was not found in the combobox then add it to the combobox.
            if (!MethodsComboBoxOptions.Any(lcMethod => lcMethod.Equals(method)))
            {
                methodsComboBoxOptions.Add(method);
            }

            // Update the list box with the right methods now, and make sure that we alert listeners.
            // If the method exists, then we need to make sure here that we update the list box
            if (ListSelectedLCMethods.Any(lcMethod => lcMethod.Name.Equals(method.Name)))
            {
                var match = ListSelectedLCMethods.First(x => x.Name.Equals(method.Name));
                var replace = (LCMethod)method.Clone();
                replace.CurrentEventNumber = match.CurrentEventNumber;
                listSelectedLcMethods.Replace(match, replace);

                MethodUpdated?.Invoke(this);
            }

            // If we added the first item, then we select that item
            // Otherwise, don't select an item since the user may have already selected an item.
            if (MethodsComboBoxOptions.Count > 0)
                SelectedLCMethod = MethodsComboBoxOptions.First();
        }

        /// <summary>
        /// Adds the selected combo box item's method to the list of preview methods.
        /// </summary>
        private void AddMethodToList()
        {
            var method = SelectedLCMethod;
            if (method != null)
            {
                var addMethod = method.Clone() as LCMethod;
                // need something likely to be unique.
                addMethod.CurrentEventNumber = (int)(DateTime.Now.Ticks + listSelectedLcMethods.Count);
                listSelectedLcMethods.Add(addMethod);
                MethodAdded?.Invoke(this);
            }
        }

        /// <summary>
        /// Removes the selected methods from the list box.
        /// </summary>
        private void RemoveSelectedMethods()
        {
            // Make sure we have some selected objects.
            if (SelectedListLCMethods.Count > 0)
            {
                // Remove the objects
                foreach (var method in SelectedListLCMethods.ToList())
                {
                    listSelectedLcMethods.Remove(method);
                }

                // Signal anyone who needs to know the objects were removed.
                MethodDeleted?.Invoke(this);
            }
        }

        /// <summary>
        /// Moves the selected items up.
        /// </summary>
        private void MoveSelectedItemsUp()
        {
            // Don't let this happen if nothing is selected
            if (SelectedListLCMethods.Count < 1) return;

            // Otherwise, we'll sort this out using an array to locally copy the
            // items, then clear and re-add them in the suited array as they are ordered.
            var names = ListSelectedLCMethods.ToList();

            // The top will be the lowest index the guy can move to.
            var top = 0;
            for (var i = 0; i < names.Count; i++)
            {
                var current = names[i];
                if (SelectedListLCMethods.Any(x => x.Name.Equals(names[i].Name) && x.CurrentEventNumber == names[i].CurrentEventNumber))
                {
                    // Calculate the new index
                    var newPos = Math.Max(i - 1, top);

                    // swap the data
                    if (newPos != i)
                    {
                        names[i] = names[newPos];
                        names[newPos] = current;
                    }

                    // set the top value to the new minimum index
                    top = newPos + 1;
                }
            }

            // TODO: use .Move(...) instead
            listSelectedLcMethods.Edit(sourceList =>
            {
                sourceList.Clear();
                sourceList.AddRange(names);
            });

            // Alerts listening objects that the order of the methods has changed.
            MethodUpdated?.Invoke(this);
        }

        /// <summary>
        /// Moves the selected items down.
        /// </summary>
        private void MoveSelectedItemsDown()
        {
            // Don't let this happen if nothing is selected
            if (SelectedListLCMethods.Count < 1) return;

            // Otherwise, we'll sort this out using an array to locally copy the
            // items, then clear and re-add them in the suited array as they are ordered.
            var names = ListSelectedLCMethods.ToList();

            // The top will be the highest index the guy can move to.
            var top = listSelectedLcMethods.Count - 1;
            for (var i = names.Count - 1; i >= 0; i--)
            {
                var current = names[i];
                if (SelectedListLCMethods.Any(x => x.Name.Equals(names[i].Name) && x.CurrentEventNumber == names[i].CurrentEventNumber))
                {
                    // Calculate the new index
                    var newPos = Math.Min(i + 1, top);

                    // swap the data
                    if (newPos != i)
                    {
                        names[i] = names[newPos];
                        names[newPos] = current;
                    }

                    // set the top value to the new maximum index
                    top = newPos - 1;
                }
            }

            // TODO: use .Move(...) instead
            listSelectedLcMethods.Edit(sourceList =>
            {
                sourceList.Clear();
                sourceList.AddRange(names);
            });

            // Alerts listening objects that the order of the methods has changed.
            MethodUpdated?.Invoke(this);
        }
    }
}
