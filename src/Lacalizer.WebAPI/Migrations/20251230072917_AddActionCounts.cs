using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lacalizer.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddActionCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentCounts",
                schema: "video",
                table: "VideoItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCounts",
                schema: "video",
                table: "VideoItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantCounts",
                schema: "video",
                table: "VideoItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShareCounts",
                schema: "video",
                table: "VideoItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentCounts",
                schema: "video",
                table: "VideoItems");

            migrationBuilder.DropColumn(
                name: "LikeCounts",
                schema: "video",
                table: "VideoItems");

            migrationBuilder.DropColumn(
                name: "ParticipantCounts",
                schema: "video",
                table: "VideoItems");

            migrationBuilder.DropColumn(
                name: "ShareCounts",
                schema: "video",
                table: "VideoItems");
        }
    }
}
