using System;
using System.Collections.Generic;
using System.IO;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method
{
    /// <summary>
    /// Definition for when a method is updated.
    /// </summary>
    /// <param name="sender">Object who updated the method.</param>
    /// <param name="method">Method that was updated.</param>
    public delegate bool DelegateMethodUpdated(object sender, classLCMethod method);

    /// <summary>
    /// Class that manages all of the LC Methods.
    /// </summary>
    public class classLCMethodManager
    {
        #region constants

        private const string CONST_METHOD_EXTENSION = "*.xml";

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        private classLCMethodManager()
        {
            m_methods = new Dictionary<string, classLCMethod>();
        }

        #region Events

        /// <summary>
        /// Fired when a LC method is added.
        /// </summary>
        public event DelegateMethodUpdated MethodAdded;

        /// <summary>
        /// Fired when a LC method is removed.
        /// </summary>
        public event DelegateMethodUpdated MethodRemoved;

        /// <summary>
        /// Fired when a LC method is updated.
        /// </summary>
        public event DelegateMethodUpdated MethodUpdated;

        #endregion

        #region Members

        /// <summary>
        /// List of available methods.
        /// </summary>
        private readonly Dictionary<string, classLCMethod> m_methods;

        /// <summary>
        /// Static object that manages each LC method to be accessible to other objects.
        /// </summary>
        private static classLCMethodManager m_manager;

        #endregion

        #region Methods

        /// <summary>
        /// Adds a method to the list of methods.
        /// </summary>
        /// <param name="method">Method to add</param>
        /// <returns>True if the method was added.  False if the method already existed.</returns>
        public bool AddMethod(classLCMethod method)
        {
            if (method?.Name == null)
                return false;

            if (m_methods.ContainsKey(method.Name) == false)
            {
                m_methods.Add(method.Name, method);
                MethodAdded?.Invoke(this, method);

                return true;
            }
            m_methods[method.Name] = method;

            MethodUpdated?.Invoke(this, method);

            return false;
        }

        /// <summary>
        /// Gets the list of LC methods available to run.
        /// </summary>
        public Dictionary<string, classLCMethod> Methods => m_methods;

        /// <summary>
        /// Removes the method from the list of available methods.
        /// </summary>
        /// <param name="method">Method to remove.</param>
        /// <returns>True if the method was removed, false if not.</returns>
        public bool RemoveMethod(classLCMethod method)
        {
            var result = true;

            //
            // Don't remove anything unless the method
            // name and method are not null.
            //
            if (method?.Name == null)
                return false;

            //
            // Make sure something is not currently using the method
            //
            if (MethodRemoved != null)
            {
                result = MethodRemoved(this, method);
            }
            if (result)
                m_methods.Remove(method.Name);

            return result;
        }

        /// <summary>
        /// Gets or sets the static manager class for the application.
        /// </summary>
        public static classLCMethodManager Manager
        {
            get
            {
                if (m_manager == null)
                {
                    m_manager = new classLCMethodManager();
                }
                return m_manager;
            }
        }

        #endregion

        #region Load/Save

        /// <summary>
        /// Loads a method from the path provided.
        /// </summary>
        /// <param name="filePath">Path to load method from</param>
        /// <param name="errors"></param>
        private void LoadMethod(string filePath, ref List<Exception> errors)
        {
            //bool retValue = false;

            var reader = new classLCMethodReader();
            classLCMethod method;
            try
            {
                method = reader.ReadMethod(filePath, errors);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not load method from " + filePath, ex);
                throw;
            }

            //
            // If the method failed to load...then...return false
            //
            if (method == null)
                throw new Exception("The method was not able to be read.");

            // Figure out if the method exists.
            if (m_methods.ContainsKey(method.Name))
            {
                //TODO: Figure out what to do if a duplicate method exists.
                var errorMessage = string.Format("The user method name from {0} conflicts with another method.",
                    filePath);
                classApplicationLogger.LogMessage(0, errorMessage);
                throw new Exception(errorMessage);
            }
            //
            // Otherwise, add the method so it can be registered with appropiate objects
            //
            AddMethod(method);
        }

        /// <summary>
        /// Loads methods stored in path.  Top-level directory only.
        /// </summary>
        /// <param name="path">Path to load methods from.</param>
        /// <returns>True if successful</returns>
        public Dictionary<string, List<Exception>> LoadMethods(string path)
        {
            var errors = new Dictionary<string, List<Exception>>();

            //
            // Find each file in the directory
            //
            var filePaths = Directory.GetFiles(path, CONST_METHOD_EXTENSION, SearchOption.TopDirectoryOnly);
            foreach (var filePath in filePaths)
            {
                try
                {
                    var methodErrors = new List<Exception>();
                    LoadMethod(filePath, ref methodErrors);

                    if (methodErrors.Count > 0)
                    {
                        errors.Add(filePath, methodErrors);
                    }
                }
                catch (Exception exception)
                {
                    errors.Add(filePath, new List<Exception> {exception});

                    classApplicationLogger.LogError(0,
                        "An unhandled exception occured when reading a user method.",
                        exception);
                }
            }
            return errors;
        }

        #endregion
    }
}