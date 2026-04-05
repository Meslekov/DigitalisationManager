using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalisationManager.Data.Migrations
{
    public partial class RefactorDigitalFileForTiffPreviewWorkflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DigitalFiles_ItemId",
                table: "DigitalFiles");

            migrationBuilder.RenameColumn(
                name: "StoredFileName",
                table: "DigitalFiles",
                newName: "OriginalStoredFileName");

            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                table: "DigitalFiles",
                newName: "OriginalSizeBytes");

            migrationBuilder.RenameColumn(
                name: "RelativePath",
                table: "DigitalFiles",
                newName: "OriginalRelativePath");

            migrationBuilder.RenameColumn(
                name: "ChecksumSha256",
                table: "DigitalFiles",
                newName: "OriginalChecksumSha256");

            migrationBuilder.AddColumn<string>(
                name: "OriginalContentType",
                table: "DigitalFiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "image/tiff");

            migrationBuilder.AddColumn<string>(
                name: "PreviewStoredFileName",
                table: "DigitalFiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreviewRelativePath",
                table: "DigitalFiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreviewContentType",
                table: "DigitalFiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "PreviewSizeBytes",
                table: "DigitalFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_DigitalFiles_ItemId_OriginalStoredFileName",
                table: "DigitalFiles",
                columns: new[] { "ItemId", "OriginalStoredFileName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DigitalFiles_OriginalChecksumSha256",
                table: "DigitalFiles",
                column: "OriginalChecksumSha256");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DigitalFiles_ItemId_OriginalStoredFileName",
                table: "DigitalFiles");

            migrationBuilder.DropIndex(
                name: "IX_DigitalFiles_OriginalChecksumSha256",
                table: "DigitalFiles");

            migrationBuilder.DropColumn(
                name: "OriginalContentType",
                table: "DigitalFiles");

            migrationBuilder.DropColumn(
                name: "PreviewStoredFileName",
                table: "DigitalFiles");

            migrationBuilder.DropColumn(
                name: "PreviewRelativePath",
                table: "DigitalFiles");

            migrationBuilder.DropColumn(
                name: "PreviewContentType",
                table: "DigitalFiles");

            migrationBuilder.DropColumn(
                name: "PreviewSizeBytes",
                table: "DigitalFiles");

            migrationBuilder.RenameColumn(
                name: "OriginalStoredFileName",
                table: "DigitalFiles",
                newName: "StoredFileName");

            migrationBuilder.RenameColumn(
                name: "OriginalSizeBytes",
                table: "DigitalFiles",
                newName: "SizeBytes");

            migrationBuilder.RenameColumn(
                name: "OriginalRelativePath",
                table: "DigitalFiles",
                newName: "RelativePath");

            migrationBuilder.RenameColumn(
                name: "OriginalChecksumSha256",
                table: "DigitalFiles",
                newName: "ChecksumSha256");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalFiles_ItemId",
                table: "DigitalFiles",
                column: "ItemId");
        }
    }
}