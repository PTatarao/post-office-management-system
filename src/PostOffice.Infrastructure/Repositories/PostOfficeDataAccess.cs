using MongoDB.Driver;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Infrastructure;

public sealed class PostOfficeDataAccess
{
    private readonly IMongoDatabase _database;

    public  PostOfficeDataAccess(IMongoClient client, string databaseName)
    {
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<PostOfficeEntity> PostOffices()
    {
       return _database.GetCollection<PostOfficeEntity>("post_offices");
    }
    public IMongoCollection<Shipment> Shipments()
    {
        return _database.GetCollection<Shipment>("shipments");
    }
 }
