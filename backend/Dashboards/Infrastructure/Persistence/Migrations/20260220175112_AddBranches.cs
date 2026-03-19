using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "university",
                table: "Student",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Branch",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_BranchId",
                schema: "university",
                table: "Student",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Branch_BranchId",
                schema: "university",
                table: "Student",
                column: "BranchId",
                principalSchema: "dictionary",
                principalTable: "Branch",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Branch_BranchId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropTable(
                name: "Branch",
                schema: "dictionary");

            migrationBuilder.DropIndex(
                name: "IX_Student_BranchId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "university",
                table: "Student");
        }
    }
}
