using System.Collections.Generic;

namespace MongoDB.Infrastructure
{
    public interface ISaveChangesResult
    {
        IReadOnlyList<object> Results { get; }
    }
}
