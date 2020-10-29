using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public List<ListItem> Items { get; set; }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count
        {
            get
            {
                return Items.Count;
            }
        
        }


        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

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
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public ListItem FindByValue(string value)
        {
            return Items.Where(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
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
