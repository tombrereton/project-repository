using System;
using System.Collections.Generic;
// ReSharper disable InconsistentlySynchronizedField

namespace Interview
{
    public class InMemoryRepository<T> : IRepository<T> where T : IStoreable
    {
        private readonly IDataContext<T> _context;
        private readonly object _contextLock;

        public InMemoryRepository(IDataContext<T> context)
        {
            _context = context;
            _contextLock = new object();
        }

        public IEnumerable<T> All()
        {
            return _context.Data;
        }

        public void Delete(IComparable id)
        {
            lock (_contextLock)
            {
                var entity = FindById(id);
                _context.Data.Remove(entity);
            }
        }

        public void Save(T item)
        {
            if (FindById(item.Id) == null)
            {
                lock (_contextLock)
                {
                    _context.Data.Add(item);
                }
            }
        }

        public T FindById(IComparable id)
        {
            return _context.Data.Find(entity => entity.Id.Equals(id));
        }
    }
}