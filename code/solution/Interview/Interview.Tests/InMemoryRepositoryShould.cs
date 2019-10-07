using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Interview.Tests.Helpers;
using Moq;
using NUnit.Framework;

namespace Interview.Tests
{
    [TestFixture]
    public class InMemoryRepositoryShould
    {
        private Mock<IDataContext<InMemoryImplementation>> _context;
        private InMemoryRepository<InMemoryImplementation> _repository;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<IDataContext<InMemoryImplementation>>();
        }

        [Test]
        public void ProvideAllEntitiesAsCorrectType()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            _context.Setup(context => context.Data).Returns(new List<InMemoryImplementation>());

            var actual = _repository.All();

            actual.Should().BeOfType<List<InMemoryImplementation>>();
        }

        [Test]
        public void ProvideAllEntities()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            var storeable = new InMemoryImplementation { Id = 1 };
            var secondStoreable = new InMemoryImplementation { Id = 2 };
            var storeables = new List<InMemoryImplementation> { storeable, secondStoreable };
            _context.Setup(context => context.Data).Returns(storeables);

            var actual = _repository.All();

            actual.Should().HaveCount(2);
        }

        [Test]
        public void StoreAndPersistAnEntity()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            var storeable = new InMemoryImplementation { Id = 1 };
            _context.Setup(context => context.Data).Returns(new List<InMemoryImplementation>());

            _repository.Save(storeable);
            var actual = _repository.All();

            actual.Should().Contain(storeable);
        }

        [Test]
        public void FindEntity()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            const int entityId = 1;
            var storeable = new InMemoryImplementation { Id = entityId };
            var storeables = new List<InMemoryImplementation> { storeable };
            _context.Setup(context => context.Data).Returns(storeables);

            var actual = _repository.FindById(entityId);

            actual.Should().BeSameAs(storeable);
        }

        [Test]
        public void FindCorrectEntity()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            const int entityId = 1;
            const int secondEntityId = 2;
            var storeable = new InMemoryImplementation { Id = entityId };
            var secondStoreable = new InMemoryImplementation { Id = secondEntityId };
            var storeables = new List<InMemoryImplementation> { storeable, secondStoreable };
            _context.Setup(context => context.Data).Returns(storeables);

            var actual = _repository.FindById(secondEntityId);

            actual.Should().BeSameAs(secondStoreable);
        }

        [Test]
        public void NotStoreADuplicateEntity()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            const int entityId = 1;
            var storeable = new InMemoryImplementation { Id = entityId };
            var duplicateStoreable = new InMemoryImplementation { Id = entityId };
            var storeables = new List<InMemoryImplementation> { storeable };
            _context.Setup(context => context.Data).Returns(storeables);

            _repository.Save(duplicateStoreable);
            var actual = _repository.All();

            actual.Should().NotContain(duplicateStoreable);
        }

        [Test]
        public void RemoveAnEntity()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            const int entityId = 1;
            var storeable = new InMemoryImplementation { Id = entityId };
            var storeables = new List<InMemoryImplementation> { storeable };
            _context.Setup(context => context.Data).Returns(storeables);

            _repository.Delete(entityId);
            var actual = _repository.All();

            actual.Should().NotContain(storeable);
        }

        [Test]
        public void RemoveCorrectEntity()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            const int entityId = 1;
            const int secondEntityId = 2;
            var storeable = new InMemoryImplementation { Id = entityId };
            var secondStoreable = new InMemoryImplementation { Id = secondEntityId };
            var storeables = new List<InMemoryImplementation> { storeable, secondStoreable };
            _context.Setup(context => context.Data).Returns(storeables);

            _repository.Delete(secondEntityId);
            var actual = _repository.All();

            actual.Should().NotContain(secondStoreable);
        }

        [Test]
        public void HandleRemovingAnEntityThatDoesNotExist()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            _context.Setup(context => context.Data).Returns(new List<InMemoryImplementation>());

            Action deleteAct = () => _repository.Delete(1);

            deleteAct.Should().NotThrow<Exception>();
        }

        [Test]
        public void AddAllProductWhenAccessedByMultipleThreads()
        {
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
            _context.Setup(context => context.Data).Returns(new List<InMemoryImplementation>());
            var list1 = BuildTestListOfStoreable(0, 1000);
            var list2 = BuildTestListOfStoreable(1000, 2000);
            var thread1 = new Thread(() => AddStoreablesToRepository(list1));
            var thread2 = new Thread(() => AddStoreablesToRepository(list2));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            var actual = _repository.All();

            actual.Should().HaveCount(2000);
        }

        [Test]
        public void DeleteAllProductsWhenAccessedByMultipleThreads()
        {
            var list1 = BuildTestListOfStoreable(0, 1000);
            IDataContext<InMemoryImplementation> context = new TestDataContext() { Data = list1 };
            _repository = new InMemoryRepository<InMemoryImplementation>(context);

            var thread1 = new Thread(() => DeleteStoreablesFromRepository(0, 500));
            var thread2 = new Thread(() => DeleteStoreablesFromRepository(500, 1000));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            var actual = _repository.All();

            actual.Should().HaveCount(0);
        }

        private void DeleteStoreablesFromRepository(int start, int finish)
        {
            for (int i = start; i < finish; i++)
            {
                _repository.Delete(i);
            }
        }

        private static List<InMemoryImplementation> BuildTestListOfStoreable(int start, int finish)
        {
            var storeables = new List<InMemoryImplementation>();
            for (var i = start; i < finish; i++)
            {
                var storeable = new InMemoryImplementation { Id = i };
                storeables.Add(storeable);

            }

            return storeables;
        }

        private void AddStoreablesToRepository(IEnumerable<InMemoryImplementation> storeables)
        {
            foreach (var storeable in storeables)
            {
                _repository.Save(storeable);
            }
        }
    }
}
