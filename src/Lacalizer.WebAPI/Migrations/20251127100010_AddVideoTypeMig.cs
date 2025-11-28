using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacalizer.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoTypeMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlobName",
                schema: "video",
                table: "VideoItem",
                newName: "VideoUri");

            migrationBuilder.AddColumn<int>(
                name: "VideoType",
                schema: "video",
                table: "VideoItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoType",
                schema: "video",
                table: "VideoItem");

            migrationBuilder.RenameColumn(
                name: "VideoUri",
                schema: "video",
                table: "VideoItem",
                newName: "BlobName");
        }
    }
}
