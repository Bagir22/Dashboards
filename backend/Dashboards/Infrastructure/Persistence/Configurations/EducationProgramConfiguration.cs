using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class EducationProgramConfiguration : IEntityTypeConfiguration<EducationProgram>
    {
        public void Configure( EntityTypeBuilder<EducationProgram> builder )
        {
            builder.ToTable( nameof( EducationProgram ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .IsRequired();
        }
    }
}
