using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BranchConfiguration: IEntityTypeConfiguration<Branch>
    {
        public void Configure( EntityTypeBuilder<Branch> builder )
        {
            builder.ToTable( nameof( Branch ), "dictionary" );
            builder.HasKey( sas => sas.Id );

            builder.Property( sas => sas.Name )
                .HasMaxLength( 100 )
                .IsRequired();
        }
    }
}
