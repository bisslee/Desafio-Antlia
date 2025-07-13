using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.Products
{
    public class ProductMustExistSpecification : IAsyncSpecification<string>
    {
        private readonly IProductReadRepository _productReadRepository;

        public ProductMustExistSpecification(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<bool> IsSatisfiedByAsync(string productCode)
        {
            var product = await _productReadRepository.GetByProductCodeAsync(productCode);
            return product != null;
        }

        public string ErrorMessage => "Product must exist";
    }
} 