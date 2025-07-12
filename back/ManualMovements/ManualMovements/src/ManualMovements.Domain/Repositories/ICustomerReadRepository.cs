using ManualMovements.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ManualMovements.Domain.Repositories
{
    public interface ICustomerReadRepository : IReadRepository<Customer>
    {
        Task<Customer?> GetCustomerWithAddressByIdAsync(Guid id);
        
        /// <summary>
        /// Busca customer por email
        /// </summary>
        /// <param name="email">Email do customer</param>
        /// <returns>Customer encontrado ou null</returns>
        Task<Customer?> GetByEmailAsync(string email);
        
        /// <summary>
        /// Busca customer por documento
        /// </summary>
        /// <param name="documentNumber">Número do documento</param>
        /// <returns>Customer encontrado ou null</returns>
        Task<Customer?> GetByDocumentAsync(string documentNumber);
    }
}
