using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DigitalisationManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImproveDemoSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "admin@admin.local", "ADMIN@ADMIN.LOCAL", "ADMIN@ADMIN.LOCAL", "admin@admin.local" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 0, "manager-concurrency-stamp", "manager@manager.com", true, "Archive", "Manager", false, null, "MANAGER@MANAGER.COM", "MANAGER@MANAGER.COM", "AQAAAAIAAYagAAAAENIhe5FuMqz0sI+E1B6wCCCwsHQdHjtB3Bzd+6z4sSq1JturIcp3a6rUOMExG0IKtQ==", null, false, "manager-security-stamp", false, "manager@manager.com" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 0, "user-concurrency-stamp", "user@user.com", true, "Archive", "User", false, null, "USER@USER.COM", "USER@USER.COM", "AQAAAAIAAYagAAAAEFxFJP1QxDchH+Mj3Zuae8NAlh+3k54Lktrxp2gRRl7EXW44v32IdlDWwsj4Gm+qJw==", null, false, "user-security-stamp", false, "user@user.com" }
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 13, 9, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 13, 9, 5, 0, 0, DateTimeKind.Utc), "Letters, notices, and official communication." });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 13, 9, 10, 0, 0, DateTimeKind.Utc), "Historical and documentary photographic materials." });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 4, new DateTime(2026, 2, 13, 9, 15, 0, 0, DateTimeKind.Utc), "Registers, ledgers, and record books.", true, "Registers" },
                    { 5, new DateTime(2026, 2, 13, 9, 20, 0, 0, DateTimeKind.Utc), "Posters, brochures, invitations, and event materials.", true, "Exhibition Materials" }
                });

            migrationBuilder.UpdateData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Title" },
                values: new object[] { new DateTime(2026, 2, 13, 8, 0, 0, 0, DateTimeKind.Utc), "Administrative documents from the municipal archive.", "Municipal Administration Archive" });

            migrationBuilder.UpdateData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Title" },
                values: new object[] { new DateTime(2026, 2, 13, 8, 5, 0, 0, DateTimeKind.Utc), "Photographic materials from local history collections.", "Historical Photo Collection" });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "FundType", "Title" },
                values: new object[,]
                {
                    { 3, "AF-002", new DateTime(2026, 2, 13, 8, 10, 0, 0, DateTimeKind.Utc), "Administrative and educational archival records.", 0, "School Documentation Archive" },
                    { 4, "PF-002", new DateTime(2026, 2, 13, 8, 15, 0, 0, DateTimeKind.Utc), "Images from public events, festivals, and exhibitions.", 2, "Cultural Events Photo Archive" },
                    { 5, "AF-003", new DateTime(2026, 2, 13, 8, 20, 0, 0, DateTimeKind.Utc), "Letters and official communication records.", 0, "Regional Correspondence Archive" },
                    { 6, "PF-003", new DateTime(2026, 2, 13, 8, 25, 0, 0, DateTimeKind.Utc), "Museum photographs, exhibition visuals, and related materials.", 2, "Museum Visual Collection" }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DocumentDateText", "FundId", "InventoryNumber", "Status" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 2, 14, 8, 0, 0, 0, DateTimeKind.Utc), "Municipal council annual report", "1948", 1, "INV-001", 0 },
                    { 2, 2, new DateTime(2026, 2, 14, 8, 5, 0, 0, DateTimeKind.Utc), "Official correspondence with regional authorities", "1951", 1, "INV-002", 0 },
                    { 4, 1, new DateTime(2026, 2, 14, 8, 15, 0, 0, DateTimeKind.Utc), "Administrative budget summary", "1960", 1, "INV-004", 0 },
                    { 5, 3, new DateTime(2026, 2, 14, 8, 20, 0, 0, DateTimeKind.Utc), "City square historical photograph", "1932", 2, "INV-005", 2 },
                    { 6, 3, new DateTime(2026, 2, 14, 8, 25, 0, 0, DateTimeKind.Utc), "Street life photography collection", "1940", 2, "INV-006", 0 },
                    { 8, 3, new DateTime(2026, 2, 14, 8, 35, 0, 0, DateTimeKind.Utc), "Railway station archival photo", "1964", 2, "INV-008", 0 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DocumentDateText", "FundId", "InventoryNumber", "Status" },
                values: new object[,]
                {
                    { 3, 4, new DateTime(2026, 2, 14, 8, 10, 0, 0, DateTimeKind.Utc), "Registry of municipal decisions", "1955", 1, "INV-003", 1 },
                    { 7, 5, new DateTime(2026, 2, 14, 8, 30, 0, 0, DateTimeKind.Utc), "Exhibition invitation photograph set", "1957", 2, "INV-007", 1 },
                    { 9, 1, new DateTime(2026, 2, 14, 8, 40, 0, 0, DateTimeKind.Utc), "School administration annual register", "1971", 3, "INV-009", 0 },
                    { 10, 4, new DateTime(2026, 2, 14, 8, 45, 0, 0, DateTimeKind.Utc), "Student attendance register", "1972", 3, "INV-010", 1 },
                    { 11, 2, new DateTime(2026, 2, 14, 8, 50, 0, 0, DateTimeKind.Utc), "Correspondence between schools", "1974", 3, "INV-011", 0 },
                    { 12, 1, new DateTime(2026, 2, 14, 8, 55, 0, 0, DateTimeKind.Utc), "Regional education report", "1976", 3, "INV-012", 0 },
                    { 13, 3, new DateTime(2026, 2, 14, 9, 0, 0, 0, DateTimeKind.Utc), "Festival opening ceremony photo", "1981", 4, "INV-013", 2 },
                    { 14, 5, new DateTime(2026, 2, 14, 9, 5, 0, 0, DateTimeKind.Utc), "Exhibition poster materials", "1983", 4, "INV-014", 0 },
                    { 15, 3, new DateTime(2026, 2, 14, 9, 10, 0, 0, DateTimeKind.Utc), "Community event photography", "1986", 4, "INV-015", 1 },
                    { 16, 5, new DateTime(2026, 2, 14, 9, 15, 0, 0, DateTimeKind.Utc), "Cultural program leaflet", "1988", 4, "INV-016", 0 },
                    { 17, 2, new DateTime(2026, 2, 14, 9, 20, 0, 0, DateTimeKind.Utc), "Official regional correspondence file", "1990", 5, "INV-017", 0 },
                    { 18, 2, new DateTime(2026, 2, 14, 9, 25, 0, 0, DateTimeKind.Utc), "Interdepartmental letter archive", "1991", 5, "INV-018", 1 },
                    { 19, 1, new DateTime(2026, 2, 14, 9, 30, 0, 0, DateTimeKind.Utc), "Regional administrative notice", "1993", 5, "INV-019", 0 },
                    { 20, 4, new DateTime(2026, 2, 14, 9, 35, 0, 0, DateTimeKind.Utc), "Correspondence register ledger", "1995", 5, "INV-020", 0 },
                    { 21, 3, new DateTime(2026, 2, 14, 9, 40, 0, 0, DateTimeKind.Utc), "Museum interior visual archive", "2001", 6, "INV-021", 2 },
                    { 22, 3, new DateTime(2026, 2, 14, 9, 45, 0, 0, DateTimeKind.Utc), "Artifact display photography", "2004", 6, "INV-022", 0 },
                    { 23, 5, new DateTime(2026, 2, 14, 9, 50, 0, 0, DateTimeKind.Utc), "Exhibition brochure archive", "2008", 6, "INV-023", 1 },
                    { 24, 3, new DateTime(2026, 2, 14, 9, 55, 0, 0, DateTimeKind.Utc), "Temporary exhibition visuals", "2012", 6, "INV-024", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("99999999-9999-9999-9999-999999999999"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "admin@digitalisationmanager.local", "ADMIN@DIGITALISATIONMANAGER.LOCAL", "ADMIN@DIGITALISATIONMANAGER.LOCAL", "admin@digitalisationmanager.local" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Letters, official communication, and related materials." });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Photographs, scans, negatives, and related visual assets." });

            migrationBuilder.UpdateData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Title" },
                values: new object[] { new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded demo fund (remove if you want empty DB).", "Archive Fund 001" });

            migrationBuilder.UpdateData(
                table: "Funds",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Title" },
                values: new object[] { new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded demo fund (remove if you want empty DB).", "Photo Fund 001" });
        }
    }
}
