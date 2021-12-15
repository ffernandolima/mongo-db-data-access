using System.Collections.Generic;

namespace MongoDB.Infrastructure.Abstractions
{
    public interface ISaveChangesResult
    {
        IReadOnlyList<object> Results { get; }
    }
}
