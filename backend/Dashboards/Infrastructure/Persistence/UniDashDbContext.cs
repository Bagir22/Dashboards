using System.Reflection;
using Application.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class UniDashDbContext( DbContextOptions<UniDashDbContext> options ): DbContext( options ), IUniDashDbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<ContingentStudent> ContingentStudents { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Citizenship> Citizenships { get; set; }
        public DbSet<AcademicState> StudentAcademicStates { get; set; }
        public DbSet<StudyForm> StudyForms { get; set; }
        public DbSet<EducationProgram> EducationPrograms { get; set; }
        public DbSet<EducationStandard> EducationStandards { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AddressState> AddressStates { get; set; }
        public DbSet<Achivment> Achivments { get; set; }
        public DbSet<AchivmentCategory> AchivmentCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderCategory> OrderCategories { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<TrainingLevel> TrainingLevels { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<SheetDiscipline> SheetDisciplines { get; set; }
        public DbSet<Plan> Plans { get; set; }

        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        {
            optionsBuilder.LogTo( Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information );
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            modelBuilder.ApplyConfigurationsFromAssembly( Assembly.GetExecutingAssembly() );
        }
    }
}
