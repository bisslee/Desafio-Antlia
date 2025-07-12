using ManualMovementsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManualMovementsManager.Infrastructure.ContextMappings
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("PRODUTO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Configuração das propriedades
            builder.Property(x => x.ProductCode)
                   .HasColumnName("COD_PRODUTO")
                   .IsRequired()
                   .HasMaxLength(4)
                   .IsFixedLength();
            
            builder.Property(x => x.Description)
                   .HasColumnName("DES_PRODUTO")
                   .HasMaxLength(30);
            
            builder.Property(x => x.Status)
                   .HasColumnName("STA_STATUS")
                   .HasConversion<string>();
            
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            
            // Configuração de índices para performance
            builder.HasIndex(x => x.ProductCode).IsUnique();
            
            // Configuração de relacionamentos
            builder.HasMany(x => x.ProductCosifs)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductCode)
                   .HasPrincipalKey(x => x.ProductCode);
            
            builder.HasMany(x => x.ManualMovements)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductCode)
                   .HasPrincipalKey(x => x.ProductCode);
        }
    }
} 