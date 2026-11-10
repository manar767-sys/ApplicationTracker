using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Practice
{
    //whre T: class means that the generic type T must be a reference type (class). This constraint ensures that T cannot be a value type (like int, float, etc.) or a struct. It allows the Any<T> class to work specifically with objects that are instances of classes, enabling features like nullability and reference semantics.
    public class Any<T> where T: class
    {
        public T[] _items;
        public void Add(T item)
        {
            if (_items == null)
            {
                _items = new T[1];
                _items[0] = item;
            }
            else
            {
                T[] newArray = new T[_items.Length + 1];
                for (int i = 0; i < _items.Length; i++)
                {
                    newArray[i] = _items[i];
                }
                newArray[newArray.Length - 1] = item;
                _items = newArray;
            }
        }
        public void Remove(T item)
        {
            if (_items == null) return;
            int index = Array.IndexOf(_items, item);
            if (index < 0) return;
            T[] newArray = new T[_items.Length - 1];
            for (int i = 0, j = 0; i < _items.Length; i++)
            {
                if (i != index)
                {
                    newArray[j++] = _items[i];
                }
            }
            _items = newArray;
        }
        public int Count()
        {
            if (_items == null) return 0;
            return _items.Length;
        }
        public void PrintItems()
        {
            if (_items == null)
            {
                Console.WriteLine("No items to display.");
                return;
            }
            foreach (var item in _items)
            {
                Console.WriteLine(item);
            }
        }
    }
    public class Person
    {
        public int id;
        public string firstName;
        public string lastName;
        public override string ToString()
        {
            return $"{firstName} {lastName} (ID: {id})";
        }
    }
}
