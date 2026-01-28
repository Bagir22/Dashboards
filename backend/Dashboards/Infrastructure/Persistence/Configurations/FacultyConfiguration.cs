using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class FacultyConfiguration: IEntityTypeConfiguration<Faculty>
    {
        public void Configure( EntityTypeBuilder<Faculty> builder )
        {
            builder.ToTable( nameof( Faculty ), "dictionary" );
            builder.HasKey( f => f.Id );

            builder.Property( f => f.Name )
                .HasMaxLength( 100 )
                .IsRequired();
        }
    }
}
