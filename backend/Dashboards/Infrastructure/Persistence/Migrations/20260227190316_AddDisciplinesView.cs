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
                CREATE OR REPLACE VIEW dictionary.""DisciplineView"" AS
                SELECT ""Name""
                FROM dictionary.""Discipline"";
            ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""DisciplineView"" TO metabase;
            ");
        }
        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON dictionary.DisciplineView FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dictionary.DisciplineView;");
        }
    }
}
