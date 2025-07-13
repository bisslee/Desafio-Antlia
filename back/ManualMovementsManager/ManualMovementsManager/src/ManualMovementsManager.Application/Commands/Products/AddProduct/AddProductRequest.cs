using MediatR;

namespace ManualMovementsManager.Application.Commands.Products.AddProduct
{
    public class AddProductRequest : IRequest<AddProductResponse>
    {
        public string ProductCode { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }
} 