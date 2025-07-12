using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ManualMovementsManager.Domain.Specifications.Customers
{
    /// <summary>
    /// Specification que valida se o documento do customer é único
    /// </summary>
    public class CustomerDocumentMustBeUniqueSpecification : IAsyncSpecification<ManualMovementsManager.Domain.Entities.Customer>
    {
        private readonly ICustomerReadRepository CustomerRepository;
        private readonly Guid? ExcludeCustomerId;

        public CustomerDocumentMustBeUniqueSpecification(ICustomerReadRepository customerRepository, Guid? excludeCustomerId = null)
        {
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            ExcludeCustomerId = excludeCustomerId;
        }

        public bool IsSatisfiedBy(ManualMovementsManager.Domain.Entities.Customer customer)
        {
            // Para compatibilidade, mas não deve ser usado
            throw new NotImplementedException("Use IsSatisfiedByAsync for async operations");
        }

        public async Task<bool> IsSatisfiedByAsync(ManualMovementsManager.Domain.Entities.Customer customer)
        {
            if (customer == null || string.IsNullOrWhiteSpace(customer.DocumentNumber))
                return true;

            var existingCustomer = await CustomerRepository.GetByDocumentAsync(customer.DocumentNumber);
            
            if (existingCustomer == null)
                return true;

            if (ExcludeCustomerId.HasValue && 
                existingCustomer.Id == ExcludeCustomerId.Value)
                return true;

            throw new CustomerDocumentAlreadyExistsException(customer.DocumentNumber, existingCustomer.Id);
        }

        public string ErrorMessage => "Document already exists.";
    }
} 