using ManualMovementsManager.Domain.Entities.Enums;
using System;

namespace ManualMovementsManager.Domain.Entities
{
    public class ManualMovement : BaseEntity
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int LaunchNumber { get; set; }
        public string ProductCode { get; set; } = null!;
        public string CosifCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime MovementDate { get; set; }
        public string UserCode { get; set; } = null!;
        public decimal Value { get; set; }
        
        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual ProductCosif ProductCosif { get; set; } = null!;
    }
} 