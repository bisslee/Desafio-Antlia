using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.ManualMovements
{
    public class ManualMovementMustExistByIdSpecification : IAsyncSpecification<ManualMovement>
    {
        private readonly IManualMovementReadRepository _manualMovementReadRepository;

        public ManualMovementMustExistByIdSpecification(IManualMovementReadRepository manualMovementReadRepository)
        {
            _manualMovementReadRepository = manualMovementReadRepository;
        }

        public async Task<bool> IsSatisfiedByAsync(ManualMovement entity)
        {
            var manualMovement = await _manualMovementReadRepository.GetByIdAsync(entity.Id);
            return manualMovement != null;
        }

        public string ErrorMessage => "Manual movement must exist";
    }
} 