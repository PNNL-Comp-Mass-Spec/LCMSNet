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
            var errorList = new ReactiveList<ErrorListing>();
            var testErrors = new string[] {"Error 1", "Error 2", "Error 3"};
            errorList.Add(new ErrorListing("File 1", testErrors));
            errorList.Add(new ErrorListing("File 2", testErrors));
            errorList.Add(new ErrorListing("File 3", testErrors));
            ErrorList = errorList;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="errors"></param>
        public FailedMethodLoadViewModel(Dictionary<string, List<Exception>> errors)
        {
            ErrorList = new ReactiveList<ErrorListing>(errors.Select(file => new ErrorListing(Path.GetFileNameWithoutExtension(file.Key), file.Value.Select(x => x.Message))));
        }

        public IReadOnlyReactiveList<ErrorListing> ErrorList { get; }
    }

    public class ErrorListing : ReactiveObject
    {
        public ErrorListing(string filename = "", IEnumerable<string> errors = null)
        {
            FileName = filename;
            Errors = new ReactiveList<string>(errors);
        }

        public string FileName { get; }

        public IReadOnlyReactiveList<string> Errors { get; }
    }
}
