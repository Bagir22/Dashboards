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
                    last_contingent AS (
                        SELECT DISTINCT ON (s.""Id"")
                            s.""Id"" AS id_студента,
                            cs.""Course"" AS ""Курс"",
                            cs.""Budget"" AS ""Источник финансирования""
                        FROM university.""Student"" s
                                 LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                        ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                    ),
                    active_students AS (
                        SELECT DISTINCT ON (s.""Id"")
                            s.""Id"" AS id_студента,
                            b.""Name""::varchar(100) AS ""Филиал"",
                            f.""Name""::varchar(100) AS ""Факультет"",
                            tl.""Name""::varchar(100) AS ""Уровень подготовки"",
                            sf.""Name""::varchar(100) AS ""Форма обучения"",
                            ep.""Name""::varchar(100) AS ""Направление подготовки"",
                            es.""Name""::varchar(100) AS ""Профиль подготовки"",
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
                                 LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                                 LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                                 LEFT JOIN last_contingent lc ON s.""Id"" = lc.id_студента
                        WHERE a_s.""Name"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                        ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                    )

                SELECT
                    ""Филиал"",
                    ""Факультет"",
                    ""Уровень подготовки"",
                    ""Форма обучения"",
                    ""Направление подготовки"",
                    ""Профиль подготовки"",
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
                FROM active_students
                GROUP BY ""Филиал"", ""Факультет"", ""Уровень подготовки"", ""Форма обучения"",
                         ""Направление подготовки"", ""Профиль подготовки"", ""Год поступления"",
                         ""Курс"", ""Источник финансирования"", ""Дата среза"";
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
