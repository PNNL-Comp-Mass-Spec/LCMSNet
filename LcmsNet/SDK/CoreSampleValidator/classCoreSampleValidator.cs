using System.Collections.Generic;
using System.ComponentModel.Composition;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Experiment;

namespace LcmsNetDataClasses.Experiment
{
    /// <summary>
    /// Validates a sample for the basics before it can be run by LCMSNet.
    /// </summary>
    [Export(typeof(ISampleValidator))]
    [ExportMetadata("Name", "CoreSampleValidator")]
    [ExportMetadata("Version", "1.0")]
    public class classCoreSampleValidator : ISampleValidator
    {
        /// <summary>
        /// Validates a sample based on the methods being correct and not in error.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public List<classSampleValidationError> ValidateSamples(classSampleData sample)
        {
            var errors = new List<classSampleValidationError>();

            // Validate the LC-Method
            if (sample.ActualLCMethod == null)
            {
                errors.Add(new classSampleValidationError("The LC-Method was not selected.", enumSampleValidationError.LCMethodNotSelected));
            }
            else
            {
                var manager = classDeviceManager.Manager;
                if (sample.ActualLCMethod.Events.Count < 1)
                {
                    errors.Add(new classSampleValidationError("The LC-Method was not selected or does not contain any events.", enumSampleValidationError.LCMethodHasNoEvents));
                }
                else
                {
                    foreach (var lcEvent in sample.ActualLCMethod.Events)
                    {
                        // VALIDATE THE DEVICE!!!
                        var device = lcEvent.Device;

                        // Make sure the device is not null and somehow snuck in
                        if (device != null)
                        {
                            // Make sure the device exists in the configuration correctly!
                            if (device.GetType() == typeof(classTimerDevice))
                            {
                                // Do nothing!
                            }
                            else if (!manager.Devices.Contains(device))
                            {
                                errors.Add(new classSampleValidationError(
                                                    string.Format("The device {0} in the LC-Method does not exist.  Make sure the method is valid.", device.Name),
                                                    enumSampleValidationError.DeviceDoesNotExist));
                            }
                        }
                        else
                        {
                            errors.Add(new classSampleValidationError(
                                                string.Format("The LC-Method {0} is invalid. Make sure the method is built correctly. ", sample.ActualLCMethod.Name),
                                                enumSampleValidationError.LCMethodIncorrect));
                        }
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Validates a list of samples to make sure if they are from the same block, they run on the same column.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public List<classSampleData> ValidateBlocks(List<classSampleData> samples)
        {
            // First we want to bin the samples based on block number, then we want to
            // figure out for each block if we are scheduled to run on the same column.
            var tempDictionary = new Dictionary<string,List<classSampleData>>();
            foreach (var sample in samples)
            {
                var block = sample.DmsData.Block;
                var batch = sample.DmsData.Batch;
                // If the items are blocked, then they need to run on one column.  For batched samples we dont care.
                if (block < 1)
                {
                    continue;
                }

                var key = batch.ToString() + "-" + block.ToString();
                if (tempDictionary.ContainsKey(key))
                {
                    tempDictionary[key].Add(sample);
                }
                else
                {
                    tempDictionary.Add(key, new List<classSampleData>(){sample});
                }
            }

            var badSamples = new List<classSampleData>();

            // Iterate over the batches
            foreach (var itemKey in tempDictionary.Keys)
            {
                // Iterate over the blocks
                var tempSamples   = tempDictionary[itemKey];
                var method                = tempSamples[0].ActualLCMethod;
                var columnID                        = tempSamples[0].ColumnData.ID;

                // Find a mis match between any of the columns. By communicative property
                // we only need to use one of the column id values to do this and perform a
                // O(n) search.
                for (var i = 1; i < tempSamples.Count; i++)
                {
                    // Make sure we also look at the sample method ... this is important.
                    if (tempSamples[i].ColumnData.ID != columnID || method.Name != tempSamples[i].ActualLCMethod.Name)
                    {
                        badSamples.AddRange(tempSamples);
                        break;
                    }
                }
            }

            return badSamples;
        }
    }
}
