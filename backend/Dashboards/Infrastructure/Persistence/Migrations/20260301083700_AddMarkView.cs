using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMarkView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
               CREATE OR REPLACE VIEW dictionary.""Справочник оценок"" AS
                    SELECT 
                        ""Name"" AS ""Название"", 
                        ""Value"" AS ""Значение"",
                        CASE 
                            WHEN ""IsGoodMark"" = TRUE THEN 'Да' 
                            ELSE 'Нет' 
                        END AS ""Хорошая оценка""
                    FROM dictionary.""Mark"";
            ");

                    migrationBuilder.Sql(@"
                GRANT USAGE ON SCHEMA dictionary TO metabase;
                GRANT SELECT ON dictionary.""Справочник оценок"" TO metabase;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("REVOKE SELECT ON dictionary.\"Справочник оценок\" FROM metabase;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dictionary.\"Справочник оценок\";");
        }

    }
}
