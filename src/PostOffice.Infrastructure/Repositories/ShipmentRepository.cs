using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PostOffice.Application.Abstractions;
using PostOffice.Application.Common.Pagination;
using PostOffice.Application.Contracts.Shipments;
using PostOffice.Application.Interface;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Infrastructure;
public sealed class ShipmentRepository(PostOfficeDataAccess db) : IShipmentRepository
{
    private readonly IMongoCollection<Shipment> _collection = db.Shipments();

    public Task<Shipment?> GetByIdAsync(Guid id)
    {
        return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public Task<bool> ExistsByShipmentNumberAsync(string shipmentNumber, Guid? Id)
    {
        var filter = Builders<Shipment>.Filter.Eq(x => x.ShipmentNumber, shipmentNumber);
        if (Id.HasValue) filter &= Builders<Shipment>.Filter.Ne(x => x.Id, Id.Value);
        return _collection.Find(filter).AnyAsync();
    }

    public Task AddAsync(Shipment entity) 
    { 
        return _collection.InsertOneAsync(entity); 
    }
    public void Remove(Shipment entity)
    {
         _collection.DeleteOne(x => x.Id == entity.Id);
    }

    public async Task<PagedResult<Shipment>> SearchAsync(ShipmentFilter filter, int pageNumber, int pageSize)
    {
        var builder = Builders<Shipment>.Filter;
        var f = builder.Empty;

        if (filter.Status.HasValue) f &= builder.Eq(x => x.Status, filter.Status.Value);
        if (filter.LocationPostOfficeId.HasValue) f &= builder.Eq(x => x.CurrentPostOfficeId, filter.LocationPostOfficeId.Value);
        if (!string.IsNullOrWhiteSpace(filter.ShipmentNumber)) f &= builder.Regex(x => x.ShipmentNumber, new BsonRegularExpression(filter.ShipmentNumber, "i"));

        if (!string.IsNullOrWhiteSpace(filter.ShipmentType))
        {
              var typeName = filter.ShipmentType.Equals("Package", StringComparison.OrdinalIgnoreCase) ? nameof(Package)
                : filter.ShipmentType.Equals("Letter", StringComparison.OrdinalIgnoreCase) ? nameof(Letter) : null;

            if (!string.IsNullOrEmpty(typeName))
                f &= builder.Eq("_t", typeName);
        }

        if (filter.Weight.HasValue)
        {
            f &= filter.Weight.Value switch
            {
                WeightCategory.LessThan1Kg => builder.Lt(x => x.WeightKg, 1m),
                WeightCategory.Between1And5Kg => builder.And(builder.Gte(x => x.WeightKg, 1m), builder.Lte(x => x.WeightKg, 5m)),
                WeightCategory.MoreThan5Kg => builder.Gt(x => x.WeightKg, 5m),
                _ => builder.Empty
            };
        }

        var total = await _collection.CountDocumentsAsync(f);
        var items = await _collection.Find(f)
            .SortByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResult<Shipment>(items, pageNumber, pageSize, (int)total);
    }
}
