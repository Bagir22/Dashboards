using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class PlanConfiguration: IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTable(nameof(Plan), "university");
            builder.HasKey(sas => new {sas.SemesterId, sas.DisciplineId});

            builder.HasOne(a => a.Semester)
                .WithMany(s => s.Plans)
                .HasForeignKey(s => s.SemesterId);

            builder.HasOne(a => a.Discipline)
                .WithMany(s => s.Plans)
                .HasForeignKey(s => s.DisciplineId);
        }
    }
}
