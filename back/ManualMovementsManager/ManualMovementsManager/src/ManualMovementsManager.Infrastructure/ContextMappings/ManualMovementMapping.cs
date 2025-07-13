using ManualMovementsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManualMovementsManager.Infrastructure.ContextMappings
{
    public class ManualMovementMapping : IEntityTypeConfiguration<ManualMovement>
    {
        public void Configure(EntityTypeBuilder<ManualMovement> builder)
        {
            builder.ToTable("MOVIMENTO_MANUAL");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Configuração das propriedades
            builder.Property(x => x.Month)
                   .HasColumnName("DAT_MES")
                   .IsRequired();
            
            builder.Property(x => x.Year)
                   .HasColumnName("DAT_ANO")
                   .IsRequired();
            
            builder.Property(x => x.LaunchNumber)
                   .HasColumnName("NUM_LANCAMENTO")
                   .IsRequired();
            
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
            
            builder.Property(x => x.Description)
                   .HasColumnName("DES_DESCRICAO")
                   .IsRequired()
                   .HasMaxLength(50);
            
            builder.Property(x => x.MovementDate)
                   .HasColumnName("DAT_MOVIMENTO")
                   .IsRequired();
            
            builder.Property(x => x.UserCode)
                   .HasColumnName("COD_USUARIO")
                   .IsRequired()
                   .HasMaxLength(15);
            
            builder.Property(x => x.Value)
                   .HasColumnName("VAL_VALOR")
                   .IsRequired()
                   .HasColumnType("numeric(18,2)");
            
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            
            // Configuração de índices para performance
            builder.HasIndex(x => new { x.Month, x.Year, x.LaunchNumber, x.ProductCode, x.CosifCode }).IsUnique();
            builder.HasIndex(x => new { x.Month, x.Year });
            builder.HasIndex(x => x.MovementDate);
            
            // Configuração de relacionamentos
            builder.HasOne(x => x.Product)
                   .WithMany(x => x.ManualMovements)
                   .HasForeignKey(x => x.ProductCode)
                   .HasPrincipalKey(x => x.ProductCode);
            
            builder.HasOne(x => x.ProductCosif)
                   .WithMany(x => x.ManualMovements)
                   .HasForeignKey(x => new { x.ProductCode, x.CosifCode })
                   .HasPrincipalKey(x => new { x.ProductCode, x.CosifCode })
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
} 