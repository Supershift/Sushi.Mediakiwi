using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    public enum ImageFileFormat
    {
        /// <summary>
        /// Nothing
        /// </summary>
        None = 0,
        /// <summary>
        /// Convert uploaded image to jpeg.
        /// </summary>
        //Jpeg80 = 1,
        /// <summary>
        /// Convert uploaded image to jpeg with a fixed format (with border).
        /// </summary>
        FixedBorder = 2,
        /// <summary>
        /// Convert uploaded image to jpeg to and validate maximum width (no border).
        /// </summary>
        ValidateMaximumWidth = 3,
        /// <summary>
        /// Convert uploaded image to jpeg to and validate maximum height (no border).
        /// </summary>
        ValidateMaximumHeight = 4,
        /// <summary>
        /// 
        /// </summary>
        ValidateMaximumAndCrop = 5,
        /// <summary>
        /// 
        /// </summary>
        ValidateMaximumWidthAndHeight = 6
    }
}
