using System;
using System.Collections.Generic;
using FluentAssertions;
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
            _repository = new InMemoryRepository<InMemoryImplementation>(_context.Object);
        }

        [Test]
        public void ProvideAllEntitiesAsCorrectType()
        {
            _context.Setup(context => context.Data).Returns(new List<InMemoryImplementation>());

            var actual = _repository.All();

            actual.Should().BeOfType<List<InMemoryImplementation>>();
        }

        [Test]
        public void ProvideAllEntities()
        {
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
            var storeable = new InMemoryImplementation { Id = 1 };
            _context.Setup(context => context.Data).Returns(new List<InMemoryImplementation>());

            _repository.Save(storeable);
            var actual = _repository.All();

            actual.Should().Contain(storeable);
        }
        
        [Test]
        public void FindEntity()
        {
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
            _context.Setup(context => context.Data).Returns(new List<InMemoryImplementation>());
        
            Action deleteAct = () => _repository.Delete(1);
        
            deleteAct.Should().NotThrow<Exception>();
        }
    }
}
