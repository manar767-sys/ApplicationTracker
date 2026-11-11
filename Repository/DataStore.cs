
using ApplicationTracker.Models;
using ApplicationTracker.Repository;
namespace ApplicationTracker.Data
{
    public class DataStore<T> : IDataStore<T> where T : class, IEntity
    {
        private protected List<T> _items = new List<T>();
        private protected int nextId = 1; // To keep track of the next unique ID

 

        public DataStore( )
        {
        }

        public  virtual void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            item.Id = nextId++; // Assign a unique ID to the item
            _items.Add(item);
        }

        public  virtual List<T> GetAll()
        {
            return new List<T>(_items);
        }

        public  virtual T GetById(int id)
        {
            foreach (var item in _items)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            throw new ArgumentException("Item not found");
        }

        public  virtual void Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            if (!_items.Contains(item))
            {
                throw new ArgumentException("Item not found");
            }
            _items.Remove(item);
        }

        public  virtual void Update(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            var existingItem = GetById(item.Id);
            var index = _items.IndexOf(existingItem);
            if (index != -1)
            {
                _items[index] = item;
            }
            else
            {
                throw new ArgumentException("Item not found");
            }
        }
        }
    }


