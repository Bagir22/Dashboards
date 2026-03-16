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
                    last_contingent AS (
                        SELECT DISTINCT ON (s.""Id"")
                            s.""Id"" AS id_студента,
                            cs.""Course"" AS ""Курс"",
                            cs.""Budget"" AS ""Источник финансирования""
                        FROM university.""Student"" s
                                 LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                        ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                    ),
                    target_students AS (
                        SELECT DISTINCT ON (s.""Id"")
                            s.""Id"" AS id_студента,
                            b.""Name"" AS ""Филиал"",                 
                            f.""Name"" AS ""Факультет"",                 
                            tl.""Name"" AS ""Уровень подготовки"",
                            sf.""Name"" AS ""Форма обучения"",
                            ep.""Name"" AS ""Направление подготовки"",
                            es.""Name"" AS ""Профиль подготовки"",
                            o.""Name"" AS ""Заказчик"",
                            s.""AdmissionYear"" AS ""Год поступления"",
                            cs.""ContingentDate"" AS ""Дата среза"",
                            lc.""Курс"",
                            lc.""Источник финансирования""
                        FROM university.""Student"" s
                                 LEFT JOIN dictionary.""Branch"" b ON s.""BranchId"" = b.""Id""
                                 LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                                 LEFT JOIN dictionary.""TrainingLevel"" tl ON s.""TrainingLevelId"" = tl.""Id""
                                 LEFT JOIN dictionary.""StudyForm"" sf ON s.""StudyFormId"" = sf.""Id""
                                 LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                                 LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                                 LEFT JOIN dictionary.""Organization"" o ON s.""OrganizationId"" = o.""Id""
                                 LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                                 LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                                 LEFT JOIN last_contingent lc ON s.""Id"" = lc.id_студента
                        WHERE a_s.""Name"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                          AND s.""OrganizationId"" IS NOT NULL
                          AND o.""Name"" IS NOT NULL
                        ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                    )

                SELECT
                    ""Филиал"",
                    ""Факультет"",
                    ""Уровень подготовки"",
                    ""Форма обучения"",
                    ""Направление подготовки"",
                    ""Профиль подготовки"",
                    ""Заказчик"",
                    ""Год поступления"",
                    ""Курс"",
                    ""Источник финансирования"",
                    ""Дата среза"",
                    CASE
                        WHEN EXTRACT(MONTH FROM ""Дата среза"") = 3 THEN 'Летняя сессия'
                        WHEN EXTRACT(MONTH FROM ""Дата среза"") = 10 THEN 'Зимняя сессия'
                        ELSE 'Обычный срез'
                        END AS ""Тип среза"",
                    COUNT(DISTINCT id_студента) AS ""Количество студентов""
                FROM target_students
                GROUP BY ""Филиал"", ""Факультет"", ""Уровень подготовки"", ""Форма обучения"",
                         ""Направление подготовки"", ""Профиль подготовки"", ""Заказчик"",
                         ""Год поступления"", ""Курс"", ""Источник финансирования"", ""Дата среза"";
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
