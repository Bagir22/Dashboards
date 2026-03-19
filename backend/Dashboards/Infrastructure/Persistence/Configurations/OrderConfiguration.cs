using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class OrderConfiguration: IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order), "university");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Date)
                .IsRequired();

            builder.Property(o => o.StudentId)
               .IsRequired();

            builder.Property(o => o.CategoryId)
                .IsRequired();

            builder.HasOne(o => o.Student)
                .WithMany(s => s.Orders)
                .HasForeignKey(s => s.StudentId);

            builder.HasOne(o => o.Category)
                .WithMany(c => c.Orders)
                .HasForeignKey(s => s.CategoryId);
        }
    }
}
