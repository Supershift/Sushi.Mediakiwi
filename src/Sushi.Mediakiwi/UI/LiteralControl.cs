namespace Sushi.Mediakiwi.UI
{
    public class LiteralControl : IControl
    {
        public LiteralControl()
        {

        }
        public LiteralControl(string value)
        {
            this.Text = value;
        }

        public string Text { get; set; }
    }
}
