using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenNookApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItemPortionLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItemPortionLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RemainingPortions = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemPortionLimits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UX_MenuItemPortionLimits_ItemType_ItemId",
                table: "MenuItemPortionLimits",
                columns: new[] { "ItemType", "ItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemPortionLimits");
        }
    }
}
