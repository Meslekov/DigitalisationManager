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
            Guid managerUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            Guid regularUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            
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
                UserName = "admin@admin.local",
                NormalizedUserName = "ADMIN@ADMIN.LOCAL",
                Email = "admin@admin.local",
                NormalizedEmail = "ADMIN@ADMIN.LOCAL",
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

            ApplicationUser managerUser = new ApplicationUser
            {
                Id = managerUserId,
                UserName = "manager@manager.com",
                NormalizedUserName = "MANAGER@MANAGER.COM",
                Email = "manager@manager.com",
                NormalizedEmail = "MANAGER@MANAGER.COM",
                EmailConfirmed = true,
                FirstName = "Archive",
                LastName = "Manager",
                SecurityStamp = "manager-security-stamp",
                ConcurrencyStamp = "manager-concurrency-stamp",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAIAAYagAAAAENIhe5FuMqz0sI+E1B6wCCCwsHQdHjtB3Bzd+6z4sSq1JturIcp3a6rUOMExG0IKtQ=="
            };

            ApplicationUser regularUser = new ApplicationUser
            {
                Id = regularUserId,
                UserName = "user@user.com",
                NormalizedUserName = "USER@USER.COM",
                Email = "user@user.com",
                NormalizedEmail = "USER@USER.COM",
                EmailConfirmed = true,
                FirstName = "Archive",
                LastName = "User",
                SecurityStamp = "user-security-stamp",
                ConcurrencyStamp = "user-concurrency-stamp",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAIAAYagAAAAEFxFJP1QxDchH+Mj3Zuae8NAlh+3k54Lktrxp2gRRl7EXW44v32IdlDWwsj4Gm+qJw=="
            };

            builder.Entity<ApplicationUser>().HasData(adminUser, managerUser, regularUser);

            builder.Entity<IdentityUserRole<Guid>>().HasData(
                    new IdentityUserRole<Guid>
                    {
                        UserId = adminUserId,
                        RoleId = administratorRoleId
                    },
                    new IdentityUserRole<Guid>
                    {
                        UserId = managerUserId,
                        RoleId = managerRoleId
                    },
                    new IdentityUserRole<Guid>
                    {
                        UserId = regularUserId,
                        RoleId = userRoleId
                    });


            builder.Entity<Fund>().HasData(
                    new Fund
                    {
                        Id = 1,
                        FundType = FundType.ArchiveFund,
                        Code = "AF-001",
                        Title = "Municipal Administration Archive",
                        Description = "Administrative documents from the municipal archive.",
                        CreatedAt = new DateTime(2026, 2, 13, 8, 0, 0, DateTimeKind.Utc)
                    },
                    new Fund
                    {
                        Id = 2,
                        FundType = FundType.PhotoFund,
                        Code = "PF-001",
                        Title = "Historical Photo Collection",
                        Description = "Photographic materials from local history collections.",
                        CreatedAt = new DateTime(2026, 2, 13, 8, 5, 0, DateTimeKind.Utc)
                    },
                    new Fund
                    {
                        Id = 3,
                        FundType = FundType.ArchiveFund,
                        Code = "AF-002",
                        Title = "School Documentation Archive",
                        Description = "Administrative and educational archival records.",
                        CreatedAt = new DateTime(2026, 2, 13, 8, 10, 0, DateTimeKind.Utc)
                    },
                    new Fund
                    {
                        Id = 4,
                        FundType = FundType.PhotoFund,
                        Code = "PF-002",
                        Title = "Cultural Events Photo Archive",
                        Description = "Images from public events, festivals, and exhibitions.",
                        CreatedAt = new DateTime(2026, 2, 13, 8, 15, 0, DateTimeKind.Utc)
                    },
                    new Fund
                    {
                        Id = 5,
                        FundType = FundType.ArchiveFund,
                        Code = "AF-003",
                        Title = "Regional Correspondence Archive",
                        Description = "Letters and official communication records.",
                        CreatedAt = new DateTime(2026, 2, 13, 8, 20, 0, DateTimeKind.Utc)
                    },
                    new Fund
                    {
                        Id = 6,
                        FundType = FundType.PhotoFund,
                        Code = "PF-003",
                        Title = "Museum Visual Collection",
                        Description = "Museum photographs, exhibition visuals, and related materials.",
                        CreatedAt = new DateTime(2026, 2, 13, 8, 25, 0, DateTimeKind.Utc)
                    });

            builder.Entity<Category>().HasData(
                    new Category
                    {
                        Id = 1,
                        Name = "Administrative Records",
                        Description = "General administrative and institutional documents.",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 2, 13, 9, 0, 0, DateTimeKind.Utc)
                    },
                    new Category
                    {
                        Id = 2,
                        Name = "Correspondence",
                        Description = "Letters, notices, and official communication.",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 2, 13, 9, 5, 0, DateTimeKind.Utc)
                    },
                    new Category
                    {
                        Id = 3,
                        Name = "Photographic Materials",
                        Description = "Historical and documentary photographic materials.",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 2, 13, 9, 10, 0, DateTimeKind.Utc)
                    },
                    new Category
                    {
                        Id = 4,
                        Name = "Registers",
                        Description = "Registers, ledgers, and record books.",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 2, 13, 9, 15, 0, DateTimeKind.Utc)
                    },
                    new Category
                    {
                        Id = 5,
                        Name = "Exhibition Materials",
                        Description = "Posters, brochures, invitations, and event materials.",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 2, 13, 9, 20, 0, DateTimeKind.Utc)
                    });

            builder.Entity<Item>().HasData(
                    new Item { Id = 1, FundId = 1, CategoryId = 1, InventoryNumber = "INV-001", Description = "Municipal council annual report", DocumentDateText = "1948", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 0, 0, DateTimeKind.Utc) },
                    new Item { Id = 2, FundId = 1, CategoryId = 2, InventoryNumber = "INV-002", Description = "Official correspondence with regional authorities", DocumentDateText = "1951", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 5, 0, DateTimeKind.Utc) },
                    new Item { Id = 3, FundId = 1, CategoryId = 4, InventoryNumber = "INV-003", Description = "Registry of municipal decisions", DocumentDateText = "1955", Status = ItemStatus.InProgress, CreatedAt = new DateTime(2026, 2, 14, 8, 10, 0, DateTimeKind.Utc) },
                    new Item { Id = 4, FundId = 1, CategoryId = 1, InventoryNumber = "INV-004", Description = "Administrative budget summary", DocumentDateText = "1960", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 15, 0, DateTimeKind.Utc) },
                 
                    new Item { Id = 5, FundId = 2, CategoryId = 3, InventoryNumber = "INV-005", Description = "City square historical photograph", DocumentDateText = "1932", Status = ItemStatus.Digitized, CreatedAt = new DateTime(2026, 2, 14, 8, 20, 0, DateTimeKind.Utc) },
                    new Item { Id = 6, FundId = 2, CategoryId = 3, InventoryNumber = "INV-006", Description = "Street life photography collection", DocumentDateText = "1940", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 25, 0, DateTimeKind.Utc) },
                    new Item { Id = 7, FundId = 2, CategoryId = 5, InventoryNumber = "INV-007", Description = "Exhibition invitation photograph set", DocumentDateText = "1957", Status = ItemStatus.InProgress, CreatedAt = new DateTime(2026, 2, 14, 8, 30, 0, DateTimeKind.Utc) },
                    new Item { Id = 8, FundId = 2, CategoryId = 3, InventoryNumber = "INV-008", Description = "Railway station archival photo", DocumentDateText = "1964", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 35, 0, DateTimeKind.Utc) },
                 
                    new Item { Id = 9, FundId = 3, CategoryId = 1, InventoryNumber = "INV-009", Description = "School administration annual register", DocumentDateText = "1971", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 40, 0, DateTimeKind.Utc) },
                    new Item { Id = 10, FundId = 3, CategoryId = 4, InventoryNumber = "INV-010", Description = "Student attendance register", DocumentDateText = "1972", Status = ItemStatus.InProgress, CreatedAt = new DateTime(2026, 2, 14, 8, 45, 0, DateTimeKind.Utc) },
                    new Item { Id = 11, FundId = 3, CategoryId = 2, InventoryNumber = "INV-011", Description = "Correspondence between schools", DocumentDateText = "1974", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 50, 0, DateTimeKind.Utc) },
                    new Item { Id = 12, FundId = 3, CategoryId = 1, InventoryNumber = "INV-012", Description = "Regional education report", DocumentDateText = "1976", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 8, 55, 0, DateTimeKind.Utc) },
                 
                    new Item { Id = 13, FundId = 4, CategoryId = 3, InventoryNumber = "INV-013", Description = "Festival opening ceremony photo", DocumentDateText = "1981", Status = ItemStatus.Digitized, CreatedAt = new DateTime(2026, 2, 14, 9, 0, 0, DateTimeKind.Utc) },
                    new Item { Id = 14, FundId = 4, CategoryId = 5, InventoryNumber = "INV-014", Description = "Exhibition poster materials", DocumentDateText = "1983", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 9, 5, 0, DateTimeKind.Utc) },
                    new Item { Id = 15, FundId = 4, CategoryId = 3, InventoryNumber = "INV-015", Description = "Community event photography", DocumentDateText = "1986", Status = ItemStatus.InProgress, CreatedAt = new DateTime(2026, 2, 14, 9, 10, 0, DateTimeKind.Utc) },
                    new Item { Id = 16, FundId = 4, CategoryId = 5, InventoryNumber = "INV-016", Description = "Cultural program leaflet", DocumentDateText = "1988", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 9, 15, 0, DateTimeKind.Utc) },
                 
                    new Item { Id = 17, FundId = 5, CategoryId = 2, InventoryNumber = "INV-017", Description = "Official regional correspondence file", DocumentDateText = "1990", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 9, 20, 0, DateTimeKind.Utc) },
                    new Item { Id = 18, FundId = 5, CategoryId = 2, InventoryNumber = "INV-018", Description = "Interdepartmental letter archive", DocumentDateText = "1991", Status = ItemStatus.InProgress, CreatedAt = new DateTime(2026, 2, 14, 9, 25, 0, DateTimeKind.Utc) },
                    new Item { Id = 19, FundId = 5, CategoryId = 1, InventoryNumber = "INV-019", Description = "Regional administrative notice", DocumentDateText = "1993", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 9, 30, 0, DateTimeKind.Utc) },
                    new Item { Id = 20, FundId = 5, CategoryId = 4, InventoryNumber = "INV-020", Description = "Correspondence register ledger", DocumentDateText = "1995", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 9, 35, 0, DateTimeKind.Utc) },
                 
                    new Item { Id = 21, FundId = 6, CategoryId = 3, InventoryNumber = "INV-021", Description = "Museum interior visual archive", DocumentDateText = "2001", Status = ItemStatus.Digitized, CreatedAt = new DateTime(2026, 2, 14, 9, 40, 0, DateTimeKind.Utc) },
                    new Item { Id = 22, FundId = 6, CategoryId = 3, InventoryNumber = "INV-022", Description = "Artifact display photography", DocumentDateText = "2004", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 9, 45, 0, DateTimeKind.Utc) },
                    new Item { Id = 23, FundId = 6, CategoryId = 5, InventoryNumber = "INV-023", Description = "Exhibition brochure archive", DocumentDateText = "2008", Status = ItemStatus.InProgress, CreatedAt = new DateTime(2026, 2, 14, 9, 50, 0, DateTimeKind.Utc) },
                    new Item { Id = 24, FundId = 6, CategoryId = 3, InventoryNumber = "INV-024", Description = "Temporary exhibition visuals", DocumentDateText = "2012", Status = ItemStatus.New, CreatedAt = new DateTime(2026, 2, 14, 9, 55, 0, DateTimeKind.Utc) }
                  );

        }
        }
    }


