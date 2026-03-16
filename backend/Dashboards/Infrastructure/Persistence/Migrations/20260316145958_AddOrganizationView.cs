using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Заказчики\";");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW university.""Заказчики"" AS
                WITH
                target_students AS (
                    SELECT DISTINCT ON (s.""Id"")
                        s.""Id"" AS id_студента,
                        b.""Name"" AS ""Институт"",             
                        f.""Name"" AS ""Филилал"",              
                        ep.""Name"" AS ""Направление подготовки"",
                        es.""Name"" AS ""Профиль подготовки"",
                        o.""Name"" AS ""Заказчик"",                
                        s.""AdmissionYear"" AS ""Год поступления"",
                        cs.""ContingentDate"" AS ""Дата среза""
                    FROM university.""Student"" s
                             LEFT JOIN dictionary.""Branch"" b ON s.""BranchId"" = b.""Id""
                             LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                             LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                             LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                             LEFT JOIN dictionary.""Organization"" o ON s.""OrganizationId"" = o.""Id""
                             LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                             LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                    WHERE a_s.""Name"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                      AND s.""OrganizationId"" IS NOT NULL 
                      AND o.""Name"" IS NOT NULL
                    ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                )

                SELECT
                    ""Институт"",
                    ""Филилал"",
                    ""Направление подготовки"",
                    ""Заказчик"",
                    ""Год поступления"",
                    ""Дата среза"",
                    COUNT(DISTINCT id_студента) AS количество_студентов
                FROM target_students
                GROUP BY ""Институт"", ""Филилал"", ""Направление подготовки"", ""Заказчик"", ""Год поступления"", ""Дата среза"";
                ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Заказчики"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Заказчики\";");
        }
    }
}
