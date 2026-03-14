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
            migrationBuilder.Sql("REVOKE SELECT ON university.\"История студента\" FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"История студента\";");

            migrationBuilder.Sql(@"
                CREATE VIEW university.""История студента"" AS
                SELECT adst.""Name""        AS ""Регион регистрации"",
                    s.""Budget""         AS ""Источник финансирования"",
                    s.""Course""         AS ""Курс"",
                    acs.""Name""         AS ""Статус"",
                    s.""Ball""           AS ""Средний балл"",
                    s.""ContingentDate"" AS ""Дата"",
                    f.""Name""           AS ""Факультет"",
                    c.""Name""           AS ""Граждантсво"",
                    b.""Name""           AS ""Льготы"",
                    ep.""Name""          AS ""Профиль подготовки"",
                    es.""Name""          AS ""Направление подготовки"",
                    g.""Name""           AS ""Группа"",
                    br.""Name""          AS ""Филилал"",
                    o.""Name""           AS ""Организация""
                FROM university.""ContingentStudent"" s
                    LEFT JOIN dictionary.""AddressState"" adst ON adst.""Id"" = s.""AddressStateId""
                    LEFT JOIN dictionary.""AcademicState"" acs ON acs.""Id"" = s.""AcademicStateId""
                    LEFT JOIN university.""Student"" st ON st.""Id"" = s.""StudentExternalId""
                    LEFT JOIN dictionary.""Faculty"" f ON st.""FacultyId"" = f.""Id""
                    LEFT JOIN dictionary.""Citizenship"" c on st.""CitizenshipId"" = c.""Id""
                    LEFT JOIN dictionary.""Benefit"" b on st.""BenefitId"" = b.""Id""
                    LEFT JOIN dictionary.""EducationProgram"" ep ON ep.""Id"" = st.""EducationProgramId""
                    LEFT JOIN dictionary.""EducationStandard"" es ON es.""Id"" = st.""EducationStandardId""
                    LEFT JOIN dictionary.""Group"" g ON g.""Id"" = st.""GroupId""
                    LEFT JOIN dictionary.""Branch"" br ON br.""Id"" = st.""BranchId""
                    LEFT JOIN dictionary.""Organization"" o ON o.""Id"" = st.""OrganizationId"";
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
