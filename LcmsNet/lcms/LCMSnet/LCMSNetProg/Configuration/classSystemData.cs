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
            m_systemIndex = 0;
            m_column = new List<classColumnData>();
        }

        #endregion

        #region Members

        /// <summary>
        /// System index of the column.
        /// </summary>
        private int m_systemIndex;

        /// <summary>
        /// Fired when the color of the column changes.
        /// </summary>
        public event DelegateColorChanged ColorChanged;

        /// <summary>
        /// Color of the column.
        /// </summary>
        private Color m_columnColor;

        /// <summary>
        /// List of columns associated with the system.
        /// </summary>
        private List<classColumnData> m_column;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the columns associated with this system.
        /// </summary>
        public List<classColumnData> Columns
        {
            get { return m_column; }
            set { m_column = value; }
        }

        /// <summary>
        /// Gets or sets the system index.
        /// </summary>
        public int SystemIndex
        {
            get { return m_systemIndex; }
            set { m_systemIndex = value; }
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