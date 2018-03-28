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
            var samples = new List<classSampleData>();
            samples.Add(new classSampleData());
            DisplaySamples(samples);
        }

        public SampleBadBlockDisplayViewModel(List<classSampleData> samples)
        {
            DisplaySamples(samples);
        }

        private readonly ReactiveList<BlockErrorData> badSamples = new ReactiveList<BlockErrorData>();

        public IReadOnlyReactiveList<BlockErrorData> BadSamples => badSamples;

        private void DisplaySamples(List<classSampleData> samples)
        {
            using (badSamples.SuppressChangeNotifications())
            {
                badSamples.AddRange(samples.Select(sample => new BlockErrorData(sample.DmsData.Batch, sample.DmsData.Block, sample.ColumnData.ID + 1, sample.DmsData.DatasetName, sample.LCMethod.Name)));
            }
        }

        public class BlockErrorData
        {
            public int Batch { get; private set; }
            public int Block { get; private set; }
            public int Column { get; private set; }
            public string DatasetName { get; private set; }
            public string MethodName { get; private set; }

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
