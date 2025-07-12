using Bogus;
using Bogus.Extensions.Brazil;
using ManualMovements.Domain.Entities;
using ManualMovements.Domain.Entities.Enums;
using ManualMovements.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ManualMovements.UnitTest.Infrastructure
{
    public class CustomerReadRepositoryTests: BaseTest
    {
        private readonly AppDbContext Context;
        private readonly CustomerReadRepository Repository;
        private readonly Faker Faker;

        public CustomerReadRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);

            Repository = new CustomerReadRepository(Context);
            SeedData();
        }

        private void SeedData()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FullName = Faker.Name.FullName(),
                Email = Faker.Internet.Email(),
                Phone = Faker.Phone.PhoneNumber(),
                Gender = Faker.PickRandom<Gender>(),
                BirthDate = Faker.Date.Past(30),
                DocumentNumber = Faker.Person.Cpf(),
                Address = new Address
                {
                    Street = Faker.Address.StreetName(),
                    Number = Faker.Random.Number(1, 1000).ToString(),
                    Complement = Faker.Address.SecondaryAddress(),
                    City = Faker.Address.City(),
                    State = Faker.Address.StateAbbr(),
                    Country = Faker.Address.Country(),
                    Neighborhood = Faker.Address.County(),
                    ZipCode = Faker.Address.ZipCode(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                },
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            Context.Customers.Add(customer);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetCustomerWithAddressByIdAsync_Should_Return_CustomerWith_Address()
        {
            var customer = Context.Customers.First();

            var result = await Repository.GetCustomerWithAddressByIdAsync(customer.Id);

            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.NotNull(result.Address);
            Assert.False(string.IsNullOrWhiteSpace(result.Address.Street));
        }

        [Fact]
        public async Task GetCustomerWithAddressByIdAsync_Should_Return_NullWhen_NotFound()
        {
            var result = await Repository.GetCustomerWithAddressByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }

}
