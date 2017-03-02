using System.Windows.Forms;
using System.Collections.Generic;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that pairs parameter values with controls and their names for editing them.
    /// </summary>
    public class classLCMethodEventParameter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public classLCMethodEventParameter()
        {
            Values = new List<object>();
            Controls = new List<Control>();
            Names = new List<string>();
            DataProviderNames = new List<string>();
        }

        /// <summary>
        /// Gets the names of each parameter.
        /// </summary>
        public List<string> Names { get; }

        /// <summary>
        /// Gets the values to use with the method. 
        /// </summary>
        public List<object> Values { get; }

        /// <summary>
        /// Gets the list of controls to edit the values.
        /// </summary>
        public List<Control> Controls { get; }

        /// <summary>
        /// List of data provider names.
        /// </summary>
        public List<string> DataProviderNames { get; set; }

        /// <summary>
        /// Adds a parameter value to the list of parameters and it's associated editing control.
        /// </summary>
        /// <param name="parameter">Parameter value to add.</param>
        /// <param name="editControl">Edit control to use.</param>
        /// <param name="name"></param>
        /// <param name="dataProviderName">Name of the data provider to use.</param>
        public void AddParameter(object parameter, Control editControl, string name, string dataProviderName)
        {
            // 
            // We use the private set.  
            // Private so that the lists are only gauranteed to be synchronized.                
            // 
            Values.Add(parameter);
            Controls.Add(editControl);
            Names.Add(name);
            DataProviderNames.Add(dataProviderName);
        }

        public override string ToString()
        {
            var data = "";

            foreach (var value in Values)
            {
                if (value != null)
                {
                    data += value + ", ";
                }
                else
                {
                    data += ", -Not Set -, ";
                }
            }
            data = data.TrimEnd(',', ' ');
            return data;
        }
    }
}