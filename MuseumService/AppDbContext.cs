using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MuseumService.Models;

// AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Score> Scores { get; set; }
    public DbSet<Exhibit> Exhibits { get; set; }
    public DbSet<ExhibitImage> ExhibitImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация для ExhibitImage
        modelBuilder.Entity<ExhibitImage>()
            .HasKey(ei => ei.ImageId); // Явное указание первичного ключа

        modelBuilder.Entity<ExhibitImage>()
            .HasOne(ei => ei.Exhibit)
            .WithMany(e => e.Images)
            .HasForeignKey(ei => ei.ExhibitId)
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление

        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => v
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(utcConverter);
                }
            }
        }
    }
}