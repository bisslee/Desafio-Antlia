using MediatR;

namespace ManualMovementsManager.Application.Commands.ProductCosifs.AddProductCosif
{
    public class AddProductCosifRequest : IRequest<AddProductCosifResponse>
    {
        public string ProductCode { get; set; } = null!;
        public string CosifCode { get; set; } = null!;
        public string ClassificationCode { get; set; } = string.Empty;
    }
} 