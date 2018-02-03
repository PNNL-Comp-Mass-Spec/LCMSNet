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
        /// Stack of waiting queues for undo operations.
        /// </summary>
        /// <remarks>
        /// The item on the top of the stack is generally the same as the displayed data; the second item down is what should be shown after a undo operation
        /// </remarks>
        private readonly Stack<List<classSampleData>> undoBackWaitingQueue;

        /// <summary>
        /// Stack of samples for redo operations
        /// </summary>
        /// <remarks>
        /// Redo stack: the item on the top of the stack is the data to restore with a redo.
        /// </remarks>
        private readonly Stack<List<classSampleData>> undoForwardWaitingQueue;

        public SampleQueueUndoRedo()
        {
            // Undo - redo operations
            undoBackWaitingQueue = new Stack<List<classSampleData>>();
            undoForwardWaitingQueue = new Stack<List<classSampleData>>();
        }

        /// <summary>
        /// Pushes the queue onto the backstack and the
        /// </summary>
        /// <param name="backStack">Undo stack</param>
        /// <param name="forwardStack">Redo stack; if null, no forward operation will be handled</param>
        /// <param name="queue">Queue to push onto the stack</param>
        /// <param name="clearForward">if the forward/redo stack should be cleared</param>
        /// <remarks>
        /// The undo/redo stacks are handled as follows:
        /// Undo stack: the item on the top of the stack is generally the same as the displayed data; the second item down is what should be shown after a undo operation
        /// Redo stack: the item on the top of the stack is the data to restore with a redo.
        /// </remarks>
        private void PushQueue(Stack<List<classSampleData>> backStack,
            Stack<List<classSampleData>> forwardStack,
            List<classSampleData> queue,
            bool clearForward = true)
        {
            //
            // If the user wants us to clear the forward stack then we will,
            // otherwise we ignore it.
            //
            if (clearForward)
                forwardStack?.Clear();

            var pushQueue = new List<classSampleData>();
            foreach (var data in queue)
            {
                var sample = data.Clone() as classSampleData;
                if (sample?.LCMethod?.Name != null)
                {
                    if (classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                    {
                        //
                        // Because sample clones are deep copies, we cannot trust that
                        // every object in the sample is serializable...so...we are stuck
                        // making sure we re-hash the method using the name which
                        // is copied during the serialization.
                        //
                        sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name];
                    }
                }
                pushQueue.Add(sample);
            }
            backStack.Push(pushQueue);
            IsDirty = true;

            SetCanUndoRedo();
        }

        /// <summary>
        /// Pops the queue from the stack if available
        /// </summary>
        /// <param name="backStack">Undo stack to pop queue from.</param>
        /// <returns>A new queue if it can be popped.  Otherwise null if the back stack is empty.</returns>
        private List<classSampleData> PopQueue(Stack<List<classSampleData>> backStack)
        {
            if (backStack.Count < 1)
                return null;

            IsDirty = true;
            var newQueue = backStack.Pop();

            return newQueue;
        }

        /// <summary>
        /// Gets the top queue from the stack if available, but doesn't remove it
        /// </summary>
        /// <param name="stack">Stack to get the queue from.</param>
        /// <returns>A new queue if there is one.  Otherwise null if the stack is empty.</returns>
        private List<classSampleData> PeekQueue(Stack<List<classSampleData>> stack)
        {
            if (stack.Count < 1)
                return null;

            IsDirty = true;
            var newQueue = stack.Peek();

            return newQueue;
        }

        private void SetCanUndoRedo()
        {
            CanUndo = undoBackWaitingQueue.Count > 1;
            CanRedo = undoForwardWaitingQueue.Count > 0;
        }

        /// <summary>
        /// Add the working queue to the list of undoable actions
        /// </summary>
        /// <param name="workingQueue"></param>
        public void AddToUndoable(List<classSampleData> workingQueue)
        {
            PushQueue(undoBackWaitingQueue, undoForwardWaitingQueue, workingQueue);
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        /// <returns>true if the working queue was modified</returns>
        public bool Undo(List<classSampleData> workingQueue)
        {
            // Pop the first item on the queue, which is usually identical to the displayed data
            var cqueue = PopQueue(undoBackWaitingQueue);
            // Get the item on top of the undo queue
            var queue = PeekQueue(undoBackWaitingQueue);
            var modified = false;

            // Then if popping
            if (queue != null && queue.Count > 0)
            {
                //
                // Save the current waiting queue onto the forward stack, thus saving it for a redo
                //
                PushQueue(undoForwardWaitingQueue, null, workingQueue);

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
            //
            // Pull the queue off the forward stack if one exists
            //
            var queue = PopQueue(undoForwardWaitingQueue);
            var modified = false;

            if (queue != null && queue.Count > 0)
            {
                //
                // Push the current queue onto the back stack, thus saving our waiting queue.
                //
                PushQueue(undoBackWaitingQueue, null, workingQueue);

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
