using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LcmsNetData
{
    public interface INotifyPropertyChangedExt : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }

    public static class NotifyPropertyChangedExtensions
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
        public static TRet RaiseAndSetIfChanged<TRet>(this INotifyPropertyChangedExt obj,
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
        public static bool RaiseAndSetIfChangedRetBool<TRet>(this INotifyPropertyChangedExt obj,
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

        /// <summary>
        /// If isLocked is false and the newValue is not equal to the backingField value (using default EqualityComparer), sets backingField and raises OnPropertyChanged
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="obj"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="isLocked"></param>
        /// <param name="propertyName"></param>
        /// <returns>final value of backingField</returns>
        public static TRet RaiseAndSetIfChangedLockCheck<TRet>(this INotifyPropertyChangedExt obj,
            ref TRet backingField, TRet newValue, bool isLocked, [CallerMemberName] string propertyName = null)
        {
            if (isLocked)
            {
                obj.OnPropertyChanged(propertyName);
                return backingField;
            }

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
        /// If isLocked is false and the newValue is not equal to the backingField value (using default EqualityComparer), sets backingField and raises OnPropertyChanged
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="obj"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="isLocked"></param>
        /// <param name="propertyName"></param>
        /// <returns>true if changed, false if not</returns>
        public static bool RaiseAndSetIfChangedLockCheckRetBool<TRet>(this INotifyPropertyChangedExt obj,
            ref TRet backingField, TRet newValue, bool isLocked, [CallerMemberName] string propertyName = null)
        {
            if (isLocked)
            {
                obj.OnPropertyChanged(propertyName);
                return false;
            }

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

        /// <summary>
        /// Raise the PropertyChanged event for the given/current property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public static void RaisePropertyChanged(this INotifyPropertyChangedExt obj,
            [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            obj.OnPropertyChanged(propertyName);
        }
    }
}
