using System;
using System.Collections.Generic;

namespace Interview
{
    public class InMemoryRepository<T> : IRepository<T> where T : IStoreable
    {
        private readonly IDataContext<T> _context;

        public InMemoryRepository(IDataContext<T> context)
        {
            _context = context;
        }

        public IEnumerable<T> All()
        {
            return _context.Data;
        }

        public void Delete(IComparable id)
        {
            var entity = FindById(id);
            _context.Data.Remove(entity);
        }

        public void Save(T item)
        {
            if (FindById(item.Id) == null)
            {
                _context.Data.Add(item);
            }
        }

        public T FindById(IComparable id)
        {
            return _context.Data.Find(entity => entity.Id.Equals(id));
        }

    }
}