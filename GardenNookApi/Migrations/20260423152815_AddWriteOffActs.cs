using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GardenNookApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWriteOffActs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WriteOffReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientWriteOffActs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    ReasonId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnitOfMeasureId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientWriteOffActs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActs_Ingredients",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActs_Staff",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActs_UnitsOfMeasure",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActs_WriteOffReasons",
                        column: x => x.ReasonId,
                        principalTable: "WriteOffReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SemiFinishedWriteOffActs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemiFinishedId = table.Column<int>(type: "int", nullable: false),
                    ReasonId = table.Column<int>(type: "int", nullable: false),
                    QuantityGrams = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemiFinishedWriteOffActs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemiFinishedWriteOffActs_SemiFinished",
                        column: x => x.SemiFinishedId,
                        principalTable: "SemiFinished",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SemiFinishedWriteOffActs_Staff",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SemiFinishedWriteOffActs_WriteOffReasons",
                        column: x => x.ReasonId,
                        principalTable: "WriteOffReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "WriteOffReasons",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Порча" },
                    { 2, "Питание персонала" },
                    { 3, "Брокераж" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActs_CreatedAt",
                table: "IngredientWriteOffActs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActs_IngredientId",
                table: "IngredientWriteOffActs",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActs_ReasonId",
                table: "IngredientWriteOffActs",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActs_StaffId",
                table: "IngredientWriteOffActs",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActs_UnitOfMeasureId",
                table: "IngredientWriteOffActs",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActs_CreatedAt",
                table: "SemiFinishedWriteOffActs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActs_ReasonId",
                table: "SemiFinishedWriteOffActs",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActs_SemiFinishedId",
                table: "SemiFinishedWriteOffActs",
                column: "SemiFinishedId");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActs_StaffId",
                table: "SemiFinishedWriteOffActs",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientWriteOffActs");

            migrationBuilder.DropTable(
                name: "SemiFinishedWriteOffActs");

            migrationBuilder.DropTable(
                name: "WriteOffReasons");
        }
    }
}
