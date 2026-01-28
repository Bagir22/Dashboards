using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class AcademicStateConfiguration: IEntityTypeConfiguration<AcademicState>
    {
        public void Configure( EntityTypeBuilder<AcademicState> builder )
        {
            builder.ToTable( nameof( AcademicState ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .HasMaxLength( 100 )
                .IsRequired();
        }
    }
}
