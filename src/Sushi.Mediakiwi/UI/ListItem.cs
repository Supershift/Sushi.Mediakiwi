using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.UI
{
    public class ListItem
    {
        public ListItem()
        {

        }
        public ListItem(object text)
        {
            this.Text = text.ToString();
            this.Value = this.Text;
        }
        public ListItem(string text)
        {
            this.Text = text;
            this.Value = text;
        }
        public ListItem(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }

        public string Text { get; set; }
        public string Value { get; set; }
        public bool Enabled { get; set; } = true;
        public bool Selected { get; set; }
    }
}
