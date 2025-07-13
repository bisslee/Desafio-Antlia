using System;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.Products
{
    public class ProductCodeMustBeUniqueForUpdateSpecification : IAsyncSpecification<Product>
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly Guid _excludeProductId;

        public ProductCodeMustBeUniqueForUpdateSpecification(IProductReadRepository productReadRepository, Guid excludeProductId)
        {
            _productReadRepository = productReadRepository;
            _excludeProductId = excludeProductId;
        }

        public async Task<bool> IsSatisfiedByAsync(Product entity)
        {
            var existingProduct = await _productReadRepository.GetByProductCodeAsync(entity.ProductCode);
            return existingProduct == null || existingProduct.Id == _excludeProductId;
        }

        public string ErrorMessage => "Product code must be unique";
    }
} 