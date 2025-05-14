using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHaeven.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIdToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Orders_OrderId",
                table: "Reviews",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Orders_OrderId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Reviews");
        }
    }
}
