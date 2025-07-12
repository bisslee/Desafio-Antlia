using ManualMovementsManager.Domain.Entities.Enums;
using System;
using System.Collections.Generic;

namespace ManualMovementsManager.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string ProductCode { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ICollection<ProductCosif> ProductCosifs { get; set; } = new List<ProductCosif>();
        public virtual ICollection<ManualMovement> ManualMovements { get; set; } = new List<ManualMovement>();
    }
} 