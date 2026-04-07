using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalisationManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class RolesTrueSeparation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Label", "Name", "NormalizedName" },
                values: new object[] { new Guid("99999999-9999-9999-9999-999999999999"), "88888888-8888-8888-8888-888888888888", "Manager", "Manager", "MANAGER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));
        }
    }
}
