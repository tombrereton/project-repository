using System;
using System.Collections.Generic;

namespace Interview
{
    internal class InMemoryRepository<T> : IRepository<T>, IDataContext<T> where T : IStoreable
    {
        public InMemoryRepository()
        {
            Data = new List<T>();
        }

        public IEnumerable<T> All()
        {
            return Data;
        }

        public void Delete(IComparable id)
        {
            var entity = FindById(id);
            Data.Remove(entity);
        }

        public void Save(T item)
        {
            if (FindById(item.Id) == null)
            {
                Data.Add(item);
            }
        }

        public T FindById(IComparable id)
        {
            return Data.Find(entity => entity.Id.Equals(id));
        }

        public List<T> Data { get; set; }
    }
}