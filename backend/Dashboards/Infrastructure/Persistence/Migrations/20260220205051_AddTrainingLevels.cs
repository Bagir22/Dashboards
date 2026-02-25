using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TrainingLevelId",
                schema: "university",
                table: "Student",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainingLevel",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingLevel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_TrainingLevelId",
                schema: "university",
                table: "Student",
                column: "TrainingLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_TrainingLevel_TrainingLevelId",
                schema: "university",
                table: "Student",
                column: "TrainingLevelId",
                principalSchema: "dictionary",
                principalTable: "TrainingLevel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_TrainingLevel_TrainingLevelId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropTable(
                name: "TrainingLevel",
                schema: "dictionary");

            migrationBuilder.DropIndex(
                name: "IX_Student_TrainingLevelId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "TrainingLevelId",
                schema: "university",
                table: "Student");
        }
    }
}
