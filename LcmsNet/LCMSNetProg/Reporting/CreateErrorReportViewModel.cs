using System.Collections.Generic;
using System.Reactive;
using System.Windows.Controls;
using LcmsNet.Method;
using LcmsNetSDK;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Reporting
{
    public class CreateErrorReportViewModel : ReactiveObject
    {
        private readonly List<ContentControl> controls = null;
        private readonly string logPath;
        private readonly LCMethodManager methodManager;

        public CreateErrorReportViewModel()
        {
            SetupCommands();
        }

        public CreateErrorReportViewModel(LCMethodManager manager, string logPath, List<ContentControl> controls)
        {
            methodManager = manager;
            methodManager.MethodAdded += MethodManager_MethodAdded;
            methodManager.MethodRemoved += MethodManager_MethodRemoved;

            this.logPath = logPath;
            this.controls = controls;

            foreach (var method in methodManager.Methods.Values)
            {
                if (!lcMethodsList.Contains(method))
                {
                    lcMethodsList.Add(method);
                    lcMethodsSelected.Add(method);
                }
            }

            SetupCommands();
        }

        private readonly ReactiveList<LCMethod> lcMethodsList = new ReactiveList<LCMethod>();
        private readonly ReactiveList<LCMethod> lcMethodsSelected = new ReactiveList<LCMethod>();

        public IReadOnlyReactiveList<LCMethod> LCMethodsList => lcMethodsList;
        public ReactiveList<LCMethod> LCMethodsSelected => lcMethodsSelected;

        #region Button Handler Events

        public ReactiveCommand<Unit, Unit> CreateReportCommand { get; private set; }

        private void SetupCommands()
        {
            CreateReportCommand = ReactiveCommand.Create(() => CreateReport());
        }

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
        private bool MethodManager_MethodRemoved(object sender, LCMethod method)
        {
            if (!lcMethodsList.Contains(method))
                return true;

            lcMethodsList.Remove(method);
            return true;
        }

        /// <summary>
        /// Adds a new LC Method to the list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool MethodManager_MethodAdded(object sender, LCMethod method)
        {
            if (lcMethodsList.Contains(method))
                return true;

            lcMethodsList.Add(method);
            return true;
        }

        #endregion
    }
}
