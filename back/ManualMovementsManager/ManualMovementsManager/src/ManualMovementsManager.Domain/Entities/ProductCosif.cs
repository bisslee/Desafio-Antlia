using ManualMovementsManager.Domain.Entities.Enums;
using System;
using System.Collections.Generic;

namespace ManualMovementsManager.Domain.Entities
{
    public class ProductCosif : BaseEntity
    {
        public string ProductCode { get; set; } = null!;
        public string CosifCode { get; set; } = null!;
        public string ClassificationCode { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<ManualMovement> ManualMovements { get; set; } = new List<ManualMovement>();
    }
} 