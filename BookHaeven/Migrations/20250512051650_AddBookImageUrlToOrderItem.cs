using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHaeven.Migrations
{
    /// <inheritdoc />
    public partial class AddBookImageUrlToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookImageUrl",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookImageUrl",
                table: "OrderItem");
        }
    }
}
