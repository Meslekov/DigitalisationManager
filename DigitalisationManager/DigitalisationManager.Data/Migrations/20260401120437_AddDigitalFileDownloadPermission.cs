using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalisationManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalFileDownloadPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDownloadAllowed",
                table: "DigitalFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDownloadAllowed",
                table: "DigitalFiles");
        }
    }
}
