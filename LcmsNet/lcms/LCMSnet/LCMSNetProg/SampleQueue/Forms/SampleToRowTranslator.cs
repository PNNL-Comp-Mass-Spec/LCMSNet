using LcmsNetDataClasses;

namespace LcmsNet.SampleQueue.Forms
{
    public class SampleToRowTranslator
    {
        /// <summary>
        /// Index Offset for going from a zero based array for configuration data to the
        /// user-readable column display.
        /// </summary>
        protected const int CONST_COLUMN_INDEX_OFFSET = 1;

        public SampleToRowTranslator(classSampleData sample)
        {
            Sample = sample;
        }

        public classSampleData Sample { get; set; }

        // controlSampleView.mcolumn_sequenceNumber
        public long SequenceNumber => Sample.SequenceID;

        // controlSampleView.mcolumn_columnNumber
        public string ColumnNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SpecialColumnNumber))
                {
                    return SpecialColumnNumber;
                }
                return (Sample.ColumnData.ID + CONST_COLUMN_INDEX_OFFSET).ToString();
            }
        }

        public string SpecialColumnNumber { get; set; }

        // controlSampleView.mcolumn_uniqueID,
        public long UniqueID => Sample.UniqueID;

        // controlSampleView.mcolumn_checkbox,
        public controlSampleView.enumCheckboxStatus Checkbox
        {
            get
            {
                var state = GetCheckboxStatusFromSampleStatus();
                if (state == controlSampleView.enumCheckboxStatus.Disabled)
                {
                    return controlSampleView.enumCheckboxStatus.Checked;
                }
                return state;
            }
        }

        // controlSampleView.mcolumn_checkbox.tag,
        public string CheckboxTag
        {
            get
            {
                var state = GetCheckboxStatusFromSampleStatus();
                if (state == controlSampleView.enumCheckboxStatus.Disabled)
                {
                    return "Disabled";
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the status message pertaining to a given samples running status.
        /// </summary>
        /// <returns>String representing the status of the running state.</returns>
        protected controlSampleView.enumCheckboxStatus GetCheckboxStatusFromSampleStatus()
        {
            var status = controlSampleView.enumCheckboxStatus.Disabled;
            switch (Sample.RunningStatus)
            {
                case enumSampleRunningStatus.Complete:
                    status = controlSampleView.enumCheckboxStatus.Disabled;
                    break;
                case enumSampleRunningStatus.Error:
                    status = controlSampleView.enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Stopped:
                    status = controlSampleView.enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Queued:
                    status = controlSampleView.enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Running:
                    status = controlSampleView.enumCheckboxStatus.Disabled;
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    status = controlSampleView.enumCheckboxStatus.Checked;
                    break;
                default:
                    //
                    // Should never get here
                    //
                    break;
            }

            return status;
        }

        // controlSampleView.Status
        public string Status
        {
            get
            {
                var statusMessage = "";
                switch (Sample.RunningStatus)
                {
                    case enumSampleRunningStatus.Complete:
                        statusMessage = "Complete";
                        break;
                    case enumSampleRunningStatus.Error:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage = "Block Error";
                        }
                        else
                        {
                            statusMessage = "Error";
                        }
                        break;
                    case enumSampleRunningStatus.Stopped:
                        statusMessage = "Stopped";
                        break;
                    case enumSampleRunningStatus.Queued:
                        statusMessage = "Queued";
                        break;
                    case enumSampleRunningStatus.Running:
                        statusMessage = "Running";
                        break;
                    case enumSampleRunningStatus.WaitingToRun:
                        statusMessage = "Waiting";
                        break;
                    default:
                        //
                        // Should never get here
                        //
                        break;
                }

                return statusMessage;
            }
        }

        // controlSampleView.Status.ToolTipText
        public string StatusToolTipText
        {
            get
            {
                var statusMessage = "";
                switch (Sample.RunningStatus)
                {
                    case enumSampleRunningStatus.Complete:
                        statusMessage = "The sample ran successfully.";
                        break;
                    case enumSampleRunningStatus.Error:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage =
                                "There was an error and this sample was part of a block.  You should re-run the block of samples";
                        }
                        else
                        {
                            statusMessage = "An error occured while running this sample.";
                        }
                        break;
                    case enumSampleRunningStatus.Stopped:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage =
                                "The sample was stopped but was part of a block.  You should re-run the block of samples";
                        }
                        else
                        {
                            statusMessage = "The sample execution was stopped.";
                        }
                        break;
                    case enumSampleRunningStatus.Queued:
                        statusMessage = "The sample is queued but not scheduled to run.";
                        break;
                    case enumSampleRunningStatus.Running:
                        statusMessage = "The sample is running.";
                        break;
                    case enumSampleRunningStatus.WaitingToRun:
                        statusMessage = "The sample is scheduled to run and waiting.";
                        break;
                    default:
                        //
                        // Should never get here
                        //
                        break;
                }

                return statusMessage;
            }
        }

        // controlSampleView.mcolumn_blockNumber
        public int BlockNumber => Sample.DmsData.Block;

        // controlSampleView.mcolumn_runOrder
        public int RunOrder => Sample.DmsData.RunOrder;

        // controlSampleView.mcolumn_requestName
        public string RequestName
        {
            get { return Sample.DmsData.RequestName; }
            set { Sample.DmsData.RequestName = value; }
        }

        // controlSampleView.mcolumn_PalTray
        public string PALTray
        {
            get { return Sample.PAL.PALTray; }
            set { Sample.PAL.PALTray = value; }
        }

        // controlSampleView.mcolumn_palVial
        public int PALVial
        {
            get { return Sample.PAL.Well; }
            set { Sample.PAL.Well = value; }
        }

        // controlSampleView.mcolumn_PALVolume
        public double PALVolume
        {
            get { return Sample.Volume; }
            set { Sample.Volume = value; }
        }

        // controlSampleView.mcolumn_LCMethod,
        public string LCMethod
        {
            get
            {
                if (Sample.LCMethod != null)
                {
                    return Sample.LCMethod.Name;
                }
                return string.Empty;
            }
            set
            {
                if (Sample.LCMethod != null)
                    Sample.LCMethod.Name = value;
            }
        }

        // controlSampleView.mcolumn_instrumentMethod
        public string InstrumentMethod
        {
            get { return Sample.InstrumentData.MethodName; }
            set { Sample.InstrumentData.MethodName = value; }
        }

        // controlSampleView.mcolumn_datasetType
        public string DatasetType
        {
            get { return Sample.DmsData.DatasetType; }
            set { Sample.DmsData.DatasetType = value; }
        }

        // controlSampleView.mcolumn_batchID
        public int BatchID => Sample.DmsData.Batch;
    }
}
