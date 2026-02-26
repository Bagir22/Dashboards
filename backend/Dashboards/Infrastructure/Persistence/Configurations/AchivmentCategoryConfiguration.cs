using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AchivmentCategoryConfiguration: IEntityTypeConfiguration<AchivmentCategory>
    {
        public void Configure(EntityTypeBuilder<AchivmentCategory> builder)
        {
            builder.ToTable(nameof(AchivmentCategory), "dictionary");
            builder.HasKey(ac => ac.Id);

            builder.Property(ac => ac.Name)
                .IsRequired();
        }
    }
}
