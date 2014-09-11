using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Class that encapsulates all trackable diagnostic data for the cart.
    /// </summary>
    public class classMonitoringComponent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public classMonitoringComponent()
        {
            Status          = "";
            PlotData        = new List<classMonitoringMeasurementPlot>();
            MeasurementData = new List<classMonitoringMeasurementScalar>();
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the model of the component.
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the description of the device.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the list of diagnostic data.
        /// </summary>
        public List<classMonitoringMeasurementScalar> MeasurementData{get;set;}
        /// <summary>
        /// Represents plot data.
        /// </summary>
        public List<classMonitoringMeasurementPlot> PlotData{get;set;}       
    }
    public class classMonitoringMeasurementScalar: classMonitoringMeasurement
    {
        /// <summary>
        /// Gets or sets the unit type.
        /// </summary>
        public string Units { get; set; }
        /// <summary>
        /// Gets or sets the value of the measurement.
        /// </summary>
        public string Value { get; set; }
    }
    /// <summary>
    /// Represents a collection of data.
    /// </summary>
    public class classMonitoringMeasurementPlot: classMonitoringMeasurement
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public classMonitoringMeasurementPlot()
        {
            XValues = new List<string>();
            YValues = new List<string>();
        }
        /// <summary>
        /// Gets or sets the X-unit type.
        /// </summary>
        public string XUnits { get; set; }
        /// <summary>
        /// Gets or sets the Y-unit type.
        /// </summary>
        public string YUnits { get; set; }
        /// <summary>
        /// Gets or sets a list of x-values.
        /// </summary>
        public List<string> XValues {get;set;}
        /// <summary>
        /// Gets or sets a list of y-values.
        /// </summary>
        public List<string> YValues {get;set;}

        public void SetY(List<double> values)
        {
            foreach (double value in values)
            {
                YValues.Add(value.ToString(".000"));
            }
        }
        public void SetY(List<DateTime> values)
        {
            foreach (DateTime value in values)
            {
                YValues.Add(value.ToString());
            }
        }
        public void SetX(List<double> values)
        {
            foreach (double value in values)
            {
                XValues.Add(value.ToString(".000"));
            }
        }
        public void SetX(List<DateTime> values)
        {
            foreach (DateTime value in values)
            {
                XValues.Add(value.ToString());
            }
        }
    }

    /// <summary>
    /// Represents the base of measurement data.
    /// </summary>
    public abstract class classMonitoringMeasurement
    {
        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public string Type {get; set;}
        /// <summary>
        /// Gets or sets the name of measurement.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description of the measurement.
        /// </summary>
        public string Description { get; set; }        
    }
}
