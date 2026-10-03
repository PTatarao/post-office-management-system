
using PostOffice.Application.Contracts.Shipments;
using PostOffice.Application.Common.Pagination;
using PostOffice.Domain.Entities;

namespace PostOffice.Application.Abstractions;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByShipmentNumberAsync(string shipmentNumber, Guid? excludeId, CancellationToken ct);
    Task AddAsync(Shipment shipment, CancellationToken ct);
    void Remove(Shipment shipment);
    Task<PagedResult<Shipment>> SearchAsync(ShipmentFilter filter, int pageNumber, int pageSize, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
