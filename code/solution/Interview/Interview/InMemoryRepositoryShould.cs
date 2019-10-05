using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Interview
{
    [TestFixture]
    internal class InMemoryRepositoryShould
    {
        [Test]
        public void ProvideAllEntitiesAsCorrectType()
        {
            var repository =  new InMemoryRepository<Storeable>();

            var actual = repository.All();

            actual.Should().BeOfType<List<Storeable>>();
        }

    }

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
            throw new NotImplementedException();
        }

        public void Save(T item)
        {
            throw new NotImplementedException();
        }

        public T FindById(IComparable id)
        {
            throw new NotImplementedException();
        }

        public List<T> Data { get; set; }
    }
}
