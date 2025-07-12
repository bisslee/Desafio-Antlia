using ManualMovements.Domain.Entities;
using ManualMovements.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovements.Infrastructure
{
    public class CustomerReadRepository : ReadRepository<Customer>, ICustomerReadRepository
    {
        public CustomerReadRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetCustomerWithAddressByIdAsync(Guid id)
        {
            return await Context.Set<Customer>()
                .AsNoTracking() 
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await Context.Set<Customer>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task<Customer?> GetByDocumentAsync(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return null;

            return await Context.Set<Customer>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber);
        }
    }
}
