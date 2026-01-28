using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class EducationStandardConfiguration : IEntityTypeConfiguration<EducationStandard>
    {
        public void Configure( EntityTypeBuilder<EducationStandard> builder )
        {
            builder.ToTable( nameof( EducationStandard ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .IsRequired();
        }
    }
}
