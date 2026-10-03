using Microsoft.EntityFrameworkCore;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Infrastructure.Persistence;

public sealed class PostOfficeDbContext(DbContextOptions<PostOfficeDbContext> options) : DbContext(options)
{
    public DbSet<PostOfficeEntity> PostOffices => Set<PostOfficeEntity>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Letter> Letters => Set<Letter>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistory => Set<ShipmentStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>().ToTable("shipments");
        modelBuilder.Entity<Letter>().ToTable("letters");
        modelBuilder.Entity<Package>().ToTable("packages");

        modelBuilder.Entity<Shipment>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.ShipmentNumber).IsUnique();
            b.Property(x => x.ShipmentNumber).HasMaxLength(50).IsRequired();
            b.Property(x => x.WeightKg).HasPrecision(10, 3);
            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            b.HasOne(x => x.OriginPostOffice).WithMany(x => x.OriginShipments)
                .HasForeignKey(x => x.OriginPostOfficeId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.DestinationPostOffice).WithMany(x => x.DestinationShipments)
                .HasForeignKey(x => x.DestinationPostOfficeId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.CurrentPostOffice).WithMany(x => x.CurrentShipments)
                .HasForeignKey(x => x.CurrentPostOfficeId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.StatusHistory).WithOne().HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostOfficeEntity>(b =>
        {
            b.ToTable("post_offices");
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.ZipCode).IsUnique();
            b.Property(x => x.ZipCode).HasMaxLength(20).IsRequired();
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.City).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ShipmentStatusHistory>(b =>
        {
            b.ToTable("shipment_status_history");
            b.HasKey(x => x.Id);
            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            b.HasIndex(x => new { x.ShipmentId, x.ChangedAtUtc });
        });
    }
}
