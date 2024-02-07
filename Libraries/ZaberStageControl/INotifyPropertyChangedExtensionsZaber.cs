using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZaberStageControl
{
    public interface INotifyPropertyChangedExtZaber : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }

    internal static class NotifyPropertyChangedExtensionsZaber
    {
        /// <summary>
        /// If the newValue is not equal to the backingField value (using default EqualityComparer), sets backingField and raises OnPropertyChanged
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="obj"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns>final value of backingField</returns>
        public static TRet RaiseAndSetIfChanged<TRet>(this INotifyPropertyChangedExtZaber obj,
            ref TRet backingField, TRet newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }

            backingField = newValue;
            obj.OnPropertyChanged(propertyName);
            return newValue;
        }

        /// <summary>
        /// If the newValue is not equal to the backingField value (using default EqualityComparer), sets backingField and raises OnPropertyChanged
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="obj"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns>true if changed, false if not</returns>
        public static bool RaiseAndSetIfChangedRetBool<TRet>(this INotifyPropertyChangedExtZaber obj,
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
            obj.OnPropertyChanged(propertyName);
            return true;
        }
    }
}
