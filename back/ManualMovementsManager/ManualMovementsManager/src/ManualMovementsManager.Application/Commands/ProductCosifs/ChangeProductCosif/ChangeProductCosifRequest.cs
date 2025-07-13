using MediatR;
using System;

namespace ManualMovementsManager.Application.Commands.ProductCosifs.ChangeProductCosif
{
    public class ChangeProductCosifRequest : IRequest<ChangeProductCosifResponse>
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = null!;
        public string CosifCode { get; set; } = null!;
        public string ClassificationCode { get; set; } = string.Empty;
    }
} 