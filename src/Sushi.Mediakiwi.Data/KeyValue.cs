namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    public class KeyValue
    {
        public KeyValue()
        {
        }

        public KeyValue(string key, bool removeKey = true)
        {
            Key = key;
            RemoveKey = removeKey;
        }

        public KeyValue(string key, object value)
        {
            Key = key;
            Value = value.ToString();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [remove key].
        /// </summary>
        /// <value><c>true</c> if [remove key]; otherwise, <c>false</c>.</value>
        public bool RemoveKey { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal bool Done;
    }
}