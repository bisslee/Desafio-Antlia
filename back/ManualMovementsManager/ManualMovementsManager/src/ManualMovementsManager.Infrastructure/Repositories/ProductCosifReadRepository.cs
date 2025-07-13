using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManualMovementsManager.Infrastructure.Repositories
{
    public class ProductCosifReadRepository : ReadRepository<ProductCosif>, IProductCosifReadRepository
    {
        private readonly AppDbContext _context;
        public ProductCosifReadRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProductCosif?> GetByProductCodeAndCosifCodeAsync(string productCode, string cosifCode)
        {
            return await _context.ProductCosifs.FirstOrDefaultAsync(pc => pc.ProductCode == productCode && pc.CosifCode == cosifCode);
        }

        public async Task<List<ProductCosif>> GetByProductCodeAsync(string productCode)
        {
            return await _context.ProductCosifs.Where(pc => pc.ProductCode == productCode).ToListAsync();
        }
    }
} 