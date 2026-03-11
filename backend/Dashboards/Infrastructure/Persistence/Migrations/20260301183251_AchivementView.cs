using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AchivementView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW university.""Достижения"" AS
            SELECT  s.""Fio"" as ""ФИО студента"",
                    c.""Name"" as ""Категория"",
                    a.""BeginDate"" as ""Дата присвоения""
            FROM university.""Achivment"" a
            LEFT JOIN university.""Student"" s ON s.""Id"" = a.""StudentId""
            LEFT JOIN dictionary.""AchivmentCategory"" c ON c.""Id"" = a.""CategoryId""
        ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Достижения"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON university.\"Достижения\" FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Достижения\";");
        }
    }
}
