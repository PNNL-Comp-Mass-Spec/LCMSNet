using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LcmsNetSDK
{
#if DotNET4
    public interface INotifyPropertyChangedExt : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }

#else
    public interface INotifyPropertyChangedExt : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName] string propertyName = "");
    }
#endif

    public static class NotifyPropertyChangedExtensions
    {
#if DotNET4
        public static TRet RaiseAndSetIfChanged<TRet>(this INotifyPropertyChangedExt obj,
            ref TRet backingField, TRet newValue, string propertyName = null)
#else
                    public static TRet RaiseAndSetIfChanged<TRet>(this INotifyPropertyChangedExt obj,
            ref TRet backingField, TRet newValue, [CallerMemberName] string propertyName = null)
#endif
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

#if DotNET4
        public static bool RaiseAndSetIfChangedRetBool<TRet>(this INotifyPropertyChangedExt obj,
            ref TRet backingField, TRet newValue, string propertyName = null)
#else
        public static bool RaiseAndSetIfChangedRetBool<TRet>(this INotifyPropertyChangedExt obj,
            ref TRet backingField, TRet newValue, [CallerMemberName] string propertyName = null)
#endif
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
