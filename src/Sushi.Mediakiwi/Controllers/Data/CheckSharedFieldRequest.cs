
namespace Sushi.Mediakiwi.Controllers.Data
{
    public class CheckSharedFieldRequest
    {
        /// <summary>
        /// The name of the field to check 
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Is this being enabled (true) or disabled (false)
        /// for this field
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
