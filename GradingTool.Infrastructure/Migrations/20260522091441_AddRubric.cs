using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GradingTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRubric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Criteria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Criteria_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "CreatedAt", "MaxScore", "OrderIndex", "QuestionNo", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3.0m, 1, 1, "Class Diagram", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4.0m, 2, 2, "Sequence Diagram", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3.0m, 3, 3, "Activity/Statechart Diagram", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "Code", "CreatedAt", "MaxScore", "Name", "OrderIndex", "QuestionId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "1.1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.0m, "Đúng các class chính", 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "1.2", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.0m, "Quan hệ giữa class", 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "1.3", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.5m, "Thuộc tính/method", 3, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "1.4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.5m, "Phần giải thích", 4, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "2.1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.0m, "Actor & Object đúng", 1, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "2.2", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.5m, "Messages đúng thứ tự", 2, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "2.3", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.0m, "Xử lý điều kiện alt/opt", 3, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "2.4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.5m, "Phần giải thích", 4, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "3.1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.5m, "Các state/activity đúng", 1, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "3.2", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.0m, "Transitions đúng", 2, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "3.3", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.5m, "Phần giải thích", 3, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_QuestionId_Code",
                table: "Criteria",
                columns: new[] { "QuestionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionNo",
                table: "Questions",
                column: "QuestionNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Criteria");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
