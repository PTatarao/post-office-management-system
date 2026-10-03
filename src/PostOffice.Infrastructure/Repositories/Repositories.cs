using Microsoft.EntityFrameworkCore;
using PostOffice.Application.Abstractions;
using PostOffice.Application.Common.Pagination;
using PostOffice.Application.Contracts.Shipments;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Infrastructure.Persistence;

public sealed class PostOfficeRepository(PostOfficeDbContext db) : IPostOfficeRepository
{
    public Task<PostOfficeEntity?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.PostOffices.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByZipCodeAsync(string zipCode, Guid? excludeId, CancellationToken ct) =>
        db.PostOffices.AnyAsync(x => x.ZipCode == zipCode && (!excludeId.HasValue || x.Id != excludeId.Value), ct);

    public async Task AddAsync(PostOfficeEntity entity, CancellationToken ct) => await db.PostOffices.AddAsync(entity, ct);
    public void Remove(PostOfficeEntity entity) => db.PostOffices.Remove(entity);
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}

public sealed class ShipmentRepository(PostOfficeDbContext db) : IShipmentRepository
{
    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Shipments.Include(x => x.StatusHistory).FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByShipmentNumberAsync(string shipmentNumber, Guid? excludeId, CancellationToken ct) =>
        db.Shipments.AnyAsync(x => x.ShipmentNumber == shipmentNumber && (!excludeId.HasValue || x.Id != excludeId.Value), ct);

    public async Task AddAsync(Shipment entity, CancellationToken ct) => await db.Shipments.AddAsync(entity, ct);
    public void Remove(Shipment entity) => db.Shipments.Remove(entity);

    public async Task<PagedResult<Shipment>> SearchAsync(ShipmentFilter filter, int pageNumber, int pageSize, CancellationToken ct)
    {
        IQueryable<Shipment> query = db.Shipments.AsNoTracking();

        if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status.Value);
        if (filter.LocationPostOfficeId.HasValue) query = query.Where(x => x.CurrentPostOfficeId == filter.LocationPostOfficeId.Value);
        if (!string.IsNullOrWhiteSpace(filter.ShipmentNumber)) query = query.Where(x => x.ShipmentNumber.Contains(filter.ShipmentNumber));
        if (!string.IsNullOrWhiteSpace(filter.ShipmentType))
        {
            if (filter.ShipmentType.Equals("Package", StringComparison.OrdinalIgnoreCase))
                query = query.OfType<Package>();
            else if (filter.ShipmentType.Equals("Letter", StringComparison.OrdinalIgnoreCase))
                query = query.OfType<Letter>();
        }

        if (filter.Weight.HasValue)
        {
            query = filter.Weight.Value switch
            {
                WeightCategory.LessThan1Kg => query.Where(x => x.WeightKg < 1m),
                WeightCategory.Between1And5Kg => query.Where(x => x.WeightKg >= 1m && x.WeightKg <= 5m),
                WeightCategory.MoreThan5Kg => query.Where(x => x.WeightKg > 5m),
                _ => query
            };
        }

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<Shipment>(items, pageNumber, pageSize, total);
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
