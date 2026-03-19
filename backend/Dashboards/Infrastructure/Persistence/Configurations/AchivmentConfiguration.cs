using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class AchivmentConfiguration: IEntityTypeConfiguration<Achivment>
    {
        public void Configure(EntityTypeBuilder<Achivment> builder)
        {
            builder.ToTable(nameof(Achivment), "university");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.BeginDate)
                .IsRequired();

            builder.Property(a => a.StudentId)
               .IsRequired();

            builder.Property(a => a.CategoryId)
                .IsRequired();

            builder.HasOne(a => a.Student)
                .WithMany(s => s.Achivments)
                .HasForeignKey(s => s.StudentId);

            builder.HasOne(a => a.Category)
                .WithMany(c => c.Achivments)
                .HasForeignKey(s => s.CategoryId);
        }
    }
}
