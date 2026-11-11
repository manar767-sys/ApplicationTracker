
using ApplicationTracker.Models;
using ApplicationTracker.Repository;
namespace ApplicationTracker.Data
{
    public class DataStore<T> : IDataStore<T> where T : class, IEntity
    {
        private List<T> items;
        private int nextId = 1; // To keep track of the next unique ID
        public DataStore()
        {
            this.items = new List<T>();
        }

        public  virtual void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            item.Id = nextId++; // Assign a unique ID to the item
            items.Add(item);
        }

        public  virtual List<T> GetAll()
        {
            return new List<T>(items);
        }

        public  virtual T GetById(int id)
        {
            foreach (var item in items)
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
            if (!items.Contains(item))
            {
                throw new ArgumentException("Item not found");
            }
            items.Remove(item);
        }

        public  virtual void Update(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            var existingItem = GetById(item.Id);
            var index = items.IndexOf(existingItem);
            if (index != -1)
            {
                items[index] = item;
            }
            else
            {
                throw new ArgumentException("Item not found");
            }
        }
        }
    }


