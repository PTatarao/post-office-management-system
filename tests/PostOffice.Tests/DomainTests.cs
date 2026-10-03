using PostOffice.Domain.Entities;
using Xunit;

namespace PostOffice.Tests;

public class DomainTests
{
    [Fact]
    public void WeightCategory_IsLessThanOneKg()
    {
        var shipment = new Package("PKG-1", 0.5m, Guid.NewGuid(), Guid.NewGuid());
        Assert.Equal(WeightCategory.LessThan1Kg, shipment.WeightCategory);
    }

    [Fact]
    public void WeightCategory_IsBetweenOneAndFiveKg()
    {
        var shipment = new Package("PKG-1", 5m, Guid.NewGuid(), Guid.NewGuid());
        Assert.Equal(WeightCategory.Between1And5Kg, shipment.WeightCategory);
    }

    [Fact]
    public void WeightCategory_IsMoreThanFiveKg()
    {
        var shipment = new Package("PKG-1", 5.01m, Guid.NewGuid(), Guid.NewGuid());
        Assert.Equal(WeightCategory.MoreThan5Kg, shipment.WeightCategory);
    }

    [Fact]
    public void NewShipment_StartsAtOrigin()
    {
        var origin = Guid.NewGuid();
        var destination = Guid.NewGuid();

        var shipment = new Package("PKG-1", 2m, origin, destination);

        Assert.Equal(origin, shipment.CurrentPostOfficeId);
        Assert.Equal(ShipmentStatus.ReceivedAtOrigin, shipment.Status);
    }

    [Fact]
    public void DestinationStatus_CannotBeSetAtWrongPostOffice()
    {
        var destination = Guid.NewGuid();
        var shipment = new Package("PKG-1", 2m, Guid.NewGuid(), destination);

        Assert.Throws<InvalidOperationException>(() =>
            shipment.MoveTo(Guid.NewGuid(), ShipmentStatus.ReceivedAtDestination));
    }
}
