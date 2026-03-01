using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrderView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW dictionary.""Приказ"" AS
            SELECT  s.""Fio"" as ""ФИО студента"",
                    c.""Name"" as ""Подкатегория"",
                    o.""Date"" as ""Дата""
            FROM university.""Order"" o
            LEFT JOIN dictionary.""Student"" s ON s.""Id"" = o.""StudentId""
            LEFT JOIN dictionary.""OrderCategory"" c ON c.""Id"" = o.""CategoryId""
        ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""Приказ"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON dictionary.\"Приказ\" FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dictionary.\"Приказ\";");
        }
    }
}
