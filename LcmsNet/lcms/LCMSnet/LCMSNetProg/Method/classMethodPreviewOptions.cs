using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Method
{
    /// <summary>
    /// Options for previewing a method alignment.
    /// </summary>
    public class classMethodPreviewOptions
    {
        /// <summary>
        /// Gets or sets whether to animate the method alignment.
        /// </summary>
        public bool Animate { get; set; }

        /// <summary>
        /// Gets or sets the animation delay.
        /// </summary>
        public int AnimateDelay { get; set; }

        /// <summary>
        /// Gets or sets the frame delay count.
        /// </summary>
        public int FrameDelay { get; set; }
    }
}