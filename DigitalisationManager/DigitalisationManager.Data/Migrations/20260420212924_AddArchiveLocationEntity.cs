using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DigitalisationManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArchiveLocationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArchiveLocationId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArchiveLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Room = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Shelf = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveLocations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ArchiveLocations",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Room", "Shelf" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 13, 10, 0, 0, 0, DateTimeKind.Utc), "Primary storage for municipal archive materials.", "Central Archive", "Room A", "Shelf 1" },
                    { 2, new DateTime(2026, 2, 13, 10, 5, 0, 0, DateTimeKind.Utc), "Storage area for photo and image materials.", "Photo Storage", "Room B", "Shelf 3" },
                    { 3, new DateTime(2026, 2, 13, 10, 10, 0, 0, DateTimeKind.Utc), "Restricted-access archival storage.", "Restricted Archive", "Room C", "Shelf 2" }
                });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 11,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 12,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 13,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 14,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 15,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 16,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 17,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 18,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 19,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 20,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 21,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 22,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 23,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 24,
                column: "ArchiveLocationId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ArchiveLocationId",
                table: "Items",
                column: "ArchiveLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveLocations_Name",
                table: "ArchiveLocations",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ArchiveLocations_ArchiveLocationId",
                table: "Items",
                column: "ArchiveLocationId",
                principalTable: "ArchiveLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ArchiveLocations_ArchiveLocationId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "ArchiveLocations");

            migrationBuilder.DropIndex(
                name: "IX_Items_ArchiveLocationId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ArchiveLocationId",
                table: "Items");
        }
    }
}
