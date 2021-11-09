namespace Sushi.Mediakiwi.Controllers.Data
{
    public class DocumentTypeFieldListItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int ContentTypeID { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsSharedField { get; set; }
        public string FieldName { get; set; }
        /// <summary>
        /// Zero based order
        /// </summary>
        public int SortOrder { get; set; }
        public bool CanOnlyCreate { get; set; }
    }
}
