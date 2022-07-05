﻿using System;
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
    public class ColumnData : IColumn, INotifyPropertyChangedExt, IEquatable<ColumnData>, ICloneable
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnData()
        {
            m_name = "";
            m_columnIndex = 0;
            m_status = ColumnStatus.Idle;
        }

        /// <summary>
        /// Clone - get a deep copy
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var newColumnData = new ColumnData
            {
                Status = Status,
                ID = ID,
                Name = Name,
                Color = Color
            };


            return newColumnData;
        }

        public override string ToString()
        {
            return $"ID = {ID} Name = {Name}";
        }

        public delegate void DelegateStatusChanged(
            object sender, ColumnStatus previousStatus, ColumnStatus newStatus);

        public delegate void DelegateColorChanged(object sender, Color previousColor, Color newColor);

        public delegate void DelegateNameChanged(object sender, string name, string oldName);

        /// <summary>
        /// Index of the column.
        /// </summary>
        private int m_columnIndex;

        /// <summary>
        /// Name of the column
        /// </summary>
        private string m_name;

        /// <summary>
        /// Status of the column
        /// </summary>
        private ColumnStatus m_status;

        /// <summary>
        /// Fired when the status of a column changes.
        /// </summary>
        [field: NonSerialized]
        public event DelegateStatusChanged StatusChanged;

        /// <summary>
        /// Fired when the color of the column changes.
        /// </summary>
        [field: NonSerialized]
        public event DelegateColorChanged ColorChanged;

        /// <summary>
        /// An event that indicates the name of the column has changed.
        /// </summary>
        [field: NonSerialized]
        public event DelegateNameChanged NameChanged;

        /// <summary>
        /// Color of the column.
        /// </summary>
        [field: NonSerialized]
        private Color m_columnColor;

        /// <summary>
        /// Handles serialization of the column color.
        /// </summary>
        private string colorString;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            m_columnColor = Colors.Transparent;
            if (ColorConverter.ConvertFromString(colorString) is Color c)
            {
                m_columnColor = c;
            }
        }

        [OnSerializing]
        private void OnSerialize(StreamingContext context)
        {
            colorString = TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(m_columnColor);
        }

        /// <summary>
        /// Gets or sets when the status changes.
        /// </summary>
        public ColumnStatus Status
        {
            get => m_status;
            set
            {
                // If the status has changed, let someone know.
                var previousStatus = m_status;
                if (this.RaiseAndSetIfChangedRetBool(ref m_status, value, nameof(Status)))
                {
                    StatusChanged?.Invoke(this, previousStatus, m_status);
                }
            }
        }

        public string StatusString => Status.ToString();

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public int ID
        {
            get => m_columnIndex;
            set => this.RaiseAndSetIfChanged(ref m_columnIndex, value, nameof(ID));
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(m_name))
                    return m_name;

                return (ID + 1).ToString();
            }
            set
            {
                var oldName = m_name;
                if (this.RaiseAndSetIfChangedRetBool(ref m_name, value, nameof(Name)))
                {
                    NameChanged?.Invoke(this, m_name, oldName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the column.
        /// </summary>
        public Color Color
        {
            get => m_columnColor;
            set
            {
                var oldColor = m_columnColor;
                if (this.RaiseAndSetIfChangedRetBool(ref m_columnColor, value, nameof(Color)))
                {
                    ColorChanged?.Invoke(this, oldColor, m_columnColor);
                }
            }
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
            return m_columnIndex == other.m_columnIndex && string.Equals(m_name, other.m_name) && m_columnColor.Equals(other.m_columnColor);
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
                var hashCode = m_columnIndex;
                hashCode = (hashCode * 397) ^ (m_name != null ? m_name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ m_columnColor.GetHashCode();
                return hashCode;
            }
        }
    }
}
