using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    public class ASyncResult
    {
        public ASyncResult()
        {
        }

        public void ApplyCollection(ListItemCollection col)
        {
            this.Result = new List<Option>();
            for (int i = 0; i < col.Count; i++)
            {
                var option = col[i];
                Result.Add(new Option()
                {
                    Text = option.Text,
                    Value = option.Value,
                    Disabled = option.Enabled ? (bool?)null : true
                });
            }
        }

        public bool? OnSelectCallBack { get; set; }
        public string CallBack { get; set; }
        public string Property { get; set; }
        public List<Option> Result { get; set; }
        public string[] Reset { get; set; }
        public object Extend { get; set; }
        //public List<string> Hide { get; set; }
        //public List<string> Show { get; set; }
        //public List<string> Enable { get; set; }
        //public List<string> Disable { get; set; }
    }

    public class Option
    {
        public Option() { }
        public Option(string text, string value) { this.Text = text; this.Value = value; }
        public Option(string text, int value) { this.Text = text; this.Value = value.ToString(); }

        public string Text { get; set; }
        public string Value { get; set; }
        public bool? Disabled { get; set;  }
        public object Extend { get; set; }
    }
}
