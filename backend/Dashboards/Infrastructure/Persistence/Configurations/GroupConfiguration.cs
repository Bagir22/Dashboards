using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class GroupConfiguration: IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable(nameof(Group), "dictionary");
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
