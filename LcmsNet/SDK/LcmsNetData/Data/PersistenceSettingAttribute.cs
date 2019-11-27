using System;

namespace LcmsNetData.Data
{
    /// <summary>
    /// Settings for object persistence (generally to the SQLite database)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PersistenceSettingAttribute : Attribute
    {
        /// <summary>
        /// If the property should not be persisted or loaded
        /// </summary>
        public bool IgnoreProperty { get; set; }

        /// <summary>
        /// The Column Name for this property in the database. If blank, the property name will be used.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// For LCMSNet objects: The prefix used for column names for properties in the object
        /// </summary>
        public string ColumnNamePrefix { get; set; }

        /// <summary>
        /// For special read handling: Supply the name of the method to use for reading the value (preferably using the "nameof(...)" compiler method). Needs to have a unique name, no parameters, and return the same type as the property.
        /// </summary>
        public string PropertyGetOverrideMethod { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PersistenceSettingAttribute()
        {
            IgnoreProperty = false;
            ColumnName = null;
            ColumnNamePrefix = null;
            PropertyGetOverrideMethod = null;
        }

        /// <summary>
        /// Constructor to simply set a prefix
        /// </summary>
        /// <param name="prefix"></param>
        public PersistenceSettingAttribute(string prefix) : this()
        {
            ColumnNamePrefix = prefix;
        }
    }
}
