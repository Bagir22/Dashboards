using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class SemesterConfiguration: IEntityTypeConfiguration<Semester>
    {
        public void Configure(EntityTypeBuilder<Semester> builder)
        {
            builder.ToTable(nameof(Semester), "dictionary");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.BeginDate)
                .IsRequired();

            builder.Property(a => a.EndDate)
               .IsRequired();

            builder.Property(a => a.Number)
                .IsRequired();

            builder.HasOne(a => a.Group)
                .WithMany(s => s.Semesters)
                .HasForeignKey(s => s.GroupId);
        }
    }
}
