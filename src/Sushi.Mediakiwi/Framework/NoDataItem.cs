namespace Sushi.Mediakiwi.Framework
{
    public class NoDataItem
    {
        public NoDataItem()
        {
            this.Title = "No result";
            this.SubTitle = "No data present";
            this.IconClass = "flaticon solid x-2 icon";
        }
        /// <summary>
        /// 
        /// </summary>
        public string SubTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IconClass { get; set; }
    }
}
