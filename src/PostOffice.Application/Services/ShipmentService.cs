using PostOffice.Application.Abstractions;
using PostOffice.Application.Common.Exceptions;
using PostOffice.Application.Common.Pagination;
using PostOffice.Application.Contracts.Shipments;
using PostOffice.Domain.Entities;

namespace PostOffice.Application.Services;

public sealed class ShipmentService(IShipmentRepository shipments, IPostOfficeRepository postOffices)
{
    public async Task<ShipmentDto> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await shipments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Shipment '{id}' was not found.");
        return Map(entity);
    }

    public async Task<PagedResult<ShipmentDto>> SearchAsync(ShipmentFilter filter, int pageNumber, int pageSize, CancellationToken ct)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await shipments.SearchAsync(filter, pageNumber, pageSize, ct);
        return new PagedResult<ShipmentDto>(result.Items.Select(Map).ToArray(), result.PageNumber, result.PageSize, result.TotalCount);
    }

    public async Task<ShipmentDto> CreateAsync(CreateShipmentRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ShipmentNumber))
            throw new ArgumentException("Shipment number is required.");

        if (!Enum.TryParse<ShipmentType>(request.ShipmentType, true, out var type))
            throw new ArgumentException("Shipment type must be Letter or Package.");

        if (await shipments.ExistsByShipmentNumberAsync(request.ShipmentNumber, null, ct))
            throw new InvalidOperationException($"Shipment number '{request.ShipmentNumber}' already exists.");

        await EnsurePostOfficeExists(request.OriginPostOfficeId, ct);
        await EnsurePostOfficeExists(request.DestinationPostOfficeId, ct);

        Shipment entity = type switch
        {
            ShipmentType.Letter => new Letter(request.ShipmentNumber.Trim(), request.WeightKg, request.OriginPostOfficeId, request.DestinationPostOfficeId),
            ShipmentType.Package => new Package(request.ShipmentNumber.Trim(), request.WeightKg, request.OriginPostOfficeId, request.DestinationPostOfficeId),
            _ => throw new ArgumentOutOfRangeException()
        };

        entity.StatusHistory.Add(new ShipmentStatusHistory(entity.Id, ShipmentStatus.ReceivedAtOrigin, entity.CurrentPostOfficeId, entity.CreatedAtUtc));
        await shipments.AddAsync(entity, ct);
        await shipments.SaveChangesAsync(ct);
        return Map(entity);
    }

    public async Task UpdateAsync(Guid id, UpdateShipmentRequest request, CancellationToken ct)
    {
        var entity = await shipments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Shipment '{id}' was not found.");

        await EnsurePostOfficeExists(request.OriginPostOfficeId, ct);
        await EnsurePostOfficeExists(request.DestinationPostOfficeId, ct);

        entity.Update(request.WeightKg, request.OriginPostOfficeId, request.DestinationPostOfficeId);
        await shipments.SaveChangesAsync(ct);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateShipmentStatusRequest request, CancellationToken ct)
    {
        var entity = await shipments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Shipment '{id}' was not found.");

        await EnsurePostOfficeExists(request.PostOfficeId, ct);
        entity.MoveTo(request.PostOfficeId, request.Status);
        await shipments.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await shipments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Shipment '{id}' was not found.");

        shipments.Remove(entity);
        await shipments.SaveChangesAsync(ct);
    }

    private async Task EnsurePostOfficeExists(Guid id, CancellationToken ct)
    {
        if (await postOffices.GetByIdAsync(id, ct) is null)
            throw new NotFoundException($"Post office '{id}' was not found.");
    }

    private static ShipmentDto Map(Shipment x) => new(
        x.Id, x.ShipmentNumber, x is Package ? "Package" : "Letter", x.WeightKg,
        x.WeightCategory, x.Status, x.OriginPostOfficeId, x.DestinationPostOfficeId,
        x.CurrentPostOfficeId, x.CreatedAtUtc);
}

internal enum ShipmentType { Letter, Package }
