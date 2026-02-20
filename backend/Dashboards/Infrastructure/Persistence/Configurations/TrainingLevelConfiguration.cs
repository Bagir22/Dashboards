using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TrainingLevelConfiguration: IEntityTypeConfiguration<TrainingLevel>
    {
        public void Configure( EntityTypeBuilder<TrainingLevel> builder )
        {
            builder.ToTable( nameof( TrainingLevel ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .HasMaxLength( 100 )
                .IsRequired();
        }
    }
}
