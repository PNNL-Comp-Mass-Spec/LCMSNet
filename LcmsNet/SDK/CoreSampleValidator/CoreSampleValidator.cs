﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Experiment;
using LcmsNetSDK.Method;

namespace CoreSampleValidator
{
    /// <summary>
    /// Validates a sample for the basics before it can be run by LCMSNet.
    /// </summary>
    [Export(typeof(ISampleValidator))]
    [ExportMetadata("Name", "CoreSampleValidator")]
    [ExportMetadata("Version", "1.0")]
    public class CoreSampleValidator : ISampleValidator
    {
        /// <summary>
        /// Validates a sample based on the methods being correct and not in error.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public List<SampleValidationError> ValidateSamples(SampleData sample)
        {
            var errors = new List<SampleValidationError>();

            // Validate the LC-Method
            if (string.IsNullOrWhiteSpace(sample.LCMethodName) || !LCMethodManager.Manager.TryGetLCMethod(sample.LCMethodName, out var method))
            {
                errors.Add(new SampleValidationError("The LC-Method was not selected.", SampleValidationErrorType.LCMethodNotSelected));
            }
            else
            {
                var manager = DeviceManager.Manager;
                if (method.Events.Count < 1)
                {
                    errors.Add(new SampleValidationError("The LC-Method was not selected or does not contain any events.", SampleValidationErrorType.LCMethodHasNoEvents));
                }
                else
                {
                    foreach (var lcEvent in method.Events)
                    {
                        // VALIDATE THE DEVICE!!!
                        var device = lcEvent.Device;

                        // Make sure the device is not null (it shouldn't be, but...)
                        if (device != null)
                        {
                            // Make sure the device exists in the configuration correctly!
                            if (device.GetType() == typeof(TimerDevice))
                            {
                                // Do nothing!
                            }
                            else if (!manager.Devices.Contains(device))
                            {
                                errors.Add(new SampleValidationError(
                                    $"The device {device.Name} in the LC-Method does not exist.  Make sure the method is valid.",
                                    SampleValidationErrorType.DeviceDoesNotExist));
                            }
                        }
                        else
                        {
                            errors.Add(new SampleValidationError(
                                $"The LC-Method {method.Name} is invalid. Make sure the method is built correctly. ",
                                SampleValidationErrorType.LCMethodIncorrect));
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
        public List<SampleData> ValidateBlocks(List<SampleData> samples)
        {
            // First we want to bin the samples based on block number, then we want to
            // figure out for each block if we are scheduled to run on the same column.
            var tempDictionary = new Dictionary<string,List<SampleData>>();
            foreach (var sample in samples)
            {
                var block = sample.DmsData.Block;
                var batch = sample.DmsData.Batch;
                // If the items are blocked, then they need to run on one column.  For batched samples we don't care.
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
                    tempDictionary.Add(key, new List<SampleData>(){sample});
                }
            }

            var badSamples = new List<SampleData>();

            // Iterate over the batches
            foreach (var itemKey in tempDictionary.Keys)
            {
                // Iterate over the blocks
                var tempSamples = tempDictionary[itemKey];
                var method = tempSamples[0].LCMethodName;
                var columnID = tempSamples[0].ColumnIndex;

                // Find a mis match between any of the columns. By communicative property
                // we only need to use one of the column id values to do this and perform a
                // O(n) search.
                for (var i = 1; i < tempSamples.Count; i++)
                {
                    // Make sure we also look at the sample method ... this is important.
                    if (tempSamples[i].ColumnIndex != columnID || method != tempSamples[i].LCMethodName)
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
