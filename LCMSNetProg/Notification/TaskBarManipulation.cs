﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Shell;

namespace LcmsNet.Notification
{
    public class TaskBarManipulation : IDisposable
    {
        public static TaskBarManipulation Instance { get; } = new TaskBarManipulation();

        private TaskBarManipulation()
        {
        }

        ~TaskBarManipulation()
        {
            Dispose();
        }

        public void Dispose()
        {
            blinkTimer?.Dispose();
            blinkTimeout?.Dispose();
            GC.SuppressFinalize(this);
        }

        private Timer blinkTimer = null;
        private Timer blinkTimeout = null;
        private TaskbarItemProgressState previousState = TaskbarItemProgressState.None;
        private double previousValue = 0.0;

        /// <summary>
        /// Blink the taskbar
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds, or Timeout.Infinite</param>
        public void BlinkTaskbar(int timeout)
        {
            if (blinkTimer != null)
            {
                return;
            }
            SafelyInvoke(() =>
            {
                if (Application.Current?.MainWindow?.TaskbarItemInfo == null)
                {
                    return;
                }
                var taskbar = Application.Current.MainWindow.TaskbarItemInfo;
                previousState = taskbar.ProgressState;
                previousValue = taskbar.ProgressValue;
                taskbar.ProgressValue = 100;
                taskbar.ProgressState = TaskbarItemProgressState.Paused;
            });
            blinkTimer = new Timer(BlinkTaskbarColor, this, 500, 500);
            if (timeout != Timeout.Infinite)
            {
                blinkTimeout = new Timer(StopTaskbarBlink, this, timeout, Timeout.Infinite);
            }
        }

        public void BlinkTaskbar()
        {
            BlinkTaskbar(Timeout.Infinite);
        }

        public void StopTaskbarBlink()
        {
            if (blinkTimer != null)
            {
                blinkTimer.Dispose();
                blinkTimer = null;

                SafelyInvoke(() =>
                {
                    if (Application.Current?.MainWindow?.TaskbarItemInfo == null)
                    {
                        return;
                    }
                    Application.Current.MainWindow.TaskbarItemInfo.ProgressState = previousState;
                    Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = previousValue;
                });
            }
        }

        private void StopTaskbarBlink(object sender)
        {
            StopTaskbarBlink();
            if (blinkTimeout != null)
            {
                blinkTimeout.Dispose();
                blinkTimeout = null;
            }
        }

        private void BlinkTaskbarColor(object sender)
        {
            SafelyInvoke(() =>
            {
                if (Application.Current?.MainWindow?.TaskbarItemInfo == null)
                {
                    return;
                }
                var taskbar = Application.Current.MainWindow.TaskbarItemInfo;
                if (taskbar.ProgressState != TaskbarItemProgressState.None)
                {
                    taskbar.ProgressState = TaskbarItemProgressState.None;
                }
                else
                {
                    if (!previousValue.Equals(taskbar.ProgressValue))
                    {
                        previousValue = taskbar.ProgressValue;
                        taskbar.ProgressValue = 100;
                    }
                    taskbar.ProgressState = TaskbarItemProgressState.Paused;
                }
            });
        }

        private static void SafelyInvoke(Action action)
        {
            try
            {
                if (Application.Current == null)
                {
                    return;
                }

                var dispatchObject = Application.Current.Dispatcher;
                if (dispatchObject == null || dispatchObject.CheckAccess())
                {
                    action();
                }
                else
                {
                    dispatchObject.Invoke(action);
                }
            }
            catch (Exception)
            {
                // suppress errors; generally only happens when closing during a task, when the UI thread becomes invalid.
            }
        }
    }
}
