using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManualMovementsManager.Infrastructure.Repositories
{
    public class ProductReadRepository : ReadRepository<Product>, IProductReadRepository
    {
        private readonly AppDbContext _context;
        public ProductReadRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetByProductCodeAsync(string productCode)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductCode == productCode);
        }

        public async Task<List<Product>> GetAllActiveAsync()
        {
            return await _context.Products.Where(p => p.Status == DataStatus.Active).ToListAsync();
        }
    }
} 