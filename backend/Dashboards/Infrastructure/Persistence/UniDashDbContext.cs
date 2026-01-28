using System.Reflection;
using Application.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class UniDashDbContext( DbContextOptions<UniDashDbContext> options ): DbContext( options ), IUniDashDbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Citizenship> Citizenships { get; set; }
        public DbSet<AcademicState> StudentAcademicStates { get; set; }
        public DbSet<StudyForm> StudyForms { get; set; }
        public DbSet<EducationProgram> EducationPrograms { get; set; }
        public DbSet<EducationStandard> EducationStandards { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AddressState> AddressStates { get; set; }

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
