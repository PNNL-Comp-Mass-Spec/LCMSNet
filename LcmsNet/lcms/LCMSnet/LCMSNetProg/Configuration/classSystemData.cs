using System;
using System.Collections.Generic;
using System.Drawing;
using LcmsNetDataClasses.Configuration;

namespace LcmsNet.Configuration
{
    /// <summary>
    /// Class that manages all of the information about a given column
    /// </summary>
    public class classSystemData
    {
        #region Delegate Definitions

        public delegate void DelegateColorChanged(object sender, Color previousColor, Color newColor);

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classSystemData()
        {
            mint_systemIndex = 0;
            mlist_column = new List<classColumnData>();
        }

        #endregion

        #region Members

        /// <summary>
        /// System index of the column.
        /// </summary>
        private int mint_systemIndex;

        /// <summary>
        /// Fired when the color of the column changes.
        /// </summary>
        public event DelegateColorChanged ColorChanged;

        /// <summary>
        /// Color of the column.
        /// </summary>
        private Color mobj_columnColor;

        /// <summary>
        /// List of columns associated with the system.
        /// </summary>
        private List<classColumnData> mlist_column;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the columns associated with this system.
        /// </summary>
        public List<classColumnData> Columns
        {
            get { return mlist_column; }
            set { mlist_column = value; }
        }

        /// <summary>
        /// Gets or sets the system index.
        /// </summary>
        public int SystemIndex
        {
            get { return mint_systemIndex; }
            set { mint_systemIndex = value; }
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