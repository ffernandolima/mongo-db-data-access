using Xunit;

namespace MongoDB.Tests.Fixtures
{
    [CollectionDefinition("Infrastructure")]
    public sealed class InfrastructureCollection : ICollectionFixture<InfrastructureFixture>
    { }
}
