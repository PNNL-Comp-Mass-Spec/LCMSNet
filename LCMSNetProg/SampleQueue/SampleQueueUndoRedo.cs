using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LcmsNet.Data;
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
            get => canUndo;
            private set => this.RaiseAndSetIfChanged(ref canUndo, value);
        }

        /// <summary>
        /// If there are queues in the redo queue
        /// </summary>
        /// <returns></returns>
        public bool CanRedo
        {
            get => canRedo;
            private set => this.RaiseAndSetIfChanged(ref canRedo, value);
        }

        public bool IsDirty { get; private set; }

        /// <summary>
        /// List/history of queue changes to support undo and redo operations; undo is performed by decrementing the current index back by 1 and returning the queue at the new index, redo is performed by incrementing the current index by one and returning the queue at the new index
        /// </summary>
        private readonly List<List<SampleData>> undoRedoList;

        /// <summary>
        /// The index of the currently displayed queue in the list/history of queue changes
        /// </summary>
        private int currentIndex = 0;

        public SampleQueueUndoRedo()
        {
            // Undo - redo operations
            undoRedoList = new List<List<SampleData>>();
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
        private void PushQueue(List<SampleData> workingQueue)
        {
            var pushQueue = new List<SampleData>();
            foreach (var data in workingQueue)
            {
                var sample = data.Clone(true);
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
        private List<SampleData> GetNextOlderQueue()
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
        private List<SampleData> GetNextNewerQueue()
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
        public void AddToUndoable(List<SampleData> workingQueue)
        {
            PushQueue(workingQueue);
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        /// <param name="undoItems">The most recent, but not current "waiting"/"not queued to run" sample list</param>
        /// <returns>true if undoItems is valid</returns>
        public bool Undo(out List<SampleData> undoItems)
        {
            // Pop the first item on the queue
            var queue = GetNextOlderQueue();
            undoItems = new List<SampleData>();
            var modified = false;

            // Then if popping
            if (queue != null && queue.Count > 0)
            {
                // Transfer the new queue to our waiting queue.
                undoItems.AddRange(queue);
                modified = true;
            }

            SetCanUndoRedo();

            return modified;
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        /// <param name="redoItems">The most recently undone "waiting"/"not queued to run" sample list</param>
        /// <returns>true if redoItems is valid</returns>
        public bool Redo(out List<SampleData> redoItems)
        {
            // Pull the queue off the forward stack if one exists
            var queue = GetNextNewerQueue();
            redoItems = new List<SampleData>();
            var modified = false;

            if (queue != null && queue.Count > 0)
            {
                redoItems.AddRange(queue);
                modified = true;
            }

            SetCanUndoRedo();

            return modified;
        }

        public void Clear()
        {
            undoRedoList.Clear();
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
