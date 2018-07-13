using System;

namespace LcmsNetData.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotStoredPropertyAttribute : Attribute { }
}
