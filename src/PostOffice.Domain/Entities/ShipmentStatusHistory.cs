namespace PostOffice.Domain.Entities;

public sealed class ShipmentStatusHistory
{
    private ShipmentStatusHistory() { }

    public ShipmentStatusHistory(Guid shipmentId, ShipmentStatus status, Guid postOfficeId, DateTime changedAtUtc)
    {
        Id = Guid.NewGuid();
        ShipmentId = shipmentId;
        Status = status;
        PostOfficeId = postOfficeId;
        ChangedAtUtc = changedAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid ShipmentId { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public Guid PostOfficeId { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }
}
