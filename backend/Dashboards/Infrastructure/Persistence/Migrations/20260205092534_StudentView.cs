using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW StudentView AS
            SELECT ep.""Name"" as ""Программа обучения"",
                    b.""Name"" as ""Льготы"",
                    adst.""Name"" as ""Регион регистрации"",
                    c.""Name"" as ""Гражданство"",
                    s.""Gender"" as ""Пол"",
                    f.""Name"" as ""Факультет"",
                    s.""Budget"" as ""Источник финансирования"",
                    s.""Course"" as ""Курс"",
                    acs.""Name"" as ""Статус"",
                    s.""ContingentDate"" as ""Дата""
    
            FROM university.""Student"" s
            LEFT JOIN dictionary.""EducationProgram"" ep ON ep.""Id"" = s.""EducationProgramId""
            LEFT JOIN dictionary.""Benefit"" b ON b.""Id"" = s.""BenefitId""
            LEFT JOIN dictionary.""AddressState"" adst ON adst.""Id"" = s.""AddressStateId""
            LEFT JOIN dictionary.""Citizenship"" c ON c.""Id"" = s.""CitizenshipId""
            LEFT JOIN dictionary.""Faculty"" f ON f.""Id"" = s.""FacultyId""
            LEFT JOIN dictionary.""AcademicState"" acs ON acs.""Id"" = s.""AcademicStateId""
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS StudentView");
        }
    }
}
