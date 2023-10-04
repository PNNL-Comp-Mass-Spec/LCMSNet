using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using LcmsNetSDK;
using LcmsNetSDK.Data;

namespace LcmsNet.Data
{
    /// <summary>
    /// Class that manages all of the information about a given column
    /// </summary>
    [Serializable]
    public class ColumnData : IColumn, INotifyPropertyChangedExt, IEquatable<ColumnData>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnData(int id, string name = "", ColumnStatus status = ColumnStatus.Idle, Color color = default(Color))
        {
            Name = name;
            ID = id;
            Status = status;
            Color = color;
        }

        public override string ToString()
        {
            return $"ID = {ID} Name = {Name}";
        }

        /// <summary>
        /// Name of the column
        /// </summary>
        private string name;

        /// <summary>
        /// Status of the column
        /// </summary>
        private ColumnStatus status;

        /// <summary>
        /// Color of the column.
        /// </summary>
        [field: NonSerialized]
        private Color color;

        /// <summary>
        /// Handles serialization of the column color.
        /// </summary>
        private string colorString;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            color = Colors.Transparent;
            if (ColorConverter.ConvertFromString(colorString) is Color c)
            {
                color = c;
            }
        }

        [OnSerializing]
        private void OnSerialize(StreamingContext context)
        {
            colorString = TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(color);
        }

        /// <summary>
        /// Gets or sets when the status changes.
        /// </summary>
        public ColumnStatus Status
        {
            get => status;
            set
            {
                var previousStatus = status;
                if (this.RaiseAndSetIfChangedRetBool(ref status, value) && value != ColumnStatus.Running && previousStatus != ColumnStatus.Running)
                {
                    // Update the program setting if changed
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_COLUMNDISABLED + ID, (status == ColumnStatus.Disabled).ToString());
                }
            }
        }

        public string StatusString => Status.ToString();

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(name))
                    return name;

                return (ID + 1).ToString();
            }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref name, value, nameof(Name)))
                {
                    // Update the program setting if changed
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_COLUMNNAME + ID, Name);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the column.
        /// </summary>
        public Color Color
        {
            get => color;
            set => this.RaiseAndSetIfChanged(ref color, value, nameof(Color));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(ColumnData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID == other.ID && string.Equals(name, other.name) && color.Equals(other.color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ColumnData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ID;
                hashCode = (hashCode * 397) ^ (name != null ? name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ color.GetHashCode();
                return hashCode;
            }
        }
    }
}
