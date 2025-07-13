using System;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.ProductCosifs
{
    public class ProductCosifCodeMustBeUniqueForUpdateSpecification : IAsyncSpecification<ProductCosif>
    {
        private readonly IProductCosifReadRepository _productCosifReadRepository;
        private readonly Guid _excludeProductCosifId;

        public ProductCosifCodeMustBeUniqueForUpdateSpecification(IProductCosifReadRepository productCosifReadRepository, Guid excludeProductCosifId)
        {
            _productCosifReadRepository = productCosifReadRepository;
            _excludeProductCosifId = excludeProductCosifId;
        }

        public async Task<bool> IsSatisfiedByAsync(ProductCosif entity)
        {
            var existingProductCosif = await _productCosifReadRepository.GetByProductCodeAndCosifCodeAsync(entity.ProductCode, entity.CosifCode);
            return existingProductCosif == null || existingProductCosif.Id == _excludeProductCosifId;
        }

        public string ErrorMessage => "Product COSIF code must be unique for this product";
    }
} 