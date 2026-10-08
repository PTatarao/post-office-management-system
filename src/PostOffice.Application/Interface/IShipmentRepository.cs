
using PostOffice.Application.Contracts.Shipments;
using PostOffice.Application.Common.Pagination;
using PostOffice.Domain.Entities;

namespace PostOffice.Application.Abstractions;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id);
    Task<bool> ExistsByShipmentNumberAsync(string shipmentNumber, Guid? Id);
    Task AddAsync(Shipment shipment);
    void Remove(Shipment shipment);
    Task<PagedResult<Shipment>> SearchAsync(ShipmentFilter filter, int pageNumber, int pageSize);
}
