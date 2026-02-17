using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Persistence.Migrations
{
    public static class MigrationBuilderExtensions
    {
        public static void CreateReadOnlyUser(this MigrationBuilder migrationBuilder,
            string username,
            string password,
            string database,
            params string[] tables)
        {
            migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = '{username}') THEN
                    EXECUTE 'CREATE USER {username} WITH PASSWORD ''{password}'' NOCREATEDB NOCREATEROLE';
                END IF;
            END
            $$;");

            migrationBuilder.Sql($@"
                GRANT CONNECT ON DATABASE {database} TO {username};
                GRANT USAGE ON SCHEMA public TO {username};
            ");

            foreach (var table in tables)
            {
                migrationBuilder.Sql($"GRANT SELECT ON public.\"{table}\" TO {username};");
            }
        }
    }
}
