using System;

namespace Interview
{
    public  class Storeable : IStoreable
    {
        public IComparable Id { get; set; }
    }
}