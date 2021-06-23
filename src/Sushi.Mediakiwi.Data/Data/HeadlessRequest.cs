namespace Sushi.Mediakiwi.Data
{
    public class HeadlessRequest
    {
        public Page Page {get;set; } 

        public Component Component { get; set; }

        public SubList.SubListitem Listitem { get; set; }
        
        public bool IsPreview { get; set; }

        public object Result { get; set; }
    }
}
