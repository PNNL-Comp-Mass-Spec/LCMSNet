using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace LcmsNetSDK.Data
{
    /// <summary>
    /// Class that manages all of the information about a given column
    /// </summary>
    [Serializable]
    public class ColumnData : INotifyPropertyChangedExt, IEquatable<ColumnData>, ICloneable
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnData()
        {
            m_name = "NOTSET";
            m_columnIndex = 0;
            m_status = ColumnStatus.Idle;
            m_first = false;
        }

        /// <summary>
        /// Clone - get a deep copy
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var newColumnData = new ColumnData
            {
                First = First,
                Status = Status,
                ID = ID,
                Name = Name,
                Color = Color
            };


            return newColumnData;
        }

        #endregion

        public override string ToString()
        {
            return $"ID = {ID} Name = {Name}";
        }

        #region Delegate Definitions

        public delegate void DelegateStatusChanged(
            object sender, ColumnStatus previousStatus, ColumnStatus newStatus);

        /// <summary>
        /// Delegate definition called if the column first value is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="first"></param>
        public delegate void DelegateFirstChanged(object sender, bool first);

        public delegate void DelegateColorChanged(object sender, Color previousColor, Color newColor);

        public delegate void DelegateNameChanged(object sender, string name, string oldName);

        #endregion

        #region Members

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
        /// Fired if the first value of this column is changed.
        /// </summary>
        [field: NonSerialized]
        public event DelegateFirstChanged FirstChanged;

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

        /// <summary>
        /// Flag indicating if this is the first column to run.
        /// </summary>
        private bool m_first;

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if the column is the first column designated to run.
        /// </summary>
        public bool First
        {
            get => m_first;
            set
            {
                m_first = value;
                FirstChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets or sets when the status changes.
        /// </summary>
        public ColumnStatus Status
        {
            get => m_status;
            set
            {
                //
                // If the status has changed,
                // let someone know.
                //
                var previousStatus = m_status;
                if (this.RaiseAndSetIfChangedRetBool(ref m_status, value, nameof(Status)))
                {
                    StatusChanged?.Invoke(this, previousStatus, m_status);
                }
            }
        }

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
            get => m_name;
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

        #endregion

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