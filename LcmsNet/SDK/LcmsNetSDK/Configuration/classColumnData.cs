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
            mstring_name = "";
            mint_columnIndex = 0;
            mint_systemIndex = 0;
            menum_status = enumColumnStatus.Idle;
            mbool_first = false;
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
        private int mint_columnIndex;

        /// <summary>
        /// Name of the column
        /// </summary>
        private string mstring_name;

        /// <summary>
        /// System index of the column.
        /// </summary>
        private int mint_systemIndex;

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
        private Color mobj_columnColor;

        /// <summary>
        /// Flag indicating if this is the first column to run.
        /// </summary>
        private bool mbool_first;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if the column is the first column designated to run.
        /// </summary>
        public bool First
        {
            get { return mbool_first; }
            set
            {
                mbool_first = value;
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
                    enumColumnStatus previousStatus = menum_status;
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
            get { return mint_columnIndex; }
            set { mint_columnIndex = value; }
        }

        /// <summary>
        /// Gets or sets the system index.
        /// </summary>
        public int SystemID
        {
            get { return mint_systemIndex; }
            set { mint_systemIndex = value; }
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Name
        {
            get { return mstring_name; }
            set
            {
                if (value != mstring_name)
                {
                    string oldName = mstring_name;

                    mstring_name = value;
                    if (NameChanged != null)
                    {
                        NameChanged(this, mstring_name, oldName);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the column.
        /// </summary>
        public Color Color
        {
            get { return mobj_columnColor; }
            set
            {
                if (mobj_columnColor != value && value != null)
                {
                    if (ColorChanged != null)
                        ColorChanged(this, mobj_columnColor, value);
                }
                mobj_columnColor = value;
            }
        }

        #endregion
    }
}