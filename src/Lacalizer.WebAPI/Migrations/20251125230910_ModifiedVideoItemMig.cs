using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacalizer.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedVideoItemMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                schema: "video",
                table: "VideoItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                schema: "video",
                table: "VideoItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "video",
                table: "VideoItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                schema: "video",
                table: "VideoItem",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobName",
                schema: "video",
                table: "VideoItem");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "video",
                table: "VideoItem");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "video",
                table: "VideoItem");

            migrationBuilder.DropColumn(
                name: "Topic",
                schema: "video",
                table: "VideoItem");
        }
    }
}
