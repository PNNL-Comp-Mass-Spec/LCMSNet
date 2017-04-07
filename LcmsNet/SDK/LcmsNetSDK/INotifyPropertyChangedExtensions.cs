using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LcmsNetSDK
{
    public interface INotifyPropertyChangedExt : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName] string propertyName = "");
    }

    public static class NotifyPropertyChangedExtensions
    {
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
    }
}
