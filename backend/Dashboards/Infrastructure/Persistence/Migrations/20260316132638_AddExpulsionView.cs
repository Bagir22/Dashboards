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
                        cs.""Id"" AS id_записи,
                        cs.""StudentExternalId"" AS id_студента,
                        EXTRACT(YEAR FROM cs.""ContingentDate"") AS ""Год"",
                        f.""Name"" AS ""Факультет"",
                        tl.""Name"" AS ""Уровень подготовки"",
                        sf.""Name"" AS ""Форма обучения"",
                        ep.""Name"" AS ""Профиль подготовки"",
                        es.""Name"" AS ""Направление подготовки"",
                        cs.""Course"" AS ""Курс"",
                        cs.""Budget"" AS ""Источник финансирования"",
                        a_s.""Name"" AS ""Статус"",
                        cs.""ContingentDate"" AS ""Дата среза""
                    FROM university.""ContingentStudent"" cs
                             JOIN university.""Student"" s ON cs.""StudentExternalId"" = s.""Id""
                             LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                             LEFT JOIN dictionary.""TrainingLevel"" tl ON s.""TrainingLevelId"" = tl.""Id""
                             LEFT JOIN dictionary.""StudyForm"" sf ON s.""StudyFormId"" = sf.""Id""
                             LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                             LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                             LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                ),

                expulsion_orders AS (
                    SELECT
                        o.""StudentId"" AS id_студента,
                        o.""Date"" AS ""Дата приказа"",
                        EXTRACT(YEAR FROM o.""Date"") AS ""Год приказа"",
                        oc.""Name"" AS ""Причина отчисления""
                    FROM university.""Order"" o
                             JOIN dictionary.""OrderCategory"" oc ON o.""CategoryId"" = oc.""Id""
                ),

                aggregated_by_slice AS (
                    SELECT
                        ""Год"",
                        ""Факультет"",
                        ""Уровень подготовки"",
                        ""Форма обучения"",
                        ""Профиль подготовки"",
                        ""Направление подготовки"",
                        ""Курс"",
                        ""Источник финансирования"",
                        ""Дата среза"",

                        COUNT(DISTINCT CASE WHEN ""Статус"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                                                THEN id_студента END) AS ""Активных"",

                        COUNT(DISTINCT CASE WHEN ""Статус"" = 'Отчислен'
                                                THEN id_студента END) AS ""Отчисленных"",

                        COUNT(DISTINCT id_студента) AS ""Всего""
                    FROM all_students
                    GROUP BY ""Год"", ""Факультет"", ""Уровень подготовки"", ""Форма обучения"",
                             ""Профиль подготовки"", ""Направление подготовки"", ""Курс"",
                             ""Источник финансирования"", ""Дата среза""
                ),

                expulsions_by_reason AS (
                    SELECT
                        EXTRACT(YEAR FROM eo.""Дата приказа"") AS ""Год"",
                        f.""Name"" AS ""Факультет"",
                        tl.""Name"" AS ""Уровень подготовки"",
                        sf.""Name"" AS ""Форма обучения"",
                        ep.""Name"" AS ""Профиль подготовки"",
                        es.""Name"" AS ""Направление подготовки"",
                        eo.""Причина отчисления"",
                        COUNT(DISTINCT eo.id_студента) AS ""Количество отчисленных по причине""
                    FROM expulsion_orders eo
                             JOIN university.""Student"" s ON eo.id_студента = s.""Id""
                             LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                             LEFT JOIN dictionary.""TrainingLevel"" tl ON s.""TrainingLevelId"" = tl.""Id""
                             LEFT JOIN dictionary.""StudyForm"" sf ON s.""StudyFormId"" = sf.""Id""
                             LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                             LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                    GROUP BY EXTRACT(YEAR FROM eo.""Дата приказа""), f.""Name"", tl.""Name"", sf.""Name"",
                             ep.""Name"", es.""Name"", eo.""Причина отчисления""
                )

                SELECT
                    a.""Год"",
                    a.""Факультет"",
                    a.""Уровень подготовки"",
                    a.""Форма обучения"",
                    a.""Профиль подготовки"",
                    a.""Направление подготовки"",
                    a.""Курс"",
                    a.""Источник финансирования"",
                    a.""Дата среза"",
                    a.""Активных"",
                    a.""Отчисленных"",
                    a.""Всего"",
                    ROUND(a.""Отчисленных"" * 100.0 / NULLIF(a.""Всего"", 0), 2) AS ""Процент отчисленных"",

                    e.""Причина отчисления"",
                    e.""Количество отчисленных по причине""

                FROM aggregated_by_slice a
                         LEFT JOIN expulsions_by_reason e ON a.""Год"" = e.""Год""
                    AND a.""Факультет"" = e.""Факультет""
                    AND a.""Уровень подготовки"" = e.""Уровень подготовки""
                    AND a.""Форма обучения"" = e.""Форма обучения""
                    AND a.""Профиль подготовки"" = e.""Профиль подготовки""
                    AND a.""Направление подготовки"" = e.""Направление подготовки""

                ORDER BY a.""Год"" DESC, a.""Факультет"", e.""Причина отчисления"";
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
