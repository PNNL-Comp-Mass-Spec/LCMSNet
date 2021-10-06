using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Controls;
using DynamicData;
using DynamicData.Binding;
using LcmsNetSDK;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Reporting
{
    public class CreateErrorReportViewModel : ReactiveObject
    {
        private readonly List<ContentControl> controls = null;
        private readonly string logPath;
        private readonly LCMethodManager methodManager;

        [Obsolete("For WPF Design-time use only", true)]
        public CreateErrorReportViewModel()
        {
        }

        public CreateErrorReportViewModel(LCMethodManager manager, string logPath, List<ContentControl> controls)
        {
            methodManager = manager;
            methodManager.MethodAdded += MethodManager_MethodAdded;
            methodManager.MethodRemoved += MethodManager_MethodRemoved;

            this.logPath = logPath;
            this.controls = controls;

            foreach (var method in methodManager.AllLCMethods)
            {
                if (!LCMethodsList.Contains(method))
                {
                    lcMethodsList.Add(method);
                    LCMethodsSelected.Add(method);
                }
            }

            lcMethodsList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var lcMethodsBound).Subscribe();
            LCMethodsList = lcMethodsBound;

            CreateReportCommand = ReactiveCommand.Create(CreateReport);
        }

        private readonly SourceList<LCMethod> lcMethodsList = new SourceList<LCMethod>();

        public ReadOnlyObservableCollection<LCMethod> LCMethodsList { get; }
        public ObservableCollectionExtended<LCMethod> LCMethodsSelected { get; }

        #region Button Handler Events

        public ReactiveCommand<Unit, Unit> CreateReportCommand { get; }

        /// <summary>
        ///
        /// </summary>
        private void CreateReport()
        {
            var methods = new List<LCMethod>(LCMethodsSelected);

            var builder = new ErrorReportBuilder();
            var path = builder.CreateReport(controls, methods, logPath, "hardwareconfig.ini");

            var cartName = LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME);
            var errorReportPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_ERRORPATH);

            ErrorReportBuilder.CopyReportToServer(path, cartName, errorReportPath);
        }

        #endregion

        #region Method Manager Updates

        /// <summary>
        /// Removes the LC Method from the listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private void MethodManager_MethodRemoved(object sender, LCMethod method)
        {
            lcMethodsList.Remove(method);
        }

        /// <summary>
        /// Adds a new LC Method to the list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private void MethodManager_MethodAdded(object sender, LCMethod method)
        {
            lcMethodsList.Edit(list =>
            {
                if (!list.Contains(method))
                {
                    lcMethodsList.Add(method);
                }
            });
        }

        #endregion
    }
}
