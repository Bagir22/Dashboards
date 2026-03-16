using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExpulsionView : Migration
    {
        /// <inheritdoc />
          protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Динамика отчислений\";");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW university.""Динамика отчислений"" AS
                WITH
                all_students AS (
                    SELECT
                        EXTRACT(YEAR FROM cs.""ContingentDate"") AS ""Год"",
                        f.""Name"" AS ""Факультет"",
                        tl.""Name"" AS ""Уровень подготовки"",
                        ep.""Name"" AS ""Профиль подготовки"",
                        es.""Name"" AS ""Направление подготовки"",
                        a_s.""Name"" AS ""Статус"",
                        COUNT(*) AS ""Количество""
                    FROM university.""ContingentStudent"" cs
                             JOIN university.""Student"" s ON cs.""StudentExternalId"" = s.""Id""
                             LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                             LEFT JOIN dictionary.""TrainingLevel"" tl ON s.""TrainingLevelId"" = tl.""Id""
                             LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                             LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                             LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                    GROUP BY EXTRACT(YEAR FROM cs.""ContingentDate""), f.""Name"", tl.""Name"", ep.""Name"", a_s.""Name"", es.""Name""
                )

                SELECT
                    ""Год"",
                    ""Факультет"",
                    ""Уровень подготовки"",
                    ""Профиль подготовки"",
                    ""Направление подготовки"",
                    SUM(CASE WHEN ""Статус"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                                 THEN ""Количество"" ELSE 0 END) AS ""Активных"",
                    SUM(CASE WHEN ""Статус"" = 'Отчислен'
                                 THEN ""Количество"" ELSE 0 END) AS отчисленных,
                    SUM(""Количество"") AS всего,

                    ROUND(
                            SUM(CASE WHEN ""Статус"" = 'Отчислен' THEN ""Количество"" ELSE 0 END) * 100.0 /
                            NULLIF(SUM(""Количество""), 0),
                            2
                    ) AS процент_отчисленных

                FROM all_students
                GROUP BY ""Год"", ""Факультет"", ""Уровень подготовки"", ""Профиль подготовки"", ""Направление подготовки""
                ORDER BY ""Год"" DESC, ""Факультет"";
                ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Динамика отчислений"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Динамика отчислений\";");
        }
    }
}
