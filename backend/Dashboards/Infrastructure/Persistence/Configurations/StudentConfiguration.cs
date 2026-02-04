using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class StudentConfiguration: IEntityTypeConfiguration<Student>
    {
        public void Configure( EntityTypeBuilder<Student> builder )
        {
            builder.ToTable( nameof( Student ), "university" );
            builder.HasKey( s => s.Id );

            builder.Property( s => s.StudentExternalId )
                .IsRequired();

            builder.Property( s => s.AcademicStateId )
                .IsRequired();

            builder.Property( s => s.CitizenshipId )
                .IsRequired();

            builder.Property( s => s.Gender )
                .IsRequired();

            builder.Property( s => s.ContingentDate )
                .IsRequired();

            builder.HasIndex( s => s.StudentExternalId );
            builder.HasIndex( s => s.CitizenshipId );
            builder.HasIndex( s => s.StudyFormId );
            builder.HasIndex( s => s.FacultyId );
            builder.HasIndex( s => s.AcademicStateId );
            builder.HasIndex( s => s.ContingentDate );
            builder.HasIndex(s => new { s.StudentExternalId, s.ContingentDate })
            .IsUnique();

            builder.HasOne( s => s.Citizenship )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.CitizenshipId );

            builder.HasOne( s => s.Faculty )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.FacultyId );

            builder.HasOne( s => s.AcademicState )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.AcademicStateId );

            builder.HasOne( s => s.StudyForm )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.StudyFormId );

            builder.HasOne( s => s.EducationProgram )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.EducationProgramId );

            builder.HasOne( s => s.EducationStandard )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.EducationStandardId );

            builder.HasOne( s => s.Organization )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.OrganizationId );

            builder.HasOne( s => s.Benefit )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.BenefitId );

            builder.HasOne( s => s.AddressState )
                .WithMany( r => r.Students )
                .HasForeignKey( s => s.AddressStateId );
        }
    }
}
