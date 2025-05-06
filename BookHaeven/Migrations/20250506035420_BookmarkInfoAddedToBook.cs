using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHaeven.Migrations
{
    /// <inheritdoc />
    public partial class BookmarkInfoAddedToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBookmarked",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBookmarked",
                table: "Books");
        }
    }
}
