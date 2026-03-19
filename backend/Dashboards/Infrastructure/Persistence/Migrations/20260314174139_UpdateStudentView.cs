using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"История студента\";");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW university.""История студента"" AS
                SELECT
                    adst.""Name"" AS ""Регион регистрации"",

                    s.""Budget"" AS ""Источник финансирования"",
                    s.""Course"" AS ""Курс"",
                    acs.""Name"" AS ""Статус"",
                    s.""Ball"" AS ""Средний балл"",
                    s.""ContingentDate"" AS ""Дата"",

                    f.""Name"" AS ""Факультет"",
                    br.""Name"" AS ""Филиал"",

                    c.""Name"" AS ""Гражданство"",
                    b.""Name"" AS ""Льготы"",
                    g.""Name"" AS ""Группа"",

                    tl.""Name"" AS ""Уровень подготовки"",
                    sf.""Name"" AS ""Форма обучения"",         
                    ep.""Name"" AS ""Профиль подготовки"",
                    es.""Name"" AS ""Направление подготовки"",

                    o.""Name"" AS ""Организация"",

                    CASE
                        WHEN EXTRACT(MONTH FROM s.""ContingentDate"") = 3 THEN 'Летняя сессия'
                        WHEN EXTRACT(MONTH FROM s.""ContingentDate"") = 10 THEN 'Зимняя сессия'
                        ELSE 'Обычный срез'
                        END AS ""Тип среза""

                FROM university.""ContingentStudent"" s
                         LEFT JOIN dictionary.""AddressState"" adst ON adst.""Id"" = s.""AddressStateId""
                         LEFT JOIN dictionary.""AcademicState"" acs ON acs.""Id"" = s.""AcademicStateId""
                         LEFT JOIN university.""Student"" st ON st.""Id"" = s.""StudentExternalId""
                         LEFT JOIN dictionary.""Faculty"" f ON st.""FacultyId"" = f.""Id""
                         LEFT JOIN dictionary.""Citizenship"" c ON st.""CitizenshipId"" = c.""Id""
                         LEFT JOIN dictionary.""Benefit"" b ON st.""BenefitId"" = b.""Id""
                         LEFT JOIN dictionary.""EducationProgram"" ep ON ep.""Id"" = st.""EducationProgramId""
                         LEFT JOIN dictionary.""EducationStandard"" es ON es.""Id"" = st.""EducationStandardId""
                         LEFT JOIN dictionary.""Group"" g ON g.""Id"" = st.""GroupId""
                         LEFT JOIN dictionary.""Branch"" br ON br.""Id"" = st.""BranchId""
                         LEFT JOIN dictionary.""Organization"" o ON o.""Id"" = st.""OrganizationId""
                         LEFT JOIN dictionary.""TrainingLevel"" tl ON tl.""Id"" = st.""TrainingLevelId""    
                         LEFT JOIN dictionary.""StudyForm"" sf ON sf.""Id"" = st.""StudyFormId"";   
                ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""История студента"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"История студента\";");
        }
    }
}
