using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class StudyFormConfiguration: IEntityTypeConfiguration<StudyForm>
    {
        public void Configure( EntityTypeBuilder<StudyForm> builder )
        {
            builder.ToTable( nameof( StudyForm ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .HasMaxLength( 100 )
                .IsRequired();
        }
    }
}
