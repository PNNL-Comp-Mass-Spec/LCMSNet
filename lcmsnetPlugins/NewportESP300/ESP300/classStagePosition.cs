using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newport.ESP300
{
    /// <summary>
    /// represents a stage position in x,y,z coordinates
    /// </summary>
    public class classStagePosition
    {
        private readonly List<float> m_coordinates;
        private int m_numAxes;

        public classStagePosition()
        {
            m_numAxes = 2;
            m_coordinates = new List<float>();
            m_coordinates.Add(0.0f);
            m_coordinates.Add(0.0f);
            m_coordinates.Add(0.0f);
        }


        /// <summary>
        /// Get the coordinate of specified axis
        /// </summary>
        /// <param name="axis">integer representing axis</param>
        /// <returns></returns>
        public float this[int axis]
        {
            get
            {
                return m_coordinates[axis];
            }
            set
            {
                m_coordinates[axis] = value;
            }
        }

        /// <summary>
        /// Gets or Sets number of axes available to this position.
        /// </summary>
        public int NumAxes
        {
            get
            {
                return m_numAxes;
            }
            set
            {
                m_numAxes = value;
            }
        }
    }
}
