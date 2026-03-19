using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class ContingentStudentConfiguration: IEntityTypeConfiguration<ContingentStudent>
    {
        public void Configure(EntityTypeBuilder<ContingentStudent> builder)
        {
            builder.ToTable(nameof(ContingentStudent), "university");
            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.StudentExternalId)
                .IsRequired();

            builder.Property(cs => cs.AcademicStateId)
                .IsRequired();

            builder.Property(cs => cs.ContingentDate)
                .IsRequired();

            builder.HasIndex(cs => cs.StudentExternalId);
            builder.HasIndex(cs => cs.AcademicStateId);
            builder.HasIndex(cs => cs.ContingentDate);
            builder.HasIndex(cs => new { cs.StudentExternalId, cs.ContingentDate })
            .IsUnique();

            builder.HasOne(cs => cs.Student)
                .WithMany(s => s.ContingentStudents)
                .HasForeignKey(cs => cs.StudentExternalId);

            builder.HasOne(cs => cs.AcademicState)
                .WithMany(s => s.Students)
                .HasForeignKey(cs => cs.AcademicStateId);

            builder.HasOne(cs => cs.AddressState)
                .WithMany(s => s.Students)
                .HasForeignKey(cs => cs.AddressStateId);
        }
    }
}
