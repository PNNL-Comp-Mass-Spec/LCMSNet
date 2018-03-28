﻿using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetSDK.Data;
using LcmsNetSDK.Experiment;
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
            public SampleData Sample { get; private set; }
            public SampleValidationError Error { get; private set; }

            public ErrorData(SampleData sample, SampleValidationError error)
            {
                Sample = sample;
                Error = error;
            }
        }
    }
}
