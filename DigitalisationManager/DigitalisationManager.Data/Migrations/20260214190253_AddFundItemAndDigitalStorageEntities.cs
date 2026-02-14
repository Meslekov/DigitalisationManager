using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DigitalisationManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFundItemAndDigitalStorageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundType = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentDateText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FundId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DigitalFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RelativePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChecksumSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalFiles_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "9a3d2b6e-3f7a-4c8c-9b4b-6c5bdfaa1111", 0, "564e41d9-da72-4812-a5cd-6cd8743ee292", "admin@digitalisationmanager.local", true, false, null, "ADMIN@DIGITALISATIONMANAGER.LOCAL", "ADMIN@DIGITALISATIONMANAGER.LOCAL", "AQAAAAIAAYagAAAAEJppDSosLe9PSUf4y/CwEcXtNPrwJxBFBtvTU7MqRSMVnEh4AjnV/UmXREB1jwelWA==", null, false, "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d", false, "admin@digitalisationmanager.local" });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "FundType", "Title" },
                values: new object[,]
                {
                    { 1, "AF-001", new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded demo fund (remove if you want empty DB).", 0, "Archive Fund 001" },
                    { 2, "PF-001", new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded demo fund (remove if you want empty DB).", 2, "Photo Fund 001" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalFiles_ItemId",
                table: "DigitalFiles",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_FundId_InventoryNumber",
                table: "Items",
                columns: new[] { "FundId", "InventoryNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DigitalFiles");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Funds");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a3d2b6e-3f7a-4c8c-9b4b-6c5bdfaa1111");
        }
    }
}
