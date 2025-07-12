using ManualMovements.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualMovements.Infrastructure.ContextMappings
{
    public class AddressMapping : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Configuração das propriedades
            builder.Property(x => x.Street).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Number).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Complement).HasMaxLength(50);
            builder.Property(x => x.Neighborhood).IsRequired().HasMaxLength(50);
            builder.Property(x => x.City).IsRequired().HasMaxLength(50);
            builder.Property(x => x.State).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Country).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ZipCode).IsRequired().HasMaxLength(10);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            
            // Configuração do relacionamento com Customer
            builder.HasOne(x => x.Customer)
                   .WithOne(x => x.Address)
                   .HasForeignKey<Address>(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            // Configuração da chave estrangeira
            builder.Property(x => x.CustomerId).IsRequired();
        }
    }
}
