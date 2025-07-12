using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.Products
{
    public class ProductMustExistByIdSpecification : IAsyncSpecification<Product>
    {
        private readonly IProductReadRepository _productReadRepository;

        public ProductMustExistByIdSpecification(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository ?? throw new ArgumentNullException(nameof(productReadRepository));
        }

        public bool IsSatisfiedBy(Product product)
        {
            // Para compatibilidade, mas não deve ser usado
            throw new NotImplementedException("Use IsSatisfiedByAsync for async operations");
        }

        public async Task<bool> IsSatisfiedByAsync(Product product)
        {
            if (product == null || product.Id == Guid.Empty)
                throw new ProductNotFoundException("Invalid product ID provided.");

            var existingProduct = await _productReadRepository.GetByIdAsync(product.Id);
            
            if (existingProduct == null)
                throw new ProductNotFoundException(product.Id);

            return true;
        }

        public string ErrorMessage => "Product not found.";
    }
} 