using Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW dictionary.""Справочник филиалов"" AS
                SELECT ""Name"" AS ""Название""
                FROM dictionary.""Branch"";
            ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""Справочник филиалов"" TO metabase;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"REVOKE SELECT ON dictionary.""Справочник филиалов"" FROM metabase;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS dictionary.""Справочник филиалов"";");
        }
    }
}
