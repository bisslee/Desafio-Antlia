using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.ProductCosifs
{
    public class ProductCosifMustExistByIdSpecification : IAsyncSpecification<ProductCosif>
    {
        private readonly IProductCosifReadRepository _productCosifReadRepository;

        public ProductCosifMustExistByIdSpecification(IProductCosifReadRepository productCosifReadRepository)
        {
            _productCosifReadRepository = productCosifReadRepository ?? throw new ArgumentNullException(nameof(productCosifReadRepository));
        }

        public bool IsSatisfiedBy(ProductCosif productCosif)
        {
            // Para compatibilidade, mas não deve ser usado
            throw new NotImplementedException("Use IsSatisfiedByAsync for async operations");
        }

        public async Task<bool> IsSatisfiedByAsync(ProductCosif productCosif)
        {
            if (productCosif == null || productCosif.Id == Guid.Empty)
                throw new ProductCosifValidationException("Invalid product cosif ID provided.");

            var existingProductCosif = await _productCosifReadRepository.GetByIdAsync(productCosif.Id);
            
            if (existingProductCosif == null)
                throw new ProductCosifNotFoundException(productCosif.ProductCode, productCosif.CosifCode);

            return true;
        }

        public string ErrorMessage => "Product cosif not found.";
    }
} 