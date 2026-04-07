namespace DigitalisationManager.Data
{
    using DigitalisationManager.Data.Models.Entities;
    using DigitalisationManager.Data.Models.Identity;
    using DigitalisationManager.GCommon;
    using DigitalisationManager.GCommon.Enums;
    using Microsoft.AspNetCore.Identity;
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
            public DbSet<Category> Categories { get; set; } = null!;
            public DbSet<ItemHistory> ItemHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

                builder.Entity<Item>()
                       .HasOne(i => i.Fund)
                       .WithMany(f => f.Items)
                       .HasForeignKey(i => i.FundId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>()
                       .HasOne(i => i.Category)
                       .WithMany(c => c.Items)
                       .HasForeignKey(i => i.CategoryId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<DigitalFile>()
                       .HasOne(df => df.Item)
                       .WithMany(i => i.DigitalFiles)
                       .HasForeignKey(df => df.ItemId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<ItemHistory>()
                       .HasOne(ih => ih.Item)
                       .WithMany(i => i.ItemHistories)
                       .HasForeignKey(ih => ih.ItemId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.Entity<DigitalFile>()
                       .HasIndex(df => new { df.ItemId, df.OriginalStoredFileName })
                       .IsUnique();

                builder.Entity<DigitalFile>()
                       .HasIndex(df => df.OriginalChecksumSha256);
                
                builder.Entity<Item>()
                       .HasIndex(i => new { i.FundId, i.InventoryNumber })
                       .IsUnique();

                builder.Entity<Category>()
                       .HasIndex(c => c.Name)
                       .IsUnique();
               
                builder.Entity<ItemHistory>()
                       .HasIndex(ih => new { ih.ItemId, ih.CreatedAt });

                Guid administratorRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                Guid userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                Guid managerRoleId = Guid.Parse("99999999-9999-9999-9999-999999999999");
                Guid adminUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
                
                ApplicationRole administratorRole = new ApplicationRole
                {
                    Id = administratorRoleId,
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Label = "Administrator",
                    ConcurrencyStamp = "44444444-4444-4444-4444-444444444444"
                };
                
                ApplicationRole userRole = new ApplicationRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER",
                    Label = "User",
                    ConcurrencyStamp = "55555555-5555-5555-5555-555555555555"
                };

                ApplicationRole managerRole = new ApplicationRole
                {
                    Id = managerRoleId,
                    Name = ApplicationConstants.RoleNames.Manager,
                    NormalizedName = ApplicationConstants.RoleNames.Manager.ToUpperInvariant(),
                    Label = "Manager",
                    ConcurrencyStamp = "88888888-8888-8888-8888-888888888888"
                };

            builder.Entity<ApplicationRole>().HasData(administratorRole, userRole, managerRole);

            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@digitalisationmanager.local",
                NormalizedUserName = "ADMIN@DIGITALISATIONMANAGER.LOCAL",
                Email = "admin@digitalisationmanager.local",
                NormalizedEmail = "ADMIN@DIGITALISATIONMANAGER.LOCAL",
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                SecurityStamp = "66666666-6666-6666-6666-666666666666",
                ConcurrencyStamp = "77777777-7777-7777-7777-777777777777",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAIAAYagAAAAEJppDSosLe9PSUf4y/CwEcXtNPrwJxBFBtvTU7MqRSMVnEh4AjnV/UmXREB1jwelWA=="
            };

            builder.Entity<ApplicationUser>().HasData(adminUser);
                
                builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
                {
                    UserId = adminUserId,
                    RoleId = administratorRoleId
                });
                
                
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

                builder.Entity<Category>().HasData(
                        new Category
                        {
                            Id = 1,
                            Name = "Administrative Records",
                            Description = "General administrative and institutional documents.",
                            IsActive = true,
                            CreatedAt = new DateTime(2026, 02, 13, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new Category
                        {
                            Id = 2,
                            Name = "Correspondence",
                            Description = "Letters, official communication, and related materials.",
                            IsActive = true,
                            CreatedAt = new DateTime(2026, 02, 13, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new Category
                        {
                            Id = 3,
                            Name = "Photographic Materials",
                            Description = "Photographs, scans, negatives, and related visual assets.",
                            IsActive = true,
                            CreatedAt = new DateTime(2026, 02, 13, 0, 0, 0, DateTimeKind.Utc)
                        }
                );

        }
        }
    }


