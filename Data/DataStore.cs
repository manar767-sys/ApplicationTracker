using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Models;

namespace ApplicationTracker.Data
{
    public class DataStore<T> where T : class,IEntity
    {
        private List<T> _items;
        public DataStore()
        {
            _items = new List<T>();
        }
        public void Add(T item)
        {
            _items.Add(item);
        }
        public void Remove(T item)
        {
            _items.Remove(item);
        }
        public List<T> GetAll()
        {
            return _items;
        }
        public T GetById(int id)
        {
            // Assuming T has a property named "Id" of type int
            return _items.Find(item => (int)item.GetType().GetProperty("Id").GetValue(item) == id);
        }
        public void Update(T item)
        {
            //  T has a property named "Id" of type int that is inherited from IEntity
            int id = (int)item.GetType().GetProperty("Id").GetValue(item);
            var existingItem = GetById(id);
            if (existingItem != null)
            {
                int index = _items.IndexOf(existingItem);
                _items[index] = item;
            }
        }

    }
}

