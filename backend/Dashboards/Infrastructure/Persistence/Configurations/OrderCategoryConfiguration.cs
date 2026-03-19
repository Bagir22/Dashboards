using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class OrderCategoryConfiguration : IEntityTypeConfiguration<OrderCategory>
    {
        public void Configure(EntityTypeBuilder<OrderCategory> builder)
        {
            builder.ToTable(nameof(OrderCategory), "dictionary");
            builder.HasKey(ac => ac.Id);

            builder.Property(ac => ac.Name)
                .IsRequired();
        }
    }
}
