using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LcmsNet.Method;
using LcmsNetDataClasses;
using LcmsNetSDK;

namespace LcmsNet.SampleQueue
{
    public class SampleQueueUndoRedo : INotifyPropertyChangedExt
    {
        private bool canUndo = false;
        private bool canRedo = false;

        /// <summary>
        /// If there are queues in the undo queue
        /// </summary>
        /// <returns></returns>
        public bool CanUndo
        {
            get { return canUndo; }
            private set { this.RaiseAndSetIfChanged(ref canUndo, value); }
        }

        /// <summary>
        /// If there are queues in the redo queue
        /// </summary>
        /// <returns></returns>
        public bool CanRedo
        {
            get { return canRedo; }
            private set { this.RaiseAndSetIfChanged(ref canRedo, value); }
        }

        public bool IsDirty { get; private set; }

        /// <summary>
        /// List/history of queue changes to support undo and redo operations; undo is performed by decrementing the current index back by 1 and returning the queue at the new index, redo is performed by incrementing the current index by one and returning the queue at the new index
        /// </summary>
        private readonly List<List<classSampleData>> undoRedoList;

        /// <summary>
        /// The index of the currently displayed queue in the list/history of queue changes
        /// </summary>
        private int currentIndex = 0;

        public SampleQueueUndoRedo()
        {
            // Undo - redo operations
            undoRedoList = new List<List<classSampleData>>();
        }

        /// <summary>
        /// Pushes the queue onto the queue history
        /// </summary>
        /// <param name="workingQueue">Queue to push onto the history</param>
        /// <remarks>
        /// The undo/redo operations are handled as follows:
        /// Undo is performed by decrementing the current index back by 1 and returning the queue at the new index
        /// Redo is performed by incrementing the current index by one and returning the queue at the new index
        /// </remarks>
        private void PushQueue(List<classSampleData> workingQueue)
        {
            var pushQueue = new List<classSampleData>();
            foreach (var data in workingQueue)
            {
                var sample = data.Clone() as classSampleData;
                if (sample?.LCMethod?.Name != null)
                {
                    if (classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                    {
                        // Because sample clones are deep copies, we cannot trust that
                        // every object in the sample is serializable...so...we are stuck
                        // making sure we re-hash the method using the name which
                        // is copied during the serialization.
                        sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name];
                    }
                }
                pushQueue.Add(sample);
            }

            // if an undo operation has been made, we need to remove the undone changes from the history
            if (currentIndex < undoRedoList.Count - 1)
            {
                undoRedoList.RemoveRange(currentIndex + 1, undoRedoList.Count - currentIndex - 1);
            }
            undoRedoList.Add(pushQueue);
            currentIndex = undoRedoList.Count - 1;
            IsDirty = true;

            SetCanUndoRedo();
        }

        /// <summary>
        /// Pops the queue from the stack if available
        /// </summary>
        /// <returns>A new queue if it can be popped.  Otherwise null if the back stack is empty.</returns>
        private List<classSampleData> GetNextOlderQueue()
        {
            if (undoRedoList.Count <= 1 || currentIndex == 0)
                return null;

            IsDirty = true;
            currentIndex--;
            var newQueue = undoRedoList[currentIndex];

            return newQueue;
        }

        /// <summary>
        /// Gets the next older queue in the history of queue changes
        /// </summary>
        /// <returns>A new queue if available.  Otherwise null if the history is empty.</returns>
        private List<classSampleData> GetNextNewerQueue()
        {
            if (undoRedoList.Count <= 1 || currentIndex == undoRedoList.Count - 1)
                return null;

            IsDirty = true;
            currentIndex++;
            var newQueue = undoRedoList[currentIndex];

            return newQueue;
        }

        private void SetCanUndoRedo()
        {
            CanUndo = currentIndex > 0;
            CanRedo = 0 <= currentIndex && currentIndex < undoRedoList.Count - 1;
        }

        /// <summary>
        /// Add the current working queue to the list of undoable actions
        /// </summary>
        /// <param name="workingQueue"></param>
        public void AddToUndoable(List<classSampleData> workingQueue)
        {
            PushQueue(workingQueue);
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        /// <returns>true if the working queue was modified</returns>
        public bool Undo(List<classSampleData> workingQueue)
        {
            // Pop the first item on the queue
            var queue = GetNextOlderQueue();
            var modified = false;

            // Then if popping
            if (queue != null && queue.Count > 0)
            {
                // Transfer the new queue to our waiting queue.
                workingQueue.Clear();
                workingQueue.AddRange(queue);
                modified = true;
            }

            SetCanUndoRedo();

            return modified;
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        /// <returns>true if the working queue was modified</returns>
        public bool Redo(List<classSampleData> workingQueue)
        {
            // Pull the queue off the forward stack if one exists
            var queue = GetNextNewerQueue();
            var modified = false;

            if (queue != null && queue.Count > 0)
            {
                workingQueue.Clear();
                workingQueue.AddRange(queue);
                modified = true;
            }

            SetCanUndoRedo();

            return modified;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
