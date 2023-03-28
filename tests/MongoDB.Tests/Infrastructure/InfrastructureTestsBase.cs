using Microsoft.Extensions.Configuration;
using MongoDB.Tests.Fixtures;
using System;
using Xunit;

namespace MongoDB.Tests.Infrastructure
{
    [Collection("Infrastructure")]
    public class InfrastructureTestsBase
    {
        public IConfiguration Configuration { get; }
        public IServiceProvider ServiceProvider { get; }
        public InfrastructureFixture InfrastructureFixture { get; }

        public InfrastructureTestsBase(InfrastructureFixture infrastructureFixture)
        {
            InfrastructureFixture = infrastructureFixture 
                ?? throw new ArgumentNullException(nameof(infrastructureFixture), $"{nameof(infrastructureFixture)} cannot be null.");

            Configuration = infrastructureFixture.Configuration;
            ServiceProvider = infrastructureFixture.ServiceProvider;
        }
    }
}
