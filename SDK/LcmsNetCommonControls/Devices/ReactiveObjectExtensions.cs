using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace LcmsNetCommonControls.Devices
{
    /// <summary>
    /// Add an additional extension method for IReactiveObject NotifyPropertyChanged
    /// </summary>
    public static class ReactiveObjectExtensions
    {
        /// <summary>
        /// If the newValue is not equal to the backingField value (using default EqualityComparer), sets backingField and raises OnPropertyChanged
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="obj"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns>true if changed, false if not</returns>
        public static bool RaiseAndSetIfChangedRetBool<TRet>(this IReactiveObject obj,
            ref TRet backingField, TRet newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return false;
            }

            backingField = newValue;
            obj.RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
