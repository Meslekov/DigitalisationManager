namespace DigitalisationManager.Data
{
    using DigitalisationManager.Data.Models.Entities;
    using DigitalisationManager.GCommon.Enums;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DigitalisationManagerDbContext : IdentityDbContext
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

               
                var defaultUser = new IdentityUser
                {
                    Id = "9a3d2b6e-3f7a-4c8c-9b4b-6c5bdfaa1111",
                    UserName = "admin@digitalisationmanager.local",
                    NormalizedUserName = "ADMIN@DIGITALISATIONMANAGER.LOCAL",
                    Email = "admin@digitalisationmanager.local",
                    NormalizedEmail = "ADMIN@DIGITALISATIONMANAGER.LOCAL",
                    EmailConfirmed = true,
                    SecurityStamp = "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d",
                    ConcurrencyStamp = "564e41d9-da72-4812-a5cd-6cd8743ee292",
                    PasswordHash = "AQAAAAIAAYagAAAAEJppDSosLe9PSUf4y/CwEcXtNPrwJxBFBtvTU7MqRSMVnEh4AjnV/UmXREB1jwelWA=="
                };

                builder.Entity<IdentityUser>().HasData(defaultUser);

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


