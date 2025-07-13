using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.Products
{
    public class ProductCodeMustBeUniqueSpecification : IAsyncSpecification<Product>
    {
        private readonly IProductReadRepository _productReadRepository;

        public ProductCodeMustBeUniqueSpecification(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<bool> IsSatisfiedByAsync(Product entity)
        {
            var existingProduct = await _productReadRepository.GetByProductCodeAsync(entity.ProductCode);
            return existingProduct == null;
        }

        public string ErrorMessage => "Product code must be unique";
    }
} 