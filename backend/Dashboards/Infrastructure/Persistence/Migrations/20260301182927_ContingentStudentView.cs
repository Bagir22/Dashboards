using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContingentStudentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW dictionary.""История студента"" AS
            SELECT  adst.""Name"" as ""Регион регистрации"",
                    s.""Budget"" as ""Источник финансирования"",
                    s.""Course"" as ""Курс"",
                    acs.""Name"" as ""Статус"",
                    s.""Ball"" as ""Средний балл"",
                    s.""ContingentDate"" as ""Дата""
            FROM university.""Student"" s
            LEFT JOIN dictionary.""AddressState"" adst ON adst.""Id"" = s.""AddressStateId""
            LEFT JOIN dictionary.""AcademicState"" acs ON acs.""Id"" = s.""AcademicStateId""
        ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""История студента"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON dictionary.\"История студента\" FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dictionary.\"История студента\";");
        }
    }
}
