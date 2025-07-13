using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManualMovementsManager.Infrastructure.Repositories
{
    public class ManualMovementReadRepository : ReadRepository<ManualMovement>, IManualMovementReadRepository
    {
        private readonly AppDbContext _context;
        public ManualMovementReadRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ManualMovement>> GetByMonthAndYearAsync(int month, int year)
        {
            return await _context.ManualMovements.Where(m => m.Month == month && m.Year == year).ToListAsync();
        }

        public async Task<List<ManualMovement>> GetByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ManualMovements.Where(m => m.MovementDate >= startDate && m.MovementDate <= endDate).ToListAsync();
        }

        public async Task<List<ManualMovement>> GetByProductAsync(string productCode)
        {
            return await _context.ManualMovements.Where(m => m.ProductCode == productCode).ToListAsync();
        }

        public async Task<int> GetNextLaunchNumberAsync(int month, int year)
        {
            var last = await _context.ManualMovements.Where(m => m.Month == month && m.Year == year).OrderByDescending(m => m.LaunchNumber).FirstOrDefaultAsync();
            return last?.LaunchNumber + 1 ?? 1;
        }
    }
} 