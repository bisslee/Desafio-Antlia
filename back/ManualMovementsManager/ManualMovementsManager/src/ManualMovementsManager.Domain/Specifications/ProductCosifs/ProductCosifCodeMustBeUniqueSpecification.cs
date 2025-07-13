using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.ProductCosifs
{
    public class ProductCosifCodeMustBeUniqueSpecification : IAsyncSpecification<ProductCosif>
    {
        private readonly IProductCosifReadRepository _productCosifReadRepository;

        public ProductCosifCodeMustBeUniqueSpecification(IProductCosifReadRepository productCosifReadRepository)
        {
            _productCosifReadRepository = productCosifReadRepository;
        }

        public async Task<bool> IsSatisfiedByAsync(ProductCosif entity)
        {
            var existingProductCosif = await _productCosifReadRepository.GetByProductCodeAndCosifCodeAsync(entity.ProductCode, entity.CosifCode);
            return existingProductCosif == null;
        }

        public string ErrorMessage => "Product COSIF code must be unique for this product";
    }
} 