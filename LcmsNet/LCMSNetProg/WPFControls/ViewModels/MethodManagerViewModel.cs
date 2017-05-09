using System;
using LcmsNet.SampleQueue;
using ReactiveUI;

namespace LcmsNet.WPFControls.ViewModels
{
    public class MethodManagerViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the column manager view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public MethodManagerViewModel()
        {
            Method1ViewModel = new MethodControlViewModel() { CommandsVisible = false };
            Method2ViewModel = new MethodControlViewModel() { CommandsVisible = false };
            Method3ViewModel = new MethodControlViewModel() { CommandsVisible = false };
            Method4ViewModel = new MethodControlViewModel() { CommandsVisible = false };
            Method5ViewModel = new MethodControlViewModel() { CommandsVisible = false };
            Method6ViewModel = new MethodControlViewModel() { CommandsVisible = false };
            Method7ViewModel = new MethodControlViewModel() { CommandsVisible = false };
            Method8ViewModel = new MethodControlViewModel() { CommandsVisible = false };
        }

        public MethodManagerViewModel(formDMSView dmsView, SampleDataManager sampleDataManager)
        {
            Method1ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };
            Method2ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };
            Method3ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };
            Method4ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };
            Method5ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };
            Method6ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };
            Method7ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };
            Method8ViewModel = new MethodControlViewModel(dmsView, sampleDataManager) { CommandsVisible = false };

            this.WhenAnyValue(x => x.Method1ViewModel, x => x.Method1ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);
            this.WhenAnyValue(x => x.Method2ViewModel, x => x.Method2ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);
            this.WhenAnyValue(x => x.Method3ViewModel, x => x.Method3ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);
            this.WhenAnyValue(x => x.Method4ViewModel, x => x.Method4ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);
            this.WhenAnyValue(x => x.Method5ViewModel, x => x.Method5ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);
            this.WhenAnyValue(x => x.Method6ViewModel, x => x.Method6ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);
            this.WhenAnyValue(x => x.Method7ViewModel, x => x.Method7ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);
            this.WhenAnyValue(x => x.Method8ViewModel, x => x.Method8ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedMethod = x.Item2 ? x.Item1 : this.FocusedMethod);

            FocusedMethod = method1ViewModel;

            SampleQueueComboBoxOptions.LcMethodOptions.Changed.Subscribe(x => this.SetMaxMethodsVisible());
            SetMaxMethodsVisible();
            this.WhenAnyValue(x => x.MethodsVisible).Subscribe(x => this.SetMethodVisibility());
            SetMethodVisibility();
        }

        private MethodControlViewModel method1ViewModel;
        private MethodControlViewModel method2ViewModel;
        private MethodControlViewModel method3ViewModel;
        private MethodControlViewModel method4ViewModel;
        private MethodControlViewModel method5ViewModel;
        private MethodControlViewModel method6ViewModel;
        private MethodControlViewModel method7ViewModel;
        private MethodControlViewModel method8ViewModel;
        private MethodControlViewModel focusedMethod;
        private int methodsVisible = 2;
        private int maxMethodsVisible = 2;

        public MethodControlViewModel Method1ViewModel
        {
            get { return method1ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method1ViewModel, value); }
        }

        public MethodControlViewModel Method2ViewModel
        {
            get { return method2ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method2ViewModel, value); }
        }

        public MethodControlViewModel Method3ViewModel
        {
            get { return method3ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method3ViewModel, value); }
        }

        public MethodControlViewModel Method4ViewModel
        {
            get { return method4ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method4ViewModel, value); }
        }

        public MethodControlViewModel Method5ViewModel
        {
            get { return method5ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method5ViewModel, value); }
        }

        public MethodControlViewModel Method6ViewModel
        {
            get { return method6ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method6ViewModel, value); }
        }

        public MethodControlViewModel Method7ViewModel
        {
            get { return method7ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method7ViewModel, value); }
        }

        public MethodControlViewModel Method8ViewModel
        {
            get { return method8ViewModel; }
            set { this.RaiseAndSetIfChanged(ref method8ViewModel, value); }
        }

        public MethodControlViewModel FocusedMethod
        {
            get { return focusedMethod; }
            set { this.RaiseAndSetIfChanged(ref focusedMethod, value); }
        }

        public int MethodsVisible
        {
            get { return methodsVisible; }
            set { this.RaiseAndSetIfChanged(ref methodsVisible, value); }
        }

        public int MaxMethodsVisible
        {
            get { return maxMethodsVisible; }
            private set { this.RaiseAndSetIfChanged(ref maxMethodsVisible, value); }
        }

        public void ClearFocused()
        {
            Method1ViewModel.ContainsKeyboardFocus = false;
            Method2ViewModel.ContainsKeyboardFocus = false;
            Method3ViewModel.ContainsKeyboardFocus = false;
            Method4ViewModel.ContainsKeyboardFocus = false;
            Method5ViewModel.ContainsKeyboardFocus = false;
            Method6ViewModel.ContainsKeyboardFocus = false;
            Method7ViewModel.ContainsKeyboardFocus = false;
            Method8ViewModel.ContainsKeyboardFocus = false;
        }

        private void SetMaxMethodsVisible()
        {
            MaxMethodsVisible = Math.Min(SampleQueueComboBoxOptions.LcMethodOptions.Count + 1, 8);
            MethodsVisible = Math.Max(MethodsVisible, MaxMethodsVisible);
        }

        private void SetMethodVisibility()
        {
            var counter = MethodsVisible;
            Method1ViewModel.MethodVisible = counter-- > 0;
            Method2ViewModel.MethodVisible = counter-- > 0;
            Method3ViewModel.MethodVisible = counter-- > 0;
            Method4ViewModel.MethodVisible = counter-- > 0;
            Method5ViewModel.MethodVisible = counter-- > 0;
            Method6ViewModel.MethodVisible = counter-- > 0;
            Method7ViewModel.MethodVisible = counter-- > 0;
            Method8ViewModel.MethodVisible = counter-- > 0;
        }
    }
}
