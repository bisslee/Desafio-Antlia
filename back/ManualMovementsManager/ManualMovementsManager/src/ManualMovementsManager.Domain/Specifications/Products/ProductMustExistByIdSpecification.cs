using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.Products
{
    public class ProductMustExistByIdSpecification : IAsyncSpecification<Product>
    {
        private readonly IProductReadRepository _productReadRepository;

        public ProductMustExistByIdSpecification(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<bool> IsSatisfiedByAsync(Product entity)
        {
            var product = await _productReadRepository.GetByIdAsync(entity.Id);
            return product != null;
        }

        public string ErrorMessage => "Product must exist";
    }
} 