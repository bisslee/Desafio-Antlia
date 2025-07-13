using ManualMovementsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManualMovementsManager.Infrastructure.ContextMappings
{
    public class ProductCosifMapping : IEntityTypeConfiguration<ProductCosif>
    {
        public void Configure(EntityTypeBuilder<ProductCosif> builder)
        {
            builder.ToTable("PRODUTO_COSIF");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Configuração das propriedades
            builder.Property(x => x.ProductCode)
                   .HasColumnName("COD_PRODUTO")
                   .IsRequired()
                   .HasMaxLength(4)
                   .IsFixedLength();
            
            builder.Property(x => x.CosifCode)
                   .HasColumnName("COD_COSIF")
                   .IsRequired()
                   .HasMaxLength(11)
                   .IsFixedLength();
            
            builder.Property(x => x.ClassificationCode)
                   .HasColumnName("COD_CLASSIFICACAO")
                   .HasMaxLength(6)
                   .IsFixedLength();
            
            builder.Property(x => x.Status)
                   .HasColumnName("STA_STATUS")
                   .HasConversion<string>();
            
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            
            // Configuração de índices para performance
            builder.HasIndex(x => new { x.ProductCode, x.CosifCode }).IsUnique();
            
            // Configuração de relacionamentos
            builder.HasOne(x => x.Product)
                   .WithMany(x => x.ProductCosifs)
                   .HasForeignKey(x => x.ProductCode)
                   .HasPrincipalKey(x => x.ProductCode);
            
            builder.HasMany(x => x.ManualMovements)
                   .WithOne(x => x.ProductCosif)
                   .HasForeignKey(x => new { x.ProductCode, x.CosifCode })
                   .HasPrincipalKey(x => new { x.ProductCode, x.CosifCode });
        }
    }
} 