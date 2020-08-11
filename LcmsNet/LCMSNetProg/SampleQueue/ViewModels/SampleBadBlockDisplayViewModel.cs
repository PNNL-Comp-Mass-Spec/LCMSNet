using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleBadBlockDisplayViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleBadBlockDisplayViewModel()
        {
            var samples = new List<SampleData>();
            samples.Add(new SampleData());
            DisplaySamples(samples);
        }

        public SampleBadBlockDisplayViewModel(List<SampleData> samples)
        {
            DisplaySamples(samples);
        }

        private readonly ReactiveList<BlockErrorData> badSamples = new ReactiveList<BlockErrorData>();

        public IReadOnlyReactiveList<BlockErrorData> BadSamples => badSamples;

        private void DisplaySamples(List<SampleData> samples)
        {
            using (badSamples.SuppressChangeNotifications())
            {
                badSamples.AddRange(samples.Select(sample => new BlockErrorData(sample.DmsData.Batch, sample.DmsData.Block, sample.ColumnIndex + 1, sample.DmsData.DatasetName, sample.LCMethodName)));
            }
        }

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
