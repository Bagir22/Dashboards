using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MarkConfiguration: IEntityTypeConfiguration<Mark>
    {
        public void Configure( EntityTypeBuilder<Mark> builder )
        {
            builder.ToTable( nameof( Mark ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .HasMaxLength( 100 )
                .IsRequired();
            
            builder.Property( sas => sas.Value )
                .IsRequired();
            
            builder.Property( sas => sas.IsGoodMark )
                .HasDefaultValue(false);
        }
    }
}
