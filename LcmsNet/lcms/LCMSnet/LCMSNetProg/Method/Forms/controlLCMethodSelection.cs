using System;
using System.Windows.Forms;
using System.Collections.Generic;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// User interface class that allows a user to select methods for comparison.
    /// </summary>
    public partial class controlLCMethodSelection : UserControl
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public controlLCMethodSelection()
        {
            InitializeComponent();

            //
            // The stupid designer forces us to check if this guy is null or not.
            //
            if (classLCMethodManager.Manager != null)
            {
                classLCMethodManager.Manager.MethodAdded += new DelegateMethodUpdated(Manager_MethodAdded);
                classLCMethodManager.Manager.MethodRemoved += new DelegateMethodUpdated(Manager_MethodRemoved);
                classLCMethodManager.Manager.MethodUpdated += new DelegateMethodUpdated(Manager_MethodAdded);
            }

            mbutton_up.LostFocus += new EventHandler(mbutton_up_LostFocus);
            mbutton_down.LostFocus += new EventHandler(mbutton_down_LostFocus);
            mcomboBox_methods.KeyUp += new KeyEventHandler(mcomboBox_methods_KeyUp);
        }

        /// <summary>
        /// Gets the list of selected methods to render.
        /// </summary>
        public List<classLCMethod> SelectedMethods
        {
            get
            {
                var methods = new List<classLCMethod>();
                foreach (var o in mlistBox_methods.Items)
                {
                    var methodName = (string) o;
                    if (methodName != null)
                    {
                        //
                        // Make sure we have a valid method
                        //
                        if (classLCMethodManager.Manager.Methods.ContainsKey(methodName) == true)
                        {
                            var method = classLCMethodManager.Manager.Methods[methodName];
                            var cloned = method.Clone() as classLCMethod;

                            if (cloned != null)
                                methods.Add(cloned);
                        }
                    }
                }
                return methods;
            }
        }

        #region Events and Delegates

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

        #endregion

        #region Methods and Event Handlers

        /// <summary>
        /// Removes the method to the user interface when it's removed from the manager.
        /// </summary>
        /// <param name="sender">Object who sent the method.</param>
        /// <param name="method">Method to remove.</param>
        /// <returns>True always, that the method was removed.</returns>
        bool Manager_MethodRemoved(object sender, classLCMethod method)
        {
            //
            // Finds the method in the listbox
            //
            var objects = new object[mlistBox_methods.Items.Count];
            mlistBox_methods.Items.CopyTo(objects, 0);
            var removeMethods = FindMethodsInList(objects, method.Name);

            //
            // The method exists, so then we need to remove it's instances.
            //
            if (removeMethods != null)
            {
                foreach (var removeMethod in removeMethods)
                    mlistBox_methods.Items.Remove(removeMethod);
            }

            //
            // Finds and removes the method in thelist box,
            //
            objects = new object[mlistBox_methods.Items.Count];
            mlistBox_methods.Items.CopyTo(objects, 0);
            removeMethods = FindMethodsInList(objects, method.Name);
            if (removeMethods != null)
            {
                //
                // Remove here
                //
                foreach (var removeMethod in removeMethods)
                    mlistBox_methods.Items.Remove(removeMethod);

                MethodDeleted?.Invoke(this);
            }

            //
            // For the users benefit, lets make sure the last item added is displayed in the list
            //
            if (mcomboBox_methods.Items.Count > 0)
                mcomboBox_methods.SelectedIndex = 0;

            if (mlistBox_methods.Items.Count > 0)
                mlistBox_methods.SelectedIndex = 0;

            return true;
        }

        /// <summary>
        /// Adds the method to the user interface when it's added to the manager.
        /// </summary>
        /// <param name="sender">Object who sent the method.</param>
        /// <param name="method">Method to add.</param>
        /// <returns>True if a method was added, false if the method was null.</returns>
        bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            if (method == null)
                return false;

            //
            // Update the combo box so that the method
            // has the right number of events, OR add the method if it
            // does not exist.
            //
            var objects = new object[mcomboBox_methods.Items.Count];
            mcomboBox_methods.Items.CopyTo(objects, 0);
            var foundMethods = FindMethodsInList(objects, method.Name);

            //
            // If the method was not found in the combobox then add it to the combobox.
            //
            if (foundMethods == null || foundMethods.Count < 1)
            {
                mcomboBox_methods.Items.Add(method.Name);
            }


            //
            // Update the list box with the right methods now, and make sure that we
            // alert listeners.
            //
            objects = new object[mlistBox_methods.Items.Count];
            mlistBox_methods.Items.CopyTo(objects, 0);
            foundMethods = FindMethodsInList(objects, method.Name);

            //
            // If the method exists, then we need to make sure here that we update the list box
            //
            if (foundMethods != null && foundMethods.Count > 0)
            {
                foreach (var foundMethod in foundMethods)
                {
                    var indexOf = mlistBox_methods.Items.IndexOf(foundMethod);
                    mlistBox_methods.Items[indexOf] = method.Name;
                }

                MethodUpdated?.Invoke(this);
            }


            //
            // If we added the first guy, then we select the first item
            // otherwise, dont select someone since the user may have
            // already selected an item.
            //
            if (mcomboBox_methods.Items.Count == 1)
                mcomboBox_methods.SelectedIndex = 0;

            return true;
        }

        /// <summary>
        /// Adds the selected combo box list method to the list of preview methods
        /// on the enter key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mcomboBox_methods_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AddMethodToList();
        }

        /// <summary>
        /// Alerts listening objects that the order of the methods has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mbutton_down_LostFocus(object sender, EventArgs e)
        {
            MethodUpdated?.Invoke(this);
        }

        /// <summary>
        /// Alerts listening objects that the order of the methods has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mbutton_up_LostFocus(object sender, EventArgs e)
        {
            MethodUpdated?.Invoke(this);
        }

        /// <summary>
        /// Finds a method in the list provided.
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private List<string> FindMethodsInList(IEnumerable<object> methods, string method)
        {
            var foundMethods = new List<string>();

            //
            // BAH!  we have to sort through the names here of the methods
            // maybe we could hash them?
            //
            foreach (var o in methods)
            {
                var lcMethod = (string) o;

                if (lcMethod != null)
                {
                    if (lcMethod == method)
                    {
                        foundMethods.Add(lcMethod);
                    }
                }
            }
            return foundMethods;
        }

        /// <summary>
        /// Adds the selected method to the list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_add_Click(object sender, EventArgs e)
        {
            AddMethodToList();
        }

        /// <summary>
        /// Adds the selected combo box item's method to the list of preview methods.
        /// </summary>
        private void AddMethodToList()
        {
            if (mcomboBox_methods.SelectedItem != null)
            {
                var method = (string) mcomboBox_methods.SelectedItem;
                if (method != null)
                {
                    mlistBox_methods.Items.Add(method);
                    MethodAdded?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Removes the selected method from the list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_remove_Click(object sender, EventArgs e)
        {
            //
            // Make sure we have some selected objects.
            //
            if (mlistBox_methods.SelectedItems != null && mlistBox_methods.SelectedItems.Count > 0)
            {
                //
                // Remove the objects
                //
                var objects = new object[mlistBox_methods.SelectedItems.Count];
                mlistBox_methods.SelectedItems.CopyTo(objects, 0);
                foreach (var o in objects)
                    mlistBox_methods.Items.Remove(o);

                //
                // Signal anyone who needs to know the objects were removed.
                //
                MethodDeleted?.Invoke(this);
            }
        }

        /// <summary>
        /// Moves the selected items up.
        /// </summary>
        private void MoveSelectedItemsUp()
        {
            //
            // Dont let this happen if we dont have anything selected
            //
            if (mlistBox_methods.SelectedIndices.Count < 1) return;

            //
            // Hold on to what items were moved
            //
            var indices = new int[mlistBox_methods.SelectedIndices.Count];

            //
            // Otherwise, we'll sort this out using an array to locally copy the
            // items, then clear and readd them in the suited array as they
            // are ordered.
            //
            var objects = new object[mlistBox_methods.Items.Count];
            mlistBox_methods.Items.CopyTo(objects, 0);

            //
            // The top will be the lowest index the guy can move to.
            //
            var top = -1;
            for (var i = 0; i < mlistBox_methods.SelectedIndices.Count; i++)
            {
                //
                // Calculate the new index
                //
                var newIndex = mlistBox_methods.SelectedIndices[i];
                var prevIndex = newIndex;

                newIndex = Math.Max(newIndex - 1, top + 1);

                //
                // swap the data
                //
                var temp = objects[prevIndex];
                objects[prevIndex] = objects[newIndex];
                objects[newIndex] = temp;

                //
                // set the new top value to be the new index
                //
                top = newIndex;

                indices[i] = newIndex;
            }


            mlistBox_methods.BeginUpdate();
            mlistBox_methods.Items.Clear();
            mlistBox_methods.Items.AddRange(objects);

            //
            // Reselect any objects that were previously based on their
            // new indices
            //
            foreach (var i in indices)
                mlistBox_methods.SetSelected(i, true);

            mlistBox_methods.EndUpdate();
        }

        /// <summary>
        /// Moves the selected items down.
        /// </summary>
        private void MoveSelectedItemsDown()
        {
            //
            // Dont let this happen if we dont have anything selected
            //
            if (mlistBox_methods.SelectedIndices.Count < 1) return;

            //
            // Hold on to what items were moved
            //
            var indices = new int[mlistBox_methods.SelectedIndices.Count];

            //
            // Otherwise, we'll sort this out using an array to locally copy the
            // items, then clear and readd them in the suited array as they
            // are ordered.
            //
            var objects = new object[mlistBox_methods.Items.Count];
            mlistBox_methods.Items.CopyTo(objects, 0);

            //
            // The top will be the lowest index the guy can move to.
            //
            var top = mlistBox_methods.Items.Count;
            for (var i = mlistBox_methods.SelectedIndices.Count - 1; i > -1; i--)
            {
                //
                // Calculate the new index
                //
                var newIndex = mlistBox_methods.SelectedIndices[i];
                var prevIndex = newIndex;

                newIndex = Math.Min(newIndex + 1, top - 1);

                //
                // swap the data
                //
                var temp = objects[prevIndex];
                objects[prevIndex] = objects[newIndex];
                objects[newIndex] = temp;

                //
                // set the new top value to be the new index
                //
                top = newIndex;
                indices[i] = newIndex;
            }


            mlistBox_methods.BeginUpdate();
            mlistBox_methods.Items.Clear();
            mlistBox_methods.Items.AddRange(objects);

            //
            // Reselect any objects that were previously based on their
            // new indices
            //
            foreach (var i in indices)
                mlistBox_methods.SetSelected(i, true);

            mlistBox_methods.EndUpdate();
        }

        /// <summary>
        /// Moves the selected items up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_up_Click(object sender, EventArgs e)
        {
            MoveSelectedItemsUp();
        }

        /// <summary>
        /// Moves the selected items down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_down_Click(object sender, EventArgs e)
        {
            MoveSelectedItemsDown();
        }

        #endregion
    }
}