namespace MongoDB.Infrastructure
{
    public interface IMongoDbFluentConfiguration
    {
        bool IsConfigured { get; }
        void Configure();
    }
}
