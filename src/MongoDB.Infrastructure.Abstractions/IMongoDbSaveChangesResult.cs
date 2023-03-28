using System.Collections.Generic;

namespace MongoDB.Infrastructure
{
    public interface IMongoDbSaveChangesResult
    {
        IReadOnlyList<object> Results { get; }
    }
}
