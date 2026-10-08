namespace PostOffice.Domain.Entities;

public class Shipment
{

    public Shipment(string shipmentNumber, decimal weightKg, Guid originPostOfficeId, Guid destinationPostOfficeId)
    {
        Id = Guid.NewGuid();
        ShipmentNumber = shipmentNumber;
        SetWeight(weightKg);
        OriginPostOfficeId = originPostOfficeId;
        DestinationPostOfficeId = destinationPostOfficeId;
        CurrentPostOfficeId = originPostOfficeId;
        CreatedAtUtc = DateTime.UtcNow;
        Status = ShipmentStatus.ReceivedAtOrigin;
    }

    public Guid Id { get; private set; }
    public string ShipmentNumber { get; private set; } = null!;
    public decimal WeightKg { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public Guid OriginPostOfficeId { get; private set; }
    public Guid DestinationPostOfficeId { get; private set; }
    public Guid CurrentPostOfficeId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public PostOffice OriginPostOffice { get; private set; } = null!;
    public PostOffice DestinationPostOffice { get; private set; } = null!;
    public PostOffice CurrentPostOffice { get; private set; } = null!;
    public ICollection<ShipmentStatusHistory> StatusHistory { get; private set; } = new List<ShipmentStatusHistory>();

    public WeightCategory WeightCategory => WeightCategoryExtensions.From(WeightKg);

    public void Update(decimal weightKg, Guid originPostOfficeId, Guid destinationPostOfficeId)
    {
        SetWeight(weightKg);
        OriginPostOfficeId = originPostOfficeId;
        DestinationPostOfficeId = destinationPostOfficeId;
    }

    public void MoveTo(Guid postOfficeId, ShipmentStatus newStatus)
    {
        if (newStatus == ShipmentStatus.ReceivedAtDestination && postOfficeId != DestinationPostOfficeId)
            throw new InvalidOperationException("Destination status can only be set at the destination post office.");

        if (newStatus == ShipmentStatus.Delivered && postOfficeId != DestinationPostOfficeId)
            throw new InvalidOperationException("Shipment can only be delivered at the destination post office.");

        CurrentPostOfficeId = postOfficeId;
        Status = newStatus;
        StatusHistory.Add(new ShipmentStatusHistory(Id, newStatus, postOfficeId, DateTime.UtcNow));
    }

    private void SetWeight(decimal weightKg)
    {
        if (weightKg <= 0) throw new ArgumentOutOfRangeException(nameof(weightKg), "Weight must be greater than zero.");
        WeightKg = weightKg;
    }
}

public sealed class Letter : Shipment
{
    public Letter(string shipmentNumber, decimal weightKg, Guid originPostOfficeId, Guid destinationPostOfficeId)
        : base(shipmentNumber, weightKg, originPostOfficeId, destinationPostOfficeId) { }
}

public sealed class Package : Shipment
{
    public Package(string shipmentNumber, decimal weightKg, Guid originPostOfficeId, Guid destinationPostOfficeId)
        : base(shipmentNumber, weightKg, originPostOfficeId, destinationPostOfficeId) { }
}
