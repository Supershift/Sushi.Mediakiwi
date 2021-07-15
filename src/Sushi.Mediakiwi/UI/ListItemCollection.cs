using System;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.UI
{
    public class ListItemCollection 
    {
        public ListItemCollection()
        {
            Items = new List<ListItem>();
            
        }
        public ListItem this[int index] { 
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        public List<ListItem> Items { get; private set; }

        public int Count
        {
            get
            {
                return Items.Count;
            }
        
        }

        public void Add(ListItem value)
        {
            Items.Add(value);
        }
        public void Add(string value)
        {
            Items.Add(new ListItem(value));
        }
        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var find = FindByValue(value.ToString());
            return find != null;
        }

        public ListItem FindByValue(string value)
        {
            return Items.FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerator<ListItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public void Insert(int index, object value)
        {
            Items.Add(new ListItem(value));
        }

        public void Remove(ListItem index)
        {
            Items.Remove(index);
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }
    }
}
