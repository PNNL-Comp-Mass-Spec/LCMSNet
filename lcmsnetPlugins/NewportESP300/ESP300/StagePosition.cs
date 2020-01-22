using System.Collections.Generic;

namespace LcmsNetPlugins.Newport.ESP300
{
    /// <summary>
    /// represents a stage position in x,y,z coordinates
    /// </summary>
    public class StagePosition
    {
        private readonly List<float> m_coordinates;

        public StagePosition()
        {
            NumAxes = 2;
            m_coordinates = new List<float> {
                0.0f,
                0.0f,
                0.0f
            };
        }

        /// <summary>
        /// Get the coordinate of specified axis
        /// </summary>
        /// <param name="axis">integer representing axis</param>
        /// <returns></returns>
        public float this[int axis]
        {
            get => m_coordinates[axis];
            set => m_coordinates[axis] = value;
        }

        /// <summary>
        /// Gets or Sets number of axes available to this position.
        /// </summary>
        public int NumAxes { get; set; }
    }
}
