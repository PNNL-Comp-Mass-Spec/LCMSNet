//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche, Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
// - 03/16/2009 (BLL) - Added LC and PAL Methods
// - 12/01/2009 (DAC) - Modified to accomodate change of vial from string to int
// - 9/26/2014  (CJW) - Modified to use MEF for DMS and sample validations
// - 9/30/2014  (CJW) - bug fixes
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LcmsNet.Method;
using LcmsNet.Method.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetDmsTools;
using LcmsNetSDK;
using LcmsNetSQLiteTools;

namespace LcmsNet.SampleQueue.Forms
{
    /// <summary>
    /// Control that displays a sample list.
    /// </summary>
    public partial class controlSampleView2 : UserControl
    {
       
        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public controlSampleView2(formDMSView dmsView, classSampleQueue sampleQueue)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                InitializeComponent();
                Initialize(dmsView, sampleQueue);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor 1: " + ex.Message, ex);
            }
            finally
            {
                // Old mdataGrid_samples.RowHeadersVisible = false;
            }
        }

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        public controlSampleView2()
        {

            try
            {
                InitializeComponent();
                Initialize(null, null);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor 2: " + ex.Message, ex);
            }
           
        }

        /// <summary>
        /// Performs initialization for the constructors.
        /// </summary>
        /// <param name="dmsView"></param>
        /// <param name="sampleQueue"></param>
        private void Initialize(formDMSView dmsView, classSampleQueue sampleQueue)
        {
            ;
        }
        
    }

   
}
