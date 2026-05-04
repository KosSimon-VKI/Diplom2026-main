using GardenNookApi.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenNookApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260409120000_AddKitchenOrderItemIsCompleted")]
    /// <inheritdoc />
    public partial class AddKitchenOrderItemIsCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "OrderDishItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "OrderDrinkItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "OrderToppingItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "OrderDishItems");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "OrderDrinkItems");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "OrderToppingItems");
        }
    }
}
