using System;

namespace ManualMovementsManager.Application.DTOs
{
    public class ProductCosifDto
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = null!;
        public string CosifCode { get; set; } = null!;
        public string ClassificationCode { get; set; } = string.Empty;
    }
} 