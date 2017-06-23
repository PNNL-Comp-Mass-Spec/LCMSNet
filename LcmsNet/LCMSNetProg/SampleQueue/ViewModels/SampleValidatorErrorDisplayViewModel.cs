using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Experiment;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleValidatorErrorDisplayViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleValidatorErrorDisplayViewModel()
        {
        }

        public SampleValidatorErrorDisplayViewModel(Dictionary<classSampleData, List<classSampleValidationError>> errorsSet)
        {
            using (errors.SuppressChangeNotifications())
            {
                foreach (var item in errorsSet)
                {
                    errors.AddRange(item.Value.Select(x => new ErrorData(item.Key, x)));
                }
            }
        }

        private readonly ReactiveList<ErrorData> errors = new ReactiveList<ErrorData>();

        public IReadOnlyReactiveList<ErrorData> Errors => errors;

        public class ErrorData
        {
            public classSampleData Sample { get; private set; }
            public classSampleValidationError Error { get; private set; }

            public ErrorData(classSampleData sample, classSampleValidationError error)
            {
                Sample = sample;
                Error = error;
            }
        }
    }
}
