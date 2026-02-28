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
                CREATE OR REPLACE VIEW dictionary.""BranchView"" AS
                SELECT ""Name""
                FROM dictionary.""Branch"";
            ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""BranchView"" TO metabase;
            ");
        }
        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON dictionary.BranchView FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dictionary.BranchView;");
        }
    }
}
