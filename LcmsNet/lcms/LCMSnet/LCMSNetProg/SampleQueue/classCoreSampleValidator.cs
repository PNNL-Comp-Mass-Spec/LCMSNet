using System;
using System.Collections.Generic;

//using LcmsNet.Devices;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Configuration;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Validates a sample for the basics before it can be run by LCMSNet.
    /// </summary>
    public class classCoreSampleValidator
    {
        /// <summary>
        /// Validates a sample based on the methods being correct and not in error.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public List<classSampleValidationError> ValidateSamples(classSampleData sample)
        {
            List<classSampleValidationError> errors = new List<classSampleValidationError>();

            ///////////////////////////////////////////////////////////////////////////////////////////// 
            /// Validate the LC-Method
            ///////////////////////////////////////////////////////////////////////////////////////////// 
            if (sample.LCMethod == null)
            {
                errors.Add(new classSampleValidationError("The LC-Method was not selected.", enumSampleValidationError.LCMethodNotSelected));
            }
            else
            {
                classDeviceManager manager = classDeviceManager.Manager;
                if (sample.LCMethod.Events.Count < 1)
                {
                    errors.Add(new classSampleValidationError("The LC-Method was not selected or does not contain any events.", enumSampleValidationError.LCMethodHasNoEvents));
                }
                else
                {
                    foreach (classLCEvent lcEvent in sample.LCMethod.Events)
                    {
                        ////////////////////////////////////////////////////////////////////////////////////////// 
                        /// VALIDATE THE DEVICE!!!
                        //////////////////////////////////////////////////////////////////////////////////////////
                        IDevice device = lcEvent.Device;
                        /// 
                        /// Make sure the device is not null and somehow snuck in
                        /// 
                        if (device != null)
                        {
                            /// 
                            /// Make sure the device exists in the configuration correctly!
                            /// 
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
                                                string.Format("The LC-Method {0} is invalid. Make sure the method is built correctly. ", sample.LCMethod.Name),
                                                enumSampleValidationError.LCMethodIncorrect));
                        }
                    }
                }           
     
                /// 
                /// We've validated the method, and the devices... Now we need to validate the PAL settings.
                /// 
                if (string.IsNullOrEmpty(sample.PAL.Method))
                    errors.Add(new classSampleValidationError("The PAL Method is not set.", enumSampleValidationError.PalMethodNotSpecified));

				if (string.IsNullOrEmpty(sample.PAL.PALTray))
				{
					errors.Add(new classSampleValidationError("The PAL Tray is not set.", enumSampleValidationError.PalTrayNotSpecified));
				}
				else if (sample.PAL.PALTray == "(select)")
				{
					errors.Add(new classSampleValidationError("The PAL Tray is not set.", enumSampleValidationError.PalTrayNotSpecified));
				}

				if (sample.DmsData.DatasetName.ToLower() == "(unused)")
				{
					errors.Add(new classSampleValidationError("The dataset name is not correct.", enumSampleValidationError.DatasetNameError));
				}

                /// 
                /// Make sure the volume is set.
                /// 
                if (sample.Volume < classCartConfiguration.MinimumVolume)
                {
                    errors.Add(new classSampleValidationError("The Sample Volume is lower than the instrument can handle.", enumSampleValidationError.InjectionVolumeOutOfRange));
                }

                //TODO: Fix this to validate what kind of tray/vial setup is being used.
                if (sample.PAL.Well <= classPalData.CONST_DEFAULT_VIAL_NUMBER || sample.PAL.Well > 96)
                    errors.Add(new classSampleValidationError("The PAL Well/Vial is not set.", enumSampleValidationError.PalVialNotSpecified));
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
            /// 
            /// First we want to bin the samples based on block number, then we want to 
            /// figure out for each block if we are scheduled to run on the same column.
            /// 
            Dictionary<string, List<classSampleData>> tempDictionary = new Dictionary<string,List<classSampleData>>();
            foreach (classSampleData sample in samples)
            {
                int block = sample.DmsData.Block;
                int batch = sample.DmsData.Batch;
				// If the items are blocked, then they need to run on one column.  For batched samples we dont care.
                if (block < 1)
                {
                    continue;
                }

                string key = batch.ToString() + "-" + block.ToString();
                if (tempDictionary.ContainsKey(key))
                {
                    tempDictionary[key].Add(sample);
                }
                else
                {
                    tempDictionary.Add(key, new List<classSampleData>(){sample});
                }
            }

            /// 
            /// I could probably make this one line of code...
            /// But here we are seeing what blocks are distributed across columns.
            /// 
            List<classSampleData> badSamples = new List<classSampleData>();
            /// 
            /// Iterate over the batches
            /// 
            foreach (string itemKey in tempDictionary.Keys)
            {
                /// 
                /// Iterate over the blocks
                /// 
                List<classSampleData> tempSamples   = tempDictionary[itemKey];
                classLCMethod method                = tempSamples[0].LCMethod;
                int columnID                        = tempSamples[0].ColumnData.ID;

                /// 
                /// Find a mis match between any of the columns. By communicative property
                /// we only need to use one of the column id values to do this and perform a 
                /// O(n) search.
                /// 
                for (int i = 1; i < tempSamples.Count; i++)
                {
                    /// 
                    /// Make sure we also look at the sample method ... this is important.
                    /// 
                    if (tempSamples[i].ColumnData.ID != columnID || method.Name != tempSamples[i].LCMethod.Name)
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
