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
            CREATE VIEW dictionary.""Достижения"" AS
            SELECT  s.""Fio"" as ""ФИО студента"",
                    c.""Name"" as ""Категория"",
                    a.""BeginDate"" as ""Дата присвоения""
            FROM university.""Achivment"" a
            LEFT JOIN dictionary.""Student"" s ON s.""Id"" = a.""StudentId""
            LEFT JOIN dictionary.""AchivmentCategory"" c ON c.""Id"" = a.""CategoryId""
        ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""Достижения"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON dictionary.\"Достижения\" FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dictionary.\"Достижения\";");
        }
    }
}
