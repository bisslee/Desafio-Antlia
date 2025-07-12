using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.ProductCosifs
{
    public class ProductCosifMustExistByIdSpecification : IAsyncSpecification<ProductCosif>
    {
        private readonly IProductCosifReadRepository _productCosifReadRepository;

        public ProductCosifMustExistByIdSpecification(IProductCosifReadRepository productCosifReadRepository)
        {
            _productCosifReadRepository = productCosifReadRepository;
        }

        public async Task<bool> IsSatisfiedByAsync(ProductCosif entity)
        {
            var productCosif = await _productCosifReadRepository.GetByIdAsync(entity.Id);
            return productCosif != null;
        }

        public string ErrorMessage => "Product COSIF must exist";
    }
} 