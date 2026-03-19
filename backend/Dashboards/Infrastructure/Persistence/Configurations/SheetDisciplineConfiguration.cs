using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class SheetDisciplineConfiguration: IEntityTypeConfiguration<SheetDiscipline>
    {
        public void Configure(EntityTypeBuilder<SheetDiscipline> builder)
        {
            builder.ToTable(nameof(SheetDiscipline), "university");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.MarkDate);

            builder.Property(a => a.Retake)
               .IsRequired();

            builder.HasOne(a => a.Student)
                .WithMany(s => s.SheetDisciplines)
                .HasForeignKey(s => s.StudentId);

            builder.HasOne(a => a.Semester)
                .WithMany(s => s.SheetDisciplines)
                .HasForeignKey(s => s.SemesterId);

            builder.HasOne(a => a.Mark)
                .WithMany(s => s.SheetDisciplines)
                .HasForeignKey(s => s.MarkId);

            builder.HasOne(a => a.Discipline)
                .WithMany(s => s.SheetDisciplines)
                .HasForeignKey(s => s.DisciplineId);
        }
    }
}
