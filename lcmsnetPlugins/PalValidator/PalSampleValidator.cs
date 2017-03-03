using System.Collections.Generic;
using System.ComponentModel.Composition;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Data;

namespace PALAutoSampler.Validator
{
    [Export(typeof(ISampleValidator))]
    [ExportMetadata("Name", "PalSampleValidations")]
    [ExportMetadata("Version", "1.0")]
    public class PalSampleValidator:ISampleValidator
    {

        public List<LcmsNetDataClasses.classSampleData> ValidateBlocks(List<LcmsNetDataClasses.classSampleData> samples)
        {
            // Blocks don't matter to the PAL.
            return new List<LcmsNetDataClasses.classSampleData>();
        }

        public List<classSampleValidationError> ValidateSamples(LcmsNetDataClasses.classSampleData sample)
        {
            var errors = new List<classSampleValidationError>();
            
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

                return errors;
        }
    }
}
