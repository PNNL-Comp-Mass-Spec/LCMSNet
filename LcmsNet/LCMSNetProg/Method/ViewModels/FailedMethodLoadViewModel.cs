using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    /// <summary>
    /// Displays the device initialization errors.
    /// </summary>
    public class FailedMethodLoadViewModel : ReactiveObject
    {

        /// <summary>
        /// Default constructor for the failed method load view model that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public FailedMethodLoadViewModel()
        {
            var testErrors = new string[] {"Error 1", "Error 2", "Error 3"};
            errorList.Add(new ErrorListing("File 1", testErrors));
            errorList.Add(new ErrorListing("File 2", testErrors));
            errorList.Add(new ErrorListing("File 3", testErrors));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="errors"></param>
        public FailedMethodLoadViewModel(Dictionary<string, List<Exception>> errors)
        {
            UpdateList(errors);
        }

        private readonly ReactiveList<ErrorListing> errorList = new ReactiveList<ErrorListing>();

        public IReadOnlyReactiveList<ErrorListing> ErrorList => errorList;

        /// <summary>
        /// Updates the listview with the error device messages.
        /// </summary>
        /// <param name="errors"></param>
        public void UpdateList(Dictionary<string, List<Exception>> errors)
        {
            errorList.Clear();
            foreach (var file in errors)
            {
                errorList.Add(new ErrorListing(Path.GetFileNameWithoutExtension(file.Key), file.Value.Select(x => x.Message)));
            }
        }
    }

    public class ErrorListing : ReactiveObject
    {
        public ErrorListing(string filename = "", IEnumerable<string> errors = null)
        {
            FileName = filename;
            using (Errors.SuppressChangeNotifications())
            {
                Errors.AddRange(errors);
            }
        }

        private string fileName = "";
        private readonly ReactiveList<string> errors = new ReactiveList<string>();

        public string FileName
        {
            get { return fileName; }
            set { this.RaiseAndSetIfChanged(ref fileName, value); }
        }

        public ReactiveList<string> Errors => errors;
    }
}
