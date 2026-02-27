using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SplitStudentAddAchivmentAndOpder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (SELECT 1 FROM pg_views WHERE schemaname = 'public' AND viewname = 'studentview') THEN
                        REVOKE SELECT ON public.studentview FROM metabase;
                    END IF;
                END $$;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS public.studentview");
            
            migrationBuilder.DropForeignKey(
                name: "FK_Student_AcademicState_AcademicStateId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_AddressState_AddressStateId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_AcademicStateId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_AddressStateId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_ContingentDate",
                schema: "university",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_StudentExternalId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_StudentExternalId_ContingentDate",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "AcademicStateId",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Ball",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Budget",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "ContingentDate",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Course",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "StudentExternalId",
                schema: "university",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "AddressStateId",
                schema: "university",
                table: "Student",
                newName: "GroupId");

            migrationBuilder.AddColumn<int>(
                name: "AdmissionYear",
                schema: "university",
                table: "Student",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Fio",
                schema: "university",
                table: "Student",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AchivmentCategory",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchivmentCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContingentStudent",
                schema: "university",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressStateId = table.Column<Guid>(type: "uuid", nullable: true),
                    Course = table.Column<int>(type: "integer", nullable: true),
                    Ball = table.Column<double>(type: "double precision", nullable: true),
                    Budget = table.Column<string>(type: "text", nullable: true),
                    ContingentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentExternalId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContingentStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContingentStudent_AcademicState_AcademicStateId",
                        column: x => x.AcademicStateId,
                        principalSchema: "dictionary",
                        principalTable: "AcademicState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContingentStudent_AddressState_AddressStateId",
                        column: x => x.AddressStateId,
                        principalSchema: "dictionary",
                        principalTable: "AddressState",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContingentStudent_Student_StudentExternalId",
                        column: x => x.StudentExternalId,
                        principalSchema: "university",
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderCategory",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Achivment",
                schema: "university",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achivment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achivment_AchivmentCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dictionary",
                        principalTable: "AchivmentCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Achivment_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "university",
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "university",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_OrderCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dictionary",
                        principalTable: "OrderCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "university",
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achivment_CategoryId",
                schema: "university",
                table: "Achivment",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Achivment_StudentId",
                schema: "university",
                table: "Achivment",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContingentStudent_AcademicStateId",
                schema: "university",
                table: "ContingentStudent",
                column: "AcademicStateId");

            migrationBuilder.CreateIndex(
                name: "IX_ContingentStudent_AddressStateId",
                schema: "university",
                table: "ContingentStudent",
                column: "AddressStateId");

            migrationBuilder.CreateIndex(
                name: "IX_ContingentStudent_ContingentDate",
                schema: "university",
                table: "ContingentStudent",
                column: "ContingentDate");

            migrationBuilder.CreateIndex(
                name: "IX_ContingentStudent_StudentExternalId",
                schema: "university",
                table: "ContingentStudent",
                column: "StudentExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_ContingentStudent_StudentExternalId_ContingentDate",
                schema: "university",
                table: "ContingentStudent",
                columns: new[] { "StudentExternalId", "ContingentDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_CategoryId",
                schema: "university",
                table: "Order",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_StudentId",
                schema: "university",
                table: "Order",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achivment",
                schema: "university");

            migrationBuilder.DropTable(
                name: "ContingentStudent",
                schema: "university");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "university");

            migrationBuilder.DropTable(
                name: "AchivmentCategory",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "OrderCategory",
                schema: "dictionary");

            migrationBuilder.DropColumn(
                name: "AdmissionYear",
                schema: "university",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Fio",
                schema: "university",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                schema: "university",
                table: "Student",
                newName: "AddressStateId");

            migrationBuilder.AddColumn<Guid>(
                name: "AcademicStateId",
                schema: "university",
                table: "Student",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "Ball",
                schema: "university",
                table: "Student",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Budget",
                schema: "university",
                table: "Student",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContingentDate",
                schema: "university",
                table: "Student",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Course",
                schema: "university",
                table: "Student",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentExternalId",
                schema: "university",
                table: "Student",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Student_AcademicStateId",
                schema: "university",
                table: "Student",
                column: "AcademicStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_AddressStateId",
                schema: "university",
                table: "Student",
                column: "AddressStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_ContingentDate",
                schema: "university",
                table: "Student",
                column: "ContingentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Student_StudentExternalId",
                schema: "university",
                table: "Student",
                column: "StudentExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_StudentExternalId_ContingentDate",
                schema: "university",
                table: "Student",
                columns: new[] { "StudentExternalId", "ContingentDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_AcademicState_AcademicStateId",
                schema: "university",
                table: "Student",
                column: "AcademicStateId",
                principalSchema: "dictionary",
                principalTable: "AcademicState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_AddressState_AddressStateId",
                schema: "university",
                table: "Student",
                column: "AddressStateId",
                principalSchema: "dictionary",
                principalTable: "AddressState",
                principalColumn: "Id");
        }
    }
}
