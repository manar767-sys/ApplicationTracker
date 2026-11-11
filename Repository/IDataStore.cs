using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Models;

namespace ApplicationTracker.Repository
{
    public interface IDataStore<T> where T : class, IEntity
    {
        public void Add(T item);
        public T GetById(int id);
        public List<T> GetAll();
        public void Update(T item);
        public void Remove(T item);

    }
}
