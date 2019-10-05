using System.Collections.Generic;

namespace Interview
{
    public interface IDataContext<T> where T : IStoreable
    {
        List<T> Data { get; set; }
    }
}