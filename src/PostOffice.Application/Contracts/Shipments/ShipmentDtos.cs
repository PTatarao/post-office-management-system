using PostOffice.Domain.Entities;

namespace PostOffice.Application.Contracts.Shipments;

public  record ShipmentDto(
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

public  record CreateShipmentRequest(
    string ShipmentNumber,
    string ShipmentType,
    decimal WeightKg,
    Guid OriginPostOfficeId,
    Guid DestinationPostOfficeId);

public  record UpdateShipmentRequest(
    decimal WeightKg,
    Guid OriginPostOfficeId,
    Guid DestinationPostOfficeId);

public  record UpdateShipmentStatusRequest(
    ShipmentStatus Status,
    Guid PostOfficeId);

public  record ShipmentFilter(
    ShipmentStatus? Status,
    Guid? LocationPostOfficeId,
    WeightCategory? Weight,
    string? ShipmentNumber,
    string? ShipmentType);
