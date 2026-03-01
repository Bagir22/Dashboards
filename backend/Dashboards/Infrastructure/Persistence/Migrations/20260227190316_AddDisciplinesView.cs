using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDisciplinesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW dictionary.""Справочник дисциплин"" AS
                SELECT ""Name"" AS ""Название""
                FROM dictionary.""Discipline"";
            ");

                    migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""Справочник дисциплин"" TO metabase;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"REVOKE SELECT ON dictionary.""Справочник дисциплин"" FROM metabase;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS dictionary.""Справочник дисциплин"";");
        }
    }
}
