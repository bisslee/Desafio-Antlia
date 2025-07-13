using ManualMovementsManager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Repositories
{
    public interface IManualMovementReadRepository : IReadRepository<ManualMovement>
    {
        Task<List<ManualMovement>> GetByMonthAndYearAsync(int month, int year);
        Task<List<ManualMovement>> GetByPeriodAsync(System.DateTime startDate, System.DateTime endDate);
        Task<List<ManualMovement>> GetByProductAsync(string productCode);
        Task<int> GetNextLaunchNumberAsync(int month, int year);
    }
} 