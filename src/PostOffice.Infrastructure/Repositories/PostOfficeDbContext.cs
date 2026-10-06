using MongoDB.Driver;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Infrastructure.Persistence;

public sealed class PostOfficeDbContext
{
    private readonly IMongoDatabase _database;

    public PostOfficeDbContext(IMongoClient client, string databaseName)
    {
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<PostOfficeEntity> PostOffices => _database.GetCollection<PostOfficeEntity>("post_offices");
    public IMongoCollection<Shipment> Shipments => _database.GetCollection<Shipment>("shipments");
    public IMongoCollection<Letter> Letters => _database.GetCollection<Letter>("letters");
    public IMongoCollection<Package> Packages => _database.GetCollection<Package>("packages");
    public IMongoCollection<ShipmentStatusHistory> ShipmentStatusHistory => _database.GetCollection<ShipmentStatusHistory>("shipment_status_history");

    // No-op for compatibility with EF-style repositories that call SaveChangesAsync
    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
}
