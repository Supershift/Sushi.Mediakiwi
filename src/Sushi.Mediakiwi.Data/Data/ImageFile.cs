namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ImageFile entity.
    /// </summary>
    public class ImageFile
    {
        /// <summary>
        /// Gets or sets the extention.
        /// </summary>
        /// <value>The extention.</value>
        public string Extention { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the alternate text.
        /// </summary>
        /// <value>The alternate text.</value>
        public string AlternateText { get; set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; set; }
    }
}