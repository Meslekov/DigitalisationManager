namespace DigitalisationManager.Data
{
    using DigitalisationManager.Data.Models.Entities;
    using DigitalisationManager.Data.Models.Identity;
    using DigitalisationManager.GCommon.Enums;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DigitalisationManagerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DigitalisationManagerDbContext(DbContextOptions<DigitalisationManagerDbContext> options)
             : base(options)
        {  
        }

            public DbSet<Fund> Funds { get; set; } = null!;
            public DbSet<Item> Items { get; set; } = null!;
            public DbSet<DigitalFile> DigitalFiles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

                builder.Entity<Item>()
                       .HasOne(i => i.Fund)
                       .WithMany(f => f.Items)
                       .HasForeignKey(i => i.FundId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<DigitalFile>()
                       .HasOne(df => df.Item)
                       .WithMany(i => i.DigitalFiles)
                       .HasForeignKey(df => df.ItemId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>()
                       .HasIndex(i => new { i.FundId, i.InventoryNumber })
                       .IsUnique();

                builder.Entity<Fund>().HasData(
                    new Fund
                    {
                        Id = 1,
                        FundType = FundType.ArchiveFund,
                        Code = "AF-001",
                        Title = "Archive Fund 001",
                        Description = "Seeded demo fund (remove if you want empty DB).",
                        CreatedAt = new DateTime(2026, 02, 13, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new Fund
                    {
                        Id = 2,
                        FundType = FundType.PhotoFund,
                        Code = "PF-001",
                        Title = "Photo Fund 001",
                        Description = "Seeded demo fund (remove if you want empty DB).",
                        CreatedAt = new DateTime(2026, 02, 13, 0, 0, 0, DateTimeKind.Utc)
                    }
                );
            }
        }
    }


