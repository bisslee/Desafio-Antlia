using System;

namespace ManualMovementsManager.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }
} 