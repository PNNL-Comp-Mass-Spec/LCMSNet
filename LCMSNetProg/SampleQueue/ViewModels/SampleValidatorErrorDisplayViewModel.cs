using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LcmsNet.Data;
using LcmsNetSDK.Data;
using LcmsNetSDK.SampleValidation;
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

        public SampleValidatorErrorDisplayViewModel(Dictionary<SampleData, List<SampleValidationError>> errorsSet)
        {
            var errors = new List<ErrorData>();
            foreach (var item in errorsSet)
            {
                errors.AddRange(item.Value.Select(x => new ErrorData(item.Key, x)));
            }

            Errors = errors.AsReadOnly();
        }

        public ReadOnlyCollection<ErrorData> Errors { get; }

        public class ErrorData
        {
            public SampleData Sample { get; }
            public SampleValidationError Error { get; }

            public ErrorData(SampleData sample, SampleValidationError error)
            {
                Sample = sample;
                Error = error;
            }
        }
    }
}
