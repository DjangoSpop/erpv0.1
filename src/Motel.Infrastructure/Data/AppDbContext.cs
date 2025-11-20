using Microsoft.EntityFrameworkCore;
using Motel.Domain.Common;
using Motel.Domain.Entities;
using Motel.Domain.Enums;

namespace Motel.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Automatically populate audit fields before saving changes
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;
        // TODO: Get actual user from HTTP context
        var currentUser = "System";

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.CreatedBy = currentUser;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAtUtc = now;
                    entry.Entity.ModifiedBy = currentUser;
                    break;

                case EntityState.Deleted:
                    // Implement soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.ModifiedAtUtc = now;
                    entry.Entity.ModifiedBy = currentUser;
                    break;
            }
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Motel.Domain.Exceptions.ConcurrencyException();
        }
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client configuration
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Phone);
            entity.HasIndex(e => e.NationalIdOrPassport);
        });

        // Room configuration
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Number).IsUnique();
            entity.Property(e => e.BaseNightlyRate).HasPrecision(18, 2);
        });

        // Reservation configuration
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoomId, e.CheckInDate, e.CheckOutDate });
            entity.Property(e => e.NightlyRate).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.ExtraCharges).HasPrecision(18, 2);

            entity.HasOne(e => e.Client)
                .WithMany(c => c.Reservations)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Room)
                .WithMany(r => r.Reservations)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Serial).IsUnique();
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxPercent).HasPrecision(5, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);

            entity.HasOne(e => e.Reservation)
                .WithOne(r => r.Invoice)
                .HasForeignKey<Invoice>(e => e.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // InvoiceLineItem configuration
        modelBuilder.Entity<InvoiceLineItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.DiscountPercent).HasPrecision(5, 2);
            entity.Property(e => e.LineTotal).HasPrecision(18, 2);

            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.LineItems)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // NotificationLog configuration
        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReservationId);
            entity.HasIndex(e => e.SentAtUtc);
        });

        // Configure global query filters for soft delete
        modelBuilder.Entity<Client>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Room>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Reservation>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Invoice>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<InvoiceLineItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<NotificationLog>().HasQueryFilter(e => !e.IsDeleted);

        // Configure concurrency tokens for all entities
        modelBuilder.Entity<Client>().Property(e => e.RowVersion).IsRowVersion();
        modelBuilder.Entity<Room>().Property(e => e.RowVersion).IsRowVersion();
        modelBuilder.Entity<Reservation>().Property(e => e.RowVersion).IsRowVersion();
        modelBuilder.Entity<Invoice>().Property(e => e.RowVersion).IsRowVersion();
        modelBuilder.Entity<InvoiceLineItem>().Property(e => e.RowVersion).IsRowVersion();
        modelBuilder.Entity<NotificationLog>().Property(e => e.RowVersion).IsRowVersion();

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seed Rooms
        var rooms = new List<Room>
        {
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Number = "101", Type = RoomType.Single, BaseNightlyRate = 300, Capacity = 1, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Number = "102", Type = RoomType.Double, BaseNightlyRate = 450, Capacity = 2, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Number = "103", Type = RoomType.Twin, BaseNightlyRate = 500, Capacity = 2, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Number = "104", Type = RoomType.Triple, BaseNightlyRate = 600, Capacity = 3, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Number = "105", Type = RoomType.Family, BaseNightlyRate = 800, Capacity = 4, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Number = "201", Type = RoomType.Dorm, BaseNightlyRate = 150, Capacity = 6, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Number = "202", Type = RoomType.Chalet, BaseNightlyRate = 1200, Capacity = 5, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Number = "203", Type = RoomType.Double, BaseNightlyRate = 450, Capacity = 2, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Number = "204", Type = RoomType.Single, BaseNightlyRate = 300, Capacity = 1, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Number = "205", Type = RoomType.Triple, BaseNightlyRate = 600, Capacity = 3, Status = RoomStatus.Available, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false }
        };

        modelBuilder.Entity<Room>().HasData(rooms);

        // Seed Clients
        var clients = new List<Client>
        {
            new() { Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"), FullName = "أحمد محمد علي", NationalIdOrPassport = "29012345678901", Phone = "01012345678", Email = "ahmed@example.com", CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"), FullName = "فاطمة حسن", NationalIdOrPassport = "29112345678902", Phone = "01023456789", Email = "fatma@example.com", CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false },
            new() { Id = Guid.Parse("c3333333-3333-3333-3333-333333333333"), FullName = "محمود سعيد", NationalIdOrPassport = "A1234567", Phone = "01098765432", Email = null, CreatedAtUtc = seedDate, CreatedBy = "System", IsDeleted = false }
        };

        modelBuilder.Entity<Client>().HasData(clients);
    }
}
