using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenNookApi.Migrations
{
    /// <inheritdoc />
    public partial class RestorePreparationTaskTextAndOptionalSemiFinished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SemiFinishedId",
                table: "PreparationTasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql(
                """
                IF COL_LENGTH(N'dbo.PreparationTasks', N'TaskText') IS NULL
                BEGIN
                    ALTER TABLE [PreparationTasks]
                    ADD [TaskText] nvarchar(255) NOT NULL
                    CONSTRAINT [DF_PreparationTasks_TaskText] DEFAULT N'';
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH(N'dbo.PreparationTasks', N'TaskText') IS NOT NULL
                BEGIN
                    DECLARE @constraintName nvarchar(128);
                    SELECT @constraintName = [dc].[name]
                    FROM [sys].[default_constraints] AS [dc]
                    INNER JOIN [sys].[columns] AS [c]
                        ON [dc].[parent_object_id] = [c].[object_id]
                       AND [dc].[parent_column_id] = [c].[column_id]
                    WHERE [dc].[parent_object_id] = OBJECT_ID(N'dbo.PreparationTasks')
                      AND [c].[name] = N'TaskText';

                    IF @constraintName IS NOT NULL
                    BEGIN
                        EXEC(N'ALTER TABLE [PreparationTasks] DROP CONSTRAINT [' + @constraintName + N']');
                    END

                    ALTER TABLE [PreparationTasks] DROP COLUMN [TaskText];
                END
                """);

            migrationBuilder.AlterColumn<int>(
                name: "SemiFinishedId",
                table: "PreparationTasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
