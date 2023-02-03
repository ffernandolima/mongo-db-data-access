using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal interface IThrottlingMongoDbSemaphore
    {
        Task AddRequestAsync(Task task);
        Task<T> AddRequestAsync<T>(Task<T> task);
    }
}
