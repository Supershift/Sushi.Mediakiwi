namespace Sushi.MailTemplate.Entities
{
    /// <summary>
    /// Placeholder class
    /// </summary>
    public class Placeholder
    {
        /// <summary>
        /// Placeholder ctor
        /// </summary>
        public Placeholder()
        {

        }

        /// <summary>
        /// Placeholder ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Placeholder(string name, string value)
        {
            Name = name;
            Value = value;
        }

        private string name;
        /// <summary>
        /// The name of the placeholder, always in uppercase
        /// </summary>
        public string Name
        {
            get
            {
                return name.ToUpper();
            }
            set
            {
                name = value.ToUpper();
            }
        }

        /// <summary>
        /// The value of the placeholder
        /// </summary>
        public string Value { get; set; }
    }
}