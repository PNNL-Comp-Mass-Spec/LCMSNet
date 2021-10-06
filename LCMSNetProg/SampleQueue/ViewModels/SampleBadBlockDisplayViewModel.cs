using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LcmsNet.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleBadBlockDisplayViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleBadBlockDisplayViewModel() : this(new List<SampleData> { new SampleData() })
        {
        }

        public SampleBadBlockDisplayViewModel(List<SampleData> samples)
        {
            BadSamples = samples.Select(sample => new BlockErrorData(sample.DmsData?.Batch ?? 0, sample.DmsData?.Block ?? 0,
                sample.ColumnIndex + 1, sample.Name, sample.LCMethodName)).ToList().AsReadOnly();
        }

        public ReadOnlyCollection<BlockErrorData> BadSamples {get; }

        public class BlockErrorData
        {
            public int Batch { get; }
            public int Block { get; }
            public int Column { get; }
            public string DatasetName { get; }
            public string MethodName { get; }

            public BlockErrorData(int batch, int block, int column, string datasetName, string methodName)
            {
                Batch = batch;
                Block = block;
                Column = column;
                DatasetName = datasetName;
                MethodName = methodName;
            }
        }
    }
}
