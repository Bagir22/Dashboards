using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGradePointAverageView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Средний балл\";");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW university.""Средний балл"" AS
                    SELECT
                        f.""Name"" AS ""Факультет"",
                        cs.""ContingentDate"" AS ""Дата"",
                        cs.""Ball"" AS ""Средний балл"",
                        a_s.""Name"" AS ""Статус студента"",

                        CASE
                            WHEN EXTRACT(MONTH FROM cs.""ContingentDate"") = 3 THEN 'летняя сессия'
                            WHEN EXTRACT(MONTH FROM cs.""ContingentDate"") = 10 THEN 'зимняя сессия'
                            ELSE 'обычный срез'
                            END AS ""Тип среза"",

                        ep.""Name"" AS ""Профиль подготовки"",
                        es.""Name"" AS ""Направление подготовки"",
                        
                        tl.""Name"" AS ""Уровень подготовки"",
                        sf.""Name"" AS ""Форма обучения"",
                        cs.""Course"" AS ""Курс"",
                        cs.""Budget"" AS ""Источник финансирования""

                    FROM university.""Student"" s
                             LEFT JOIN dictionary.""Faculty"" f ON s.""FacultyId"" = f.""Id""
                             LEFT JOIN dictionary.""EducationStandard"" es ON s.""EducationStandardId"" = es.""Id""
                             LEFT JOIN dictionary.""EducationProgram"" ep ON s.""EducationProgramId"" = ep.""Id""
                             LEFT JOIN dictionary.""TrainingLevel"" tl ON s.""TrainingLevelId"" = tl.""Id""           
                             LEFT JOIN dictionary.""StudyForm"" sf ON s.""StudyFormId"" = sf.""Id""
                             LEFT JOIN university.""ContingentStudent"" cs ON s.""Id"" = cs.""StudentExternalId""
                             LEFT JOIN dictionary.""AcademicState"" a_s ON cs.""AcademicStateId"" = a_s.""Id""
                    WHERE cs.""ContingentDate"" IS NOT NULL;
                ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Средний балл"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Средний балл\";");
        }
    }
}
