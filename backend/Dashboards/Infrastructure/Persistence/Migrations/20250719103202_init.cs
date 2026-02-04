using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dictionary");

            migrationBuilder.EnsureSchema(
                name: "university");

            migrationBuilder.CreateTable(
                name: "AcademicState",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressState",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Benefit",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benefit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Citizenship",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citizenship", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationProgram",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationProgram", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationStandard",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationStandard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faculty",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyForm",
                schema: "dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                schema: "university",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyFormId = table.Column<Guid>(type: "uuid", nullable: true),
                    CitizenshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uuid", nullable: true),
                    EducationProgramId = table.Column<Guid>(type: "uuid", nullable: true),
                    EducationStandardId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    BenefitId = table.Column<Guid>(type: "uuid", nullable: true),
                    AddressStateId = table.Column<Guid>(type: "uuid", nullable: true),
                    Course = table.Column<int>(type: "integer", nullable: true),
                    Ball = table.Column<double>(type: "double precision", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Budget = table.Column<string>(type: "text", nullable: true),
                    ContingentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_AcademicState_AcademicStateId",
                        column: x => x.AcademicStateId,
                        principalSchema: "dictionary",
                        principalTable: "AcademicState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Student_AddressState_AddressStateId",
                        column: x => x.AddressStateId,
                        principalSchema: "dictionary",
                        principalTable: "AddressState",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Benefit_BenefitId",
                        column: x => x.BenefitId,
                        principalSchema: "dictionary",
                        principalTable: "Benefit",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Citizenship_CitizenshipId",
                        column: x => x.CitizenshipId,
                        principalSchema: "dictionary",
                        principalTable: "Citizenship",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Student_EducationProgram_EducationProgramId",
                        column: x => x.EducationProgramId,
                        principalSchema: "dictionary",
                        principalTable: "EducationProgram",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_EducationStandard_EducationStandardId",
                        column: x => x.EducationStandardId,
                        principalSchema: "dictionary",
                        principalTable: "EducationStandard",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Faculty_FacultyId",
                        column: x => x.FacultyId,
                        principalSchema: "dictionary",
                        principalTable: "Faculty",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "dictionary",
                        principalTable: "Organization",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_StudyForm_StudyFormId",
                        column: x => x.StudyFormId,
                        principalSchema: "dictionary",
                        principalTable: "StudyForm",
                        principalColumn: "Id");
                });

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
                name: "IX_Student_BenefitId",
                schema: "university",
                table: "Student",
                column: "BenefitId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_CitizenshipId",
                schema: "university",
                table: "Student",
                column: "CitizenshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_ContingentDate",
                schema: "university",
                table: "Student",
                column: "ContingentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Student_EducationProgramId",
                schema: "university",
                table: "Student",
                column: "EducationProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_EducationStandardId",
                schema: "university",
                table: "Student",
                column: "EducationStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_FacultyId",
                schema: "university",
                table: "Student",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_OrganizationId",
                schema: "university",
                table: "Student",
                column: "OrganizationId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Student_StudyFormId",
                schema: "university",
                table: "Student",
                column: "StudyFormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student",
                schema: "university");

            migrationBuilder.DropTable(
                name: "AcademicState",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "AddressState",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "Benefit",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "Citizenship",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "EducationProgram",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "EducationStandard",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "Faculty",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "Organization",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "StudyForm",
                schema: "dictionary");
        }
    }
}
