using ManualMovementsManager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Repositories
{
    public interface IProductReadRepository : IReadRepository<Product>
    {
        Task<Product?> GetByProductCodeAsync(string productCode);
        Task<List<Product>> GetAllActiveAsync();
    }
} 