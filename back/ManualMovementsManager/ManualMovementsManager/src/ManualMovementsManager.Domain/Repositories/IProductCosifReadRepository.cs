using ManualMovementsManager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Repositories
{
    public interface IProductCosifReadRepository : IReadRepository<ProductCosif>
    {
        Task<ProductCosif?> GetByProductCodeAndCosifCodeAsync(string productCode, string cosifCode);
        Task<List<ProductCosif>> GetByProductCodeAsync(string productCode);
    }
} 