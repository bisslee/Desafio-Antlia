using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.ProductCosifs
{
    public class ProductCosifMustExistSpecification : IAsyncSpecification<(string productCode, string cosifCode)>
    {
        private readonly IProductCosifReadRepository _productCosifReadRepository;

        public ProductCosifMustExistSpecification(IProductCosifReadRepository productCosifReadRepository)
        {
            _productCosifReadRepository = productCosifReadRepository;
        }

        public async Task<bool> IsSatisfiedByAsync((string productCode, string cosifCode) input)
        {
            var productCosif = await _productCosifReadRepository.GetByProductCodeAndCosifCodeAsync(input.productCode, input.cosifCode);
            return productCosif != null;
        }

        public string ErrorMessage => "Product COSIF must exist";
    }
} 