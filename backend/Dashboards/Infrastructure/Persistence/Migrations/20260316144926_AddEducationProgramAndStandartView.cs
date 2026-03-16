using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationProgramAndStandartView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Направления и профили подготовки\";");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW university.""Направления и профили подготовки"" AS
                WITH
                active_students AS (
                    SELECT DISTINCT ON (s.""Id"")
                        s.""Id"" AS id_студента,
                        b.""Name""::varchar(100) AS ""Филиал"",
                        f.""Name""::varchar(100) AS ""Факультет"",
                        ep.""Name""::varchar(100) AS ""Направление подготовки"",
                        es.""Name""::varchar(100) AS ""Профиль подготовки"",
                        s.""AdmissionYear"" AS ""Год поступления"",
                        cs.""ContingentDate"" AS ""Дата среза""
                    FROM university.""Student"" s
                             LEFT JOIN dictionary.""Branch"" b ON s.""BranchId"" = b.""Id""
                             LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                             LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                             LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                             LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                             LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                    WHERE a_s.""Name"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                    ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                )

                SELECT
                    ""Филиал"",
                    ""Факультет"",
                    ""Направление подготовки"",
                    ""Профиль подготовки"",
                    ""Год поступления"",
                    ""Дата среза"",
                    COUNT(DISTINCT id_студента) AS ""Количество студентов""
                FROM active_students
                GROUP BY ""Филиал"", ""Факультет"", ""Направление подготовки"", ""Профиль подготовки"", ""Год поступления"", ""Дата среза"";
                ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Направления и профили подготовки"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Направления и профили подготовки\";");
        }
    }
}
