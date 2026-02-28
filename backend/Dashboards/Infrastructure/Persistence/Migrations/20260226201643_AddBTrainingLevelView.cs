using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBTrainingLevelView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW dictionary.""TrainingLevelView"" AS
                SELECT ""Name""
                FROM dictionary.""TrainingLevel"";
            ");

            migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""TrainingLevelView"" TO metabase;
            ");
        }
        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON dictionary.TrainingLevelView FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dictionary.TrainingLevelView;");
        }
    }
}
