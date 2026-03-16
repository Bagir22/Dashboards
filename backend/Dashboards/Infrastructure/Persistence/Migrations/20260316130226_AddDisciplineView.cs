using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDisciplineView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Дисциплины\";");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW university.""Дисциплины"" AS
                WITH
                    last_contingent AS (
                        SELECT DISTINCT ON (s.""Id"")
                            s.""Id"" AS id_студента,
                            cs.""Course"" AS ""Курс"",
                            cs.""Budget"" AS ""Источник финансирования"",
                            cs.""ContingentDate"" AS ""Дата среза""
                        FROM university.""Student"" s
                                 LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                        ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                    ),
                    active_students AS (
                        SELECT DISTINCT ON (s.""Id"")
                            s.""Id"" AS id_студента,
                            f.""Name"" AS ""Факультет"",
                            tl.""Name"" AS ""Уровень подготовки"",
                            sf.""Name"" AS ""Форма обучения"",
                            s.""AdmissionYear"" AS ""Год поступления"",
                            lc.""Курс"",
                            lc.""Источник финансирования""
                        FROM university.""Student"" s
                                 LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                                 LEFT JOIN dictionary.""TrainingLevel"" tl ON s.""TrainingLevelId"" = tl.""Id""
                                 LEFT JOIN dictionary.""StudyForm"" sf ON s.""StudyFormId"" = sf.""Id""
                                 LEFT JOIN last_contingent lc ON s.""Id"" = lc.id_студента
                                 LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                                 LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                        WHERE a_s.""Name"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                        ORDER BY s.""Id"", cs.""ContingentDate"" DESC
                    )

                SELECT
                    a.""Факультет"",
                    a.""Уровень подготовки"",
                    a.""Форма обучения"",
                    a.""Год поступления"",
                    a.""Курс"",
                    a.""Источник финансирования"",
                    d.""Name"" AS ""Дисциплина"",
                    COUNT(*) AS ""Количество записей"",
                    
                    CASE
                        WHEN EXTRACT(MONTH FROM sd.""MarkDate"") = 3 THEN 'Летняя сессия'
                        WHEN EXTRACT(MONTH FROM sd.""MarkDate"") = 11 THEN 'Зимняя сессия'
                        ELSE 'Межсессионный период'
                        END AS ""Тип сессии"",

                    sd.""MarkDate"" AS ""Дата оценки""

                FROM active_students a
                         JOIN university.""SheetDiscipline"" sd ON a.id_студента = sd.""StudentId""
                         JOIN dictionary.""Discipline"" d ON sd.""DisciplineId"" = d.""Id""
                GROUP BY a.""Факультет"", a.""Уровень подготовки"", a.""Форма обучения"", a.""Год поступления"",
                         a.""Курс"", a.""Источник финансирования"",
                         d.""Name"", EXTRACT(YEAR FROM sd.""MarkDate""), EXTRACT(MONTH FROM sd.""MarkDate""),
                         CASE
                             WHEN EXTRACT(MONTH FROM sd.""MarkDate"") = 3 THEN 'Летняя сессия'
                             WHEN EXTRACT(MONTH FROM sd.""MarkDate"") = 11 THEN 'Зимняя сессия'
                             ELSE 'Межсессионный период'
                             END,
                         sd.""MarkDate"";
                ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Дисциплины"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Дисциплины\";");
        }
    }
}
