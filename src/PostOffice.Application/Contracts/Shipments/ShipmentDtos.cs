using PostOffice.Domain.Entities;

namespace PostOffice.Application.Contracts.Shipments;

public sealed record ShipmentDto(
    Guid Id,
    string ShipmentNumber,
    string ShipmentType,
    decimal WeightKg,
    WeightCategory WeightCategory,
    ShipmentStatus Status,
    Guid OriginPostOfficeId,
    Guid DestinationPostOfficeId,
    Guid CurrentPostOfficeId,
    DateTime CreatedAtUtc);

public sealed record CreateShipmentRequest(
    string ShipmentNumber,
    string ShipmentType,
    decimal WeightKg,
    Guid OriginPostOfficeId,
    Guid DestinationPostOfficeId);

public sealed record UpdateShipmentRequest(
    decimal WeightKg,
    Guid OriginPostOfficeId,
    Guid DestinationPostOfficeId);

public sealed record UpdateShipmentStatusRequest(
    ShipmentStatus Status,
    Guid PostOfficeId);

public sealed record ShipmentFilter(
    ShipmentStatus? Status,
    Guid? LocationPostOfficeId,
    WeightCategory? Weight,
    string? ShipmentNumber,
    string? ShipmentType);
