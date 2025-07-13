using ManualMovementsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualMovementsManager.Infrastructure.ContextMappings
{
    public class CustomerMapping: IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customer");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Configuração das propriedades
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.BirthDate).IsRequired();
            builder.Property(x => x.DocumentNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            
            // Configuração do relacionamento com Address
            builder.HasOne(x => x.Address)
                   .WithOne(x => x.Customer)
                   .HasForeignKey<Address>(x => x.CustomerId);
            
            // Configuração de índices para performance
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.DocumentNumber).IsUnique();
        }
    }
}
