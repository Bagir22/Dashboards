using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class BenefitConfiguration : IEntityTypeConfiguration<Benefit>
    {
        public void Configure( EntityTypeBuilder<Benefit> builder )
        {
            builder.ToTable( nameof( Benefit ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .IsRequired();
        }
    }
}
