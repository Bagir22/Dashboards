using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class CitizenshipConfiguration: IEntityTypeConfiguration<Citizenship>
    {
        public void Configure( EntityTypeBuilder<Citizenship> builder )
        {
            builder.ToTable( nameof( Citizenship ), "dictionary" );
            builder.HasKey( c => c.Id );

            builder.Property( c => c.Name )
                .HasMaxLength( 100 )
                .IsRequired();
        }
    }
}
