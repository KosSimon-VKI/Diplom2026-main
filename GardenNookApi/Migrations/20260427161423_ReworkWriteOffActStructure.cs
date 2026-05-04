using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GardenNookApi.Migrations
{
    /// <inheritdoc />
    public partial class ReworkWriteOffActStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientWriteOffActs");

            migrationBuilder.DropTable(
                name: "SemiFinishedWriteOffActs");

            migrationBuilder.DropTable(
                name: "WriteOffReasons");

            migrationBuilder.CreateTable(
                name: "WriteOffActs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StaffId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffActs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WriteOffActs_Staff",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientWriteOffActItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WriteOffActId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnitOfMeasureId = table.Column<int>(type: "int", nullable: true),
                    WriteOffTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientWriteOffActItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActItems_Ingredients",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActItems_UnitsOfMeasure",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActItems_WriteOffActs",
                        column: x => x.WriteOffActId,
                        principalTable: "WriteOffActs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientWriteOffActItems_WriteOffTypes",
                        column: x => x.WriteOffTypeId,
                        principalTable: "WriteOffTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SemiFinishedWriteOffActItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WriteOffActId = table.Column<int>(type: "int", nullable: false),
                    SemiFinishedId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnitOfMeasureId = table.Column<int>(type: "int", nullable: true),
                    WriteOffTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemiFinishedWriteOffActItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemiFinishedWriteOffActItems_SemiFinished",
                        column: x => x.SemiFinishedId,
                        principalTable: "SemiFinished",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SemiFinishedWriteOffActItems_UnitsOfMeasure",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SemiFinishedWriteOffActItems_WriteOffActs",
                        column: x => x.WriteOffActId,
                        principalTable: "WriteOffActs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SemiFinishedWriteOffActItems_WriteOffTypes",
                        column: x => x.WriteOffTypeId,
                        principalTable: "WriteOffTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "WriteOffTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Порча" },
                    { 2, "Питание персонала" },
                    { 3, "Брокераж" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActItems_IngredientId",
                table: "IngredientWriteOffActItems",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActItems_UnitOfMeasureId",
                table: "IngredientWriteOffActItems",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActItems_WriteOffActId",
                table: "IngredientWriteOffActItems",
                column: "WriteOffActId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientWriteOffActItems_WriteOffTypeId",
                table: "IngredientWriteOffActItems",
                column: "WriteOffTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActItems_SemiFinishedId",
                table: "SemiFinishedWriteOffActItems",
                column: "SemiFinishedId");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActItems_UnitOfMeasureId",
                table: "SemiFinishedWriteOffActItems",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActItems_WriteOffActId",
                table: "SemiFinishedWriteOffActItems",
                column: "WriteOffActId");

            migrationBuilder.CreateIndex(
                name: "IX_SemiFinishedWriteOffActItems_WriteOffTypeId",
                table: "SemiFinishedWriteOffActItems",
                column: "WriteOffTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffActs_Date",
                table: "WriteOffActs",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffActs_StaffId",
                table: "WriteOffActs",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientWriteOffActItems");

            migrationBuilder.DropTable(
                name: "SemiFinishedWriteOffActItems");

            migrationBuilder.DropTable(
                name: "WriteOffActs");

            migrationBuilder.DropTable(
                name: "WriteOffTypes");

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
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    UnitOfMeasureId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                    ReasonId = table.Column<int>(type: "int", nullable: false),
                    SemiFinishedId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityGrams = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
    }
}
