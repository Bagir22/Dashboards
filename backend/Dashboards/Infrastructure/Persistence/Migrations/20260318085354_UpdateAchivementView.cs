using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAchivementView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Достижения\";");
            
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW university.""Достижения"" AS
                WITH
                last_contingent AS (
                    SELECT DISTINCT ON (cs.""StudentExternalId"")
                        cs.""StudentExternalId"" AS id_студента,
                        cs.""Course"" AS ""Курс"",
                        cs.""Budget"" AS ""Источник финансирования"",
                        cs.""ContingentDate"" AS ""Дата среза"",
                        cs.""AcademicStateId"" AS ""Статус ID"",
                        acs.""Name"" AS ""Статус""
                    FROM university.""ContingentStudent"" cs
                             LEFT JOIN dictionary.""AcademicState"" acs ON cs.""AcademicStateId"" = acs.""Id""
                    ORDER BY cs.""StudentExternalId"", cs.""ContingentDate"" DESC
                ),

                active_students AS (
                    SELECT
                        lc.id_студента,
                        lc.""Курс"",
                        lc.""Источник финансирования"",
                        lc.""Дата среза"",
                        lc.""Статус""
                    FROM last_contingent lc
                    WHERE lc.""Статус"" IN ('Учится', 'Условно переведён', 'В академическом отпуске')
                ),

                student_info AS (
                    SELECT DISTINCT ON (s.""Id"")
                        s.""Id"" AS id_студента,
                        b.""Name"" AS ""Филиал"",
                        f.""Name"" AS ""Факультет"",
                        tl.""Name"" AS ""Уровень подготовки"",
                        sf.""Name"" AS ""Форма обучения"",
                        ep.""Name"" AS ""Профиль подготовки"",
                        es.""Name"" AS ""Направление подготовки"",
                        g.""Name"" AS ""Группа"",
                        c.""Name"" AS ""Гражданство"",
                        bn.""Name"" AS ""Льготы"",
                        o.""Name"" AS ""Организация"",
                        s.""AdmissionYear"" AS ""Год поступления""
                    FROM university.""Student"" s
                             LEFT JOIN dictionary.""Branch"" b ON s.""BranchId"" = b.""Id""
                             LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                             LEFT JOIN dictionary.""TrainingLevel"" tl ON s.""TrainingLevelId"" = tl.""Id""
                             LEFT JOIN dictionary.""StudyForm"" sf ON s.""StudyFormId"" = sf.""Id""
                             LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                             LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                             LEFT JOIN dictionary.""Group"" g ON s.""GroupId"" = g.""Id""
                             LEFT JOIN dictionary.""Citizenship"" c ON s.""CitizenshipId"" = c.""Id""
                             LEFT JOIN dictionary.""Benefit"" bn ON s.""BenefitId"" = bn.""Id""
                             LEFT JOIN dictionary.""Organization"" o ON s.""OrganizationId"" = o.""Id""
                ),

                achievements AS (
                    SELECT
                        a.""StudentId"" AS id_студента,
                        a.""BeginDate"" AS ""Дата достижения"",
                        EXTRACT(YEAR FROM a.""BeginDate"") AS ""Год достижения"",
                        EXTRACT(MONTH FROM a.""BeginDate"") AS ""Месяц достижения"",
                        ac.""Name"" AS ""Категория достижения"",
                        COUNT(*) OVER (PARTITION BY a.""StudentId"") AS ""Всего достижений студента""
                    FROM university.""Achivment"" a
                             LEFT JOIN dictionary.""AchivmentCategory"" ac ON a.""CategoryId"" = ac.""Id""
                )

                SELECT
                    si.""Филиал"",
                    si.""Факультет"",
                    si.""Уровень подготовки"",
                    si.""Форма обучения"",
                    si.""Профиль подготовки"",
                    si.""Направление подготовки"",
                    si.""Группа"",
                    si.""Гражданство"",
                    si.""Льготы"",
                    si.""Организация"",
                    si.""Год поступления"",

                    lc.""Курс"",
                    lc.""Источник финансирования"",
                    lc.""Статус"",
                    lc.""Дата среза"",

                    a.""Дата достижения"",
                    a.""Категория достижения"",

                    a.""Всего достижений студента"",

                    CASE
                        WHEN EXTRACT(MONTH FROM a.""Дата достижения"") = 3 THEN 'Летняя сессия'
                        WHEN EXTRACT(MONTH FROM a.""Дата достижения"") = 10 THEN 'Зимняя сессия'
                        ELSE 'Обычный период'
                        END AS ""Тип периода"",

                    a.id_студента || '_' || a.""Дата достижения"" || '_' || a.""Категория достижения"" AS ""ID записи""

                FROM student_info si
                         JOIN achievements a ON si.id_студента = a.id_студента
                         LEFT JOIN last_contingent lc ON si.id_студента = lc.id_студента
                WHERE lc.id_студента IS NOT NULL
                ORDER BY a.""Всего достижений студента"" DESC, si.""Факультет"", si.""Группа"";  
                ");

            
            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Достижения"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Достижения\";");       
        }
    }
}
