using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts
{
    public interface IUniDashDbContext
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

        Task<int> SaveChangesAsync( CancellationToken ct = default );
    }
}
