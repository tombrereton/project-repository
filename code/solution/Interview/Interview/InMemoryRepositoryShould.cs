using System;
using System.Collections.Generic;
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
            var repository = new InMemoryRepository<Storeable>();

            var actual = repository.All();

            actual.Should().BeOfType<List<Storeable>>();
        }

        [Test]
        public void StoreAndPersistAnEntity()
        {
            var repository = new InMemoryRepository<Storeable>();
            var storeable = new Storeable { Id = 1 };
            repository.Save(storeable);

            var actual = repository.All();

            actual.Should().Contain(storeable);
        }

        [Test]
        public void FindEntity()
        {
            var repository = new InMemoryRepository<Storeable>();
            const int entityId = 1;
            var storeable = new Storeable { Id = entityId };
            repository.Save(storeable);

            var actual = repository.FindById(entityId);

            actual.Should().BeSameAs(storeable);
        }

        [Test]
        public void FindCorrectEntity()
        {
            var repository = new InMemoryRepository<Storeable>();
            const int entityId = 1;
            const int secondEntityId = 2;
            var storeable = new Storeable { Id = entityId };
            var secondStoreable = new Storeable { Id = secondEntityId };
            repository.Save(storeable);
            repository.Save(secondStoreable);

            var actual = repository.FindById(secondEntityId);

            actual.Should().BeSameAs(secondStoreable);
        }

        [Test]
        public void NotStoreADuplicateEntity()
        {
            var repository = new InMemoryRepository<Storeable>();
            var storeable = new Storeable { Id = 1 };
            var duplicateStoreable = new Storeable { Id = 1 };
            repository.Save(storeable);
            repository.Save(duplicateStoreable);

            var actual = repository.All();

            actual.Should().NotContain(duplicateStoreable);
        }
        
        [Test]
        public void RemoveAnEntity()
        {
            var repository = new InMemoryRepository<Storeable>();
            const int entityId = 1;
            var storeable = new Storeable { Id = entityId };
            repository.Save(storeable);
            repository.Delete(entityId);

            var actual = repository.All();
        
            actual.Should().NotContain(storeable);
        }

        [Test]
        public void RemoveCorrectEntity()
        {
            var repository = new InMemoryRepository<Storeable>();
            const int entityId = 1;
            const int secondEntityId = 2;
            var storeable = new Storeable { Id = entityId };
            var secondStoreable = new Storeable(){Id = secondEntityId};
            repository.Save(storeable);
            repository.Save(secondStoreable);
            repository.Delete(secondEntityId);

            var actual = repository.All();
        
            actual.Should().NotContain(secondStoreable);
        }

        [Test]
        public void HandleRemovingAnEntityThatDoesNotExist()
        {
            var repository = new InMemoryRepository<Storeable>();

            Action deleteAct = () => repository.Delete(1);

            deleteAct.Should().NotThrow<Exception>();
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
