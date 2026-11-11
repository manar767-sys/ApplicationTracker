using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Models;
using ApplicationTracker.Practice.Data;
using ApplicationTracker.Repository;
namespace ApplicationTracker.Data
{
    public class DataStore<T> : IDataStore<T> where T : class, IEntity
    {
        protected SeedingData seedingData;
        public DataStore(SeedingData seedingData)
        {
            this.seedingData = seedingData;
        }
        void IDataStore<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        List<T> IDataStore<T>.GetAll()
        {
            throw new NotImplementedException();
        }

        T IDataStore<T>.GetById(int id)
        {
            throw new NotImplementedException();
        }

        void IDataStore<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        void IDataStore<T>.Update(T item)
        {
            throw new NotImplementedException();
        }
    }
}

