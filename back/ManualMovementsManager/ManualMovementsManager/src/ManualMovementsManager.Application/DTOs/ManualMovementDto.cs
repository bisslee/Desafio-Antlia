using System;

namespace ManualMovementsManager.Application.DTOs
{
    public class ManualMovementDto
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int LaunchNumber { get; set; }
        public string ProductCode { get; set; } = null!;
        public string CosifCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime MovementDate { get; set; }
        public string UserCode { get; set; } = null!;
        public decimal Value { get; set; }
    }
} 