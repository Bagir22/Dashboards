using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DisciplineConfiguration: IEntityTypeConfiguration<Discipline>
    {
        public void Configure( EntityTypeBuilder<Discipline> builder )
        {
            builder.ToTable( nameof( Discipline ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .HasMaxLength( 500 )
                .IsRequired();
        }
    }
}
