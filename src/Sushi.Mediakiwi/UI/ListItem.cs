namespace Sushi.Mediakiwi.UI
{
    public class ListItem
    {
        public ListItem()
        {

        }
        public ListItem(object text)
        {
            Text = text.ToString();
            Value = Text;
        }

        public ListItem(string text)
        {
            Text = text;
            Value = text;
        }

        public ListItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; set; }
        public string Value { get; set; }
        public bool Enabled { get; set; } = true;
        public bool Selected { get; set; }
    }
}
