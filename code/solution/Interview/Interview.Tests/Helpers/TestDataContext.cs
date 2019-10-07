using System.Collections.Generic;

namespace Interview.Tests.Helpers
{
    public class TestDataContext : IDataContext<InMemoryImplementation>
    {
        public List<InMemoryImplementation> Data { get; set; }
    }
}