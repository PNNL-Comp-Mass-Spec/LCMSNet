using System;
using System.Collections.Generic;

namespace LcmsNetSDK.Method
{
    public class LCMethodManager
    {
        public static LCMethodManager Manager { get; } = new LCMethodManager();

        private LCMethodManager()
        {

        }

        #region Events

        /// <summary>
        /// Fired when a LC method is added.
        /// </summary>
        public event EventHandler<LCMethod> MethodAdded;

        /// <summary>
        /// Fired when a LC method is removed.
        /// </summary>
        public event EventHandler<LCMethod> MethodRemoved;

        /// <summary>
        /// Fired when a LC method is updated.
        /// </summary>
        public event EventHandler<LCMethod> MethodUpdated;

        #endregion

        /// <summary>
        /// Gets the list of LC methods available to run.
        /// </summary>
        private readonly Dictionary<string, LCMethod> methods = new Dictionary<string, LCMethod>();

        public IEnumerable<LCMethod> AllLCMethods => methods.Values;

        public LCMethod GetLCMethodByName(string methodName)
        {
            if (TryGetLCMethod(methodName, out var method))
            {
                return method;
            }

            return null;
        }

        public bool TryGetLCMethod(string methodName, out LCMethod method)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                method = null;
                return false;
            }

            return methods.TryGetValue(methodName, out method);
        }

        public bool MethodExists(string methodName)
        {
            return methods.ContainsKey(methodName);
        }

        /// <summary>
        /// Adds a method to the list of methods.
        /// </summary>
        /// <param name="method">Method to add</param>
        /// <returns>True if the method was added.  False if the method already existed.</returns>
        public bool AddOrUpdateMethod(LCMethod method)
        {
            if (method?.Name == null)
                return false;

            if (methods.ContainsKey(method.Name))
            {
                methods[method.Name] = method;
                MethodUpdated?.Invoke(this, method);
                return false;
            }

            methods.Add(method.Name, method);
            MethodAdded?.Invoke(this, method);
            return true;
        }

        /// <summary>
        /// Removes the method from the list of available methods.
        /// </summary>
        /// <param name="methodName">Method to remove.</param>
        /// <returns>True if the method was removed, false if not.</returns>
        public bool RemoveMethod(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return false;
            }

            if (methods.TryGetValue(methodName, out var method))
            {
                methods.Remove(methodName);

                if (!method.Name.Equals(methodName))
                {
                    method = (LCMethod)method.Clone();
                    method.Name = methodName;
                }

                MethodRemoved?.Invoke(this, method);
                return true;
            }

            return false;
        }
    }
}
