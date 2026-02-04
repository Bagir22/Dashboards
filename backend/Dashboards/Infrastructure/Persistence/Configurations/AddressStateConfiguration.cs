using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class AddressStateConfiguration : IEntityTypeConfiguration<AddressState>
    {
        public void Configure( EntityTypeBuilder<AddressState> builder )
        {
            builder.ToTable( nameof( AddressState ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .IsRequired();
        }
    }
}
