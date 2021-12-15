using Microsoft.Extensions.DependencyInjection;

namespace MongoDB.Tests.Infrastructure
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
