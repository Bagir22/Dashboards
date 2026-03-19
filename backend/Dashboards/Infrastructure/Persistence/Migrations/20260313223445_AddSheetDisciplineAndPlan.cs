using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSheetDisciplineAndPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plan",
                schema: "university",
                columns: table => new
                {
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => new { x.SemesterId, x.DisciplineId });
                    table.ForeignKey(
                        name: "FK_Plan_Discipline_DisciplineId",
                        column: x => x.DisciplineId,
                        principalSchema: "dictionary",
                        principalTable: "Discipline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plan_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalSchema: "dictionary",
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SheetDiscipline",
                schema: "university",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Retake = table.Column<int>(type: "integer", nullable: false),
                    MarkDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uuid", nullable: false),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarkId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetDiscipline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheetDiscipline_Discipline_DisciplineId",
                        column: x => x.DisciplineId,
                        principalSchema: "dictionary",
                        principalTable: "Discipline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SheetDiscipline_Mark_MarkId",
                        column: x => x.MarkId,
                        principalSchema: "dictionary",
                        principalTable: "Mark",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SheetDiscipline_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalSchema: "dictionary",
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SheetDiscipline_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "university",
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plan_DisciplineId",
                schema: "university",
                table: "Plan",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetDiscipline_DisciplineId",
                schema: "university",
                table: "SheetDiscipline",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetDiscipline_MarkId",
                schema: "university",
                table: "SheetDiscipline",
                column: "MarkId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetDiscipline_SemesterId",
                schema: "university",
                table: "SheetDiscipline",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetDiscipline_StudentId",
                schema: "university",
                table: "SheetDiscipline",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plan",
                schema: "university");

            migrationBuilder.DropTable(
                name: "SheetDiscipline",
                schema: "university");
        }
    }
}
