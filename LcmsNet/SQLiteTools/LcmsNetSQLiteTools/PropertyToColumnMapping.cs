using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using LcmsNetData.Data;

namespace LcmsNetSQLiteTools
{
    internal class PropertyToColumnMapping
    {
        // Ignore Spelling: nullable

        private readonly Dictionary<Type, Dictionary<string, PropertyColumnMapping>> propertyColumnMappings = new Dictionary<Type, Dictionary<string, PropertyColumnMapping>>();
        private readonly Dictionary<Type, Dictionary<string, string>> propertyColumnNameMappings = new Dictionary<Type, Dictionary<string, string>>();

        internal Dictionary<string, string> GetPropertyColumnNameMapping(Type type)
        {
            if (propertyColumnNameMappings.TryGetValue(type, out var stored))
            {
                return stored;
            }

            GetPropertyColumnMapping(type);
            return propertyColumnNameMappings[type];
        }

        internal Dictionary<string, PropertyColumnMapping> GetPropertyColumnMapping(Type type)
        {
            if (propertyColumnMappings.TryGetValue(type, out var mappings))
            {
                return mappings;
            }

            mappings = new Dictionary<string, PropertyColumnMapping>();
            var nameMapping = new Dictionary<string, string>();

            foreach (var property in type.GetProperties())
            {
                var settings = (PersistenceSettingAttribute)Attribute.GetCustomAttribute(property, typeof(PersistenceSettingAttribute)) ?? new PersistenceSettingAttribute();
                if (settings.IgnoreProperty)
                {
                    continue;
                }

                // Test to make sure the property has both get and set accessors
                // TODO: Should not be necessary for non-primitive-type properties, as long as they are otherwise initialized appropriately.
                // TODO: Requirement: if an object, then "CanWrite" can be false as long as there is a default constructor for the object type.
                if (!(property.CanRead && property.CanWrite))
                {
                    throw new NotSupportedException(
                        "Operation requires get and set accessors for all persisted properties. " +
                        "Add the attribute '[PersistenceSetting(IgnoreProperty = true)]' to ignore the failing property. Property info: Class '" +
                        type.FullName + "', property '" + property.Name + "'.");
                }

                if (string.IsNullOrWhiteSpace(settings.ColumnName))
                {
                    settings.ColumnName = property.Name;
                }

                if (string.IsNullOrWhiteSpace(settings.ColumnNamePrefix))
                {
                    settings.ColumnNamePrefix = settings.ColumnName;
                }

                if (!string.IsNullOrWhiteSpace(settings.PropertyGetOverrideMethod))
                {
                    // Special read method: resolve the method, and generate the mappings
                    try
                    {
                        var methodInfo = type.GetMethod(settings.PropertyGetOverrideMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (methodInfo == null)
                        {
                            throw new NotSupportedException($"Method named \"{settings.PropertyGetOverrideMethod}\" not found in type \"{type.FullName ?? type.Name}\"!");
                        }

                        if (methodInfo.GetParameters().Length > 0 && methodInfo.GetParameters().Any(x => !x.HasDefaultValue))
                        {
                            throw new NotSupportedException($"Method named \"{settings.PropertyGetOverrideMethod}\" in type \"{type.FullName ?? type.Name}\" has required arguments, and cannot be used for object persistence!");
                        }

                        var propMappings = GetPropertyColumnMapping(type, property, settings, x => methodInfo.Invoke(x, null));
                        foreach (var mapping in propMappings)
                        {
                            mappings.Add(mapping.ColumnName, mapping);
                            nameMapping.Add(mapping.ColumnName.ToLower(), mapping.ColumnName);
                        }
                    }
                    catch (AmbiguousMatchException)
                    {
                        throw new NotSupportedException($"Multiple matches found in class \"{type.FullName ?? type.Name}\" for method name \"{settings.PropertyGetOverrideMethod}\". Method name must be unique!");
                    }
                }
                else
                {
                    var propMappings = GetPropertyColumnMapping(type, property, settings);
                    foreach (var mapping in propMappings)
                    {
                        mappings.Add(mapping.ColumnName, mapping);
                        nameMapping.Add(mapping.ColumnName.ToLower(), mapping.ColumnName);
                    }
                }
            }

            propertyColumnMappings.Add(type, mappings);
            propertyColumnNameMappings.Add(type, nameMapping);
            return mappings;
        }

        private IEnumerable<PropertyColumnMapping> GetPropertyColumnMapping(Type type, PropertyInfo property, PersistenceSettingAttribute settings, Func<object, object> readMethod = null)
        {
            var propType = property.PropertyType;
            if (propType.IsValueType || propType.IsEnum || propType == typeof(string))
            {
                // Built-in direct handling: read or assign, with some error checking
                yield return new PropertyColumnMapping(settings.ColumnName, propType, settings.IsUniqueColumn, x =>
                {
                    if (x.GetType() != type)
                    {
                        // return the default value for the type
                        //return Activator.CreateInstance(propType);
                        throw new NotSupportedException($"Cannot access property \"{property.Name}\" on object of type \"{x.GetType().FullName ?? x.GetType().Name}\"");
                    }

                    if (readMethod != null)
                    {
                        return readMethod(x);
                    }

                    return property.GetValue(x);
                }, (cls, propValue) =>
                {
                    if (cls.GetType() != type)
                    {
                        // return the default value for the type
                        //return Activator.CreateInstance(propType);
                        throw new NotSupportedException($"Cannot access property \"{property.Name}\" on object of type \"{cls.GetType().FullName ?? cls.GetType().Name}\"");
                    }

                    if (propValue.GetType() == propType)
                    {
                        property.SetValue(cls, propValue);
                    }
                    else
                    {
                        try
                        {
                            var value = ConvertToType(propValue, propType);
                            property.SetValue(cls, value);
                        }
                        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                        {
                            throw new NotSupportedException($"Could not convert value of type \"{propValue.GetType().Name}\" with value \"{propValue}\" to target type \"{propType.Name}\"; type \"{cls.GetType().FullName ?? cls.GetType().Name}\", column name \"{settings.ColumnName}\"");
                        }
                    }
                });
                yield break;
            }

            if (propType.IsClass && propType.FullName != null && propType.FullName.StartsWith("LCMSNet", StringComparison.OrdinalIgnoreCase))
            {
                // LCMSNet class: generate the mapping, with cascading through sub-objects
                var objectMappings = GetPropertyColumnMapping(propType);
                foreach (var mapping in objectMappings)
                {
                    yield return new PropertyColumnMapping($"{settings.ColumnNamePrefix}{mapping.Key}", mapping.Value.PropertyType, mapping.Value.IsUniqueColumn, x =>
                    {
                        if (x.GetType() != type)
                        {
                            // return the default value for the type
                            //return Activator.CreateInstance(propType);
                            throw new NotSupportedException($"Cannot access property {property.Name} on object of type {x.GetType()}");
                        }

                        if (readMethod != null)
                        {
                            return mapping.Value.ReadProperty(readMethod(x));
                        }

                        return mapping.Value.ReadProperty(property.GetValue(x));
                    }, (cls, propValue) =>
                    {
                        if (cls.GetType() != type)
                        {
                            // return the default value for the type
                            //return Activator.CreateInstance(propType);
                            throw new NotSupportedException($"Cannot access property \"{property.Name}\" on object of type \"{cls.GetType().FullName ?? cls.GetType().Name}\"");
                        }

                        var subObject = property.GetValue(cls);
                        if (subObject == null)
                        {
                            try
                            {
                                subObject = Activator.CreateInstance(propType);
                                property.SetValue(cls, subObject);
                            }
                            catch (Exception e)
                            {
                                throw new NotSupportedException($"Could not set a value for property \"{property.Name}\" in class \"{cls.GetType().FullName ?? cls.GetType().Name}\", of type \"{propType}\": {e}");
                            }
                        }

                        var targetType = mapping.Value.PropertyType;
                        if (propValue.GetType() == targetType)
                        {
                            mapping.Value.SetProperty(subObject, propValue);
                        }
                        else
                        {
                            try
                            {
                                var value = ConvertToType(propValue, targetType);
                                mapping.Value.SetProperty(subObject, value);
                            }
                            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                            {
                                throw new NotSupportedException($"Could not convert value of type \"{propValue.GetType().Name}\" with value \"{propValue}\" to target type \"{propType.Name}\"; type \"{cls.GetType().FullName ?? cls.GetType().Name}\", column name \"{settings.ColumnName}\"");
                            }
                        }
                    });
                }
            }
        }

        private object ConvertToType(object value, Type targetType)
        {
            if (value == null || value is DBNull)
            {
                if (targetType.IsValueType || targetType.IsEnum || targetType.IsValueType)
                {
                    // return the default value for the type
                    return Activator.CreateInstance(targetType);
                }

                return null;
            }
            if (value.GetType() == targetType)
            {
                return value;
            }
            if (targetType == typeof(string))
            {
                return value;
            }

            if (targetType.IsEnum && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                return Enum.Parse(targetType, value.ToString());
            }
            if (targetType.IsPrimitive)
            {
                return Convert.ChangeType(value, targetType);
            }
            if (targetType == typeof(DateTime))
            {
                var tempValue = (DateTime)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return TimeZoneInfo.ConvertTimeToUtc(tempValue);
            }
            if (targetType == typeof(Color))
            {
                var convertFromString = TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(value.ToString());
                if (convertFromString != null)
                {
                    return (Color)convertFromString;
                }

                return new Color();
            }
            if (targetType.ToString().StartsWith("System.Nullable"))
            {
                var wrappedType = targetType.GenericTypeArguments[0];
                if (string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return null;
                }

                // We're dealing with nullable types here, and the default
                // value for those is null, so we shouldn't have to set the
                // value to null if parsing doesn't work.
                try
                {
                    return ConvertToType(value, wrappedType);
                }
                catch (InvalidCastException) { }
                catch (FormatException) { }
                catch (OverflowException) { }

                return null;
            }

            throw new NotSupportedException("LcmsNetSQLiteTools.SQLiteTools.ConvertToType(), Invalid property type specified: " + targetType);
        }

        internal class PropertyColumnMapping
        {
            /// <summary>
            /// SQLite table column name
            /// </summary>
            public string ColumnName { get; }

            /// <summary>
            /// The type of the property, for conversion handling
            /// </summary>
            public Type PropertyType { get; }

            /// <summary>
            /// Method for reading the property
            /// </summary>
            public Func<object, object> ReadProperty { get; }

            /// <summary>
            /// Method for setting the property
            /// </summary>
            public Action<object, object> SetProperty { get; }

            /// <summary>
            /// Is property is marked as a column with unique values
            /// </summary>
            public bool IsUniqueColumn { get; }

            /// <summary>
            /// True if the property type is string and the property is not marked as a unique value
            /// </summary>
            public bool DoStringDeDuplication => PropertyType == typeof(string) && !IsUniqueColumn;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="columnName">SQLite table column name</param>
            /// <param name="propertyType">Type of the property, for conversion handling</param>
            /// <param name="isUniqueColumn">If the column has a 'unique' constraint, or other reason to believe that duplicates never occur</param>
            /// <param name="readProperty">Method for reading the property</param>
            /// <param name="setProperty">Method for setting the property</param>
            public PropertyColumnMapping(string columnName, Type propertyType, bool isUniqueColumn,
                Func<object, object> readProperty, Action<object, object> setProperty)
            {
                ColumnName = columnName;
                PropertyType = propertyType;
                ReadProperty = readProperty;
                SetProperty = setProperty;
                IsUniqueColumn = isUniqueColumn;
            }
        }
    }
}
