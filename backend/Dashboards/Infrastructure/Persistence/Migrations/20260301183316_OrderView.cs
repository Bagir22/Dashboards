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
            CREATE VIEW university.""Приказ"" AS
            SELECT  s.""Fio"" as ""ФИО студента"",
                    c.""Name"" as ""Подкатегория"",
                    o.""Date"" as ""Дата""
            FROM university.""Order"" o
            LEFT JOIN university.""Student"" s ON s.""Id"" = o.""StudentId""
            LEFT JOIN dictionary.""OrderCategory"" c ON c.""Id"" = o.""CategoryId""
        ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA university TO metabase;
                GRANT SELECT ON university.""Приказ"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON university.\"Приказ\" FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS university.\"Приказ\";");
        }
    }
}
