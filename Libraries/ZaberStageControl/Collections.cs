using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Zaber.Motion;

namespace ZaberStageControl
{
    public static class Collections
    {
        public static ReadOnlyObservableCollection<KeyValuePair<string, Units>> DistanceUnits { get; }
        public static ReadOnlyObservableCollection<KeyValuePair<string, Units>> VelocityUnits { get; }
        public static ReadOnlyObservableCollection<KeyValuePair<string, Units>> AccelerationUnits { get; }

        static Collections()
        {
            var units = Enum.GetValues(typeof(Units)).Cast<Units>().ToList();
            DistanceUnits = CreateCollection(units.Where(x => x == Units.Native || x.ToString().StartsWith("Length")), "Length_");
            VelocityUnits = CreateCollection(units.Where(x => x == Units.Native || x.ToString().StartsWith("Velocity")), "Velocity_");
            AccelerationUnits = CreateCollection(units.Where(x => x == Units.Native || x.ToString().StartsWith("Length")), "Acceleration_");
        }

        private static ReadOnlyObservableCollection<KeyValuePair<string, Units>> CreateCollection(IEnumerable<Units> units, string trimPrefix)
        {
            return new ReadOnlyObservableCollection<KeyValuePair<string, Units>>(new ObservableCollection<KeyValuePair<string, Units>>(CreateLabels(units, trimPrefix)));
        }

        private static List<KeyValuePair<string, Units>> CreateLabels(IEnumerable<Units> units, string startTrim)
        {
            var labeled = new List<KeyValuePair<string, Units>>();

            foreach (var unit in units)
            {
                labeled.Add(new KeyValuePair<string, Units>(CreateLabel(unit, startTrim), unit));
            }

            return labeled;
        }

        private static readonly IReadOnlyList<KeyValuePair<string, string>> Replacements = new List<KeyValuePair<string, string>>()
        {
            // In order of replacement priority - order does matter for some!
            // Length units
            // ReSharper disable StringLiteralTypo
            new KeyValuePair<string, string>("centimetres", "cm"),
            new KeyValuePair<string, string>("millimetres", "mm"),
            new KeyValuePair<string, string>("micrometres", "\u00B5m"), // µm
            new KeyValuePair<string, string>("nanometres", "nm"),
            new KeyValuePair<string, string>("metres", "m"),
            new KeyValuePair<string, string>("inches", "in"),

            // Acceleration and velocity additions
            new KeyValuePair<string, string>("persecondsquared", "/s\u00B2"), // /s²
            new KeyValuePair<string, string>("persecond", "/s"), // /s²
            // ReSharper restore StringLiteralTypo
        };

        private static string CreateLabel(Units unit, string startTrim)
        {
            var label = unit.ToString();

            if (unit == Units.Native)
            {
                return label;
            }

            if (label.StartsWith(startTrim, StringComparison.OrdinalIgnoreCase))
            {
                label = label.Substring(startTrim.Length).Trim('_');
            }

            foreach (var replacement in Replacements)
            {
                label = Regex.Replace(label, replacement.Key, replacement.Value, RegexOptions.IgnoreCase);
            }

            return label;
        }
    }
}
