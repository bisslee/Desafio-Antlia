using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.ManualMovements
{
    public class ManualMovementMustExistByIdSpecification : IAsyncSpecification<ManualMovement>
    {
        private readonly IManualMovementReadRepository _manualMovementReadRepository;

        public ManualMovementMustExistByIdSpecification(IManualMovementReadRepository manualMovementReadRepository)
        {
            _manualMovementReadRepository = manualMovementReadRepository ?? throw new ArgumentNullException(nameof(manualMovementReadRepository));
        }

        public bool IsSatisfiedBy(ManualMovement manualMovement)
        {
            // Para compatibilidade, mas não deve ser usado
            throw new NotImplementedException("Use IsSatisfiedByAsync for async operations");
        }

        public async Task<bool> IsSatisfiedByAsync(ManualMovement manualMovement)
        {
            if (manualMovement == null || manualMovement.Id == Guid.Empty)
                throw new ManualMovementValidationException("Invalid manual movement ID provided.");

            var existingManualMovement = await _manualMovementReadRepository.GetByIdAsync(manualMovement.Id);
            
            if (existingManualMovement == null)
                throw new ManualMovementNotFoundException(manualMovement.Id);

            return true;
        }

        public string ErrorMessage => "Manual movement not found.";
    }
} 