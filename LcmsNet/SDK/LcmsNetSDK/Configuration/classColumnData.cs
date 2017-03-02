using System;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;


namespace LcmsNetDataClasses.Configuration
{
    /// <summary>
    /// Class that manages all of the information about a given column
    /// </summary>
    [Serializable]
    public class classColumnData : classDataClassBase
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classColumnData()
        {
            m_name = "";
            m_columnIndex = 0;
            m_systemIndex = 0;
            menum_status = enumColumnStatus.Idle;
            m_first = false;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("ID = {0} Name = {1}", ID, Name);
        }

        #region Delegate Definitions

        public delegate void DelegateStatusChanged(
            object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus);

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
        /// System index of the column.
        /// </summary>
        private int m_systemIndex;

        /// <summary>
        /// Status of the column
        /// </summary>
        private enumColumnStatus menum_status;

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
        private Color m_columnColor;

        /// <summary>
        /// Flag indicating if this is the first column to run.
        /// </summary>
        private bool m_first;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if the column is the first column designated to run.
        /// </summary>
        public bool First
        {
            get { return m_first; }
            set
            {
                m_first = value;
                if (FirstChanged != null)
                {
                    FirstChanged(this, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets when the status changes.
        /// </summary>
        public enumColumnStatus Status
        {
            get { return menum_status; }
            set
            {
                // 
                // If the status has changed,
                // let someone know.
                // 
                if (value != menum_status)
                {
                    var previousStatus = menum_status;
                    menum_status = value;
                    if (StatusChanged != null)
                        StatusChanged(this, previousStatus, menum_status);
                }
                menum_status = value;
            }
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public int ID
        {
            get { return m_columnIndex; }
            set { m_columnIndex = value; }
        }

        /// <summary>
        /// Gets or sets the system index.
        /// </summary>
        public int SystemID
        {
            get { return m_systemIndex; }
            set { m_systemIndex = value; }
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                if (value != m_name)
                {
                    var oldName = m_name;

                    m_name = value;
                    if (NameChanged != null)
                    {
                        NameChanged(this, m_name, oldName);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the column.
        /// </summary>
        public Color Color
        {
            get { return m_columnColor; }
            set
            {
                if (m_columnColor != value && value != null)
                {
                    if (ColorChanged != null)
                        ColorChanged(this, m_columnColor, value);
                }
                m_columnColor = value;
            }
        }

        #endregion
    }
}