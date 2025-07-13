using Bogus;
using Bogus.Extensions.Brazil;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class ReadRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly ReadRepository<Customer> Repository;
        private readonly Faker Faker;
        public ReadRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);

            Repository = new ReadRepository<Customer>(Context);

            SeedData();
        }

        private Customer CreateCustomer()
        {
            var customerId = Guid.NewGuid();
            return new Customer
            {
                Id = customerId,
                FullName = Faker.Name.FullName(),
                Email = Faker.Internet.Email(provider: "example.com"),
                Phone = Faker.Phone.PhoneNumber(),
                Gender = Faker.PickRandom<Gender>(),
                Status = Faker.PickRandom<DataStatus>(),
                AcceptPrivacyPolicy = Faker.Random.Bool(),
                AcceptTermsUse = Faker.Random.Bool(),
                FavoriteClub = Faker.Random.String2(10),
                FavoriteSport = Faker.Random.String2(10),
                DocumentNumber = Faker.Person.Cpf(),
                BirthDate = Faker.Date.Past(Faker.Random.Int(18, 65)),
                CreatedAt = DateTime.Now,
                CreatedBy = "testes",
                UpdatedAt = DateTime.Now,
                UpdatedBy = "testes",
                Address = new Address
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Street = Faker.Address.StreetName(),
                    Number = Faker.Address.BuildingNumber(),
                    Complement = Faker.Address.SecondaryAddress(),
                    Neighborhood = Faker.Address.SecondaryAddress(),
                    City = Faker.Address.City(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = "testes",
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "testes",

                    State = Faker.Address.StateAbbr(),
                    Country = Faker.Address.Country(),
                    ZipCode = Faker.Address.ZipCode(),
                    Status = Faker.PickRandom<DataStatus>()
                }
            };
        }

        private void SeedData()
        {
            var customers = new List<Customer>();
            for (int i = 0; i < 10; i++)
            {
                Context.Customers.Add(CreateCustomer());
                Context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            var customers = await Repository.Find(x => x.Id != Guid.Empty);
            Assert.Equal(10, customers.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomer()
        {
            var firstCustomer = Context.Customers.FirstOrDefault();
            var result = await Repository.GetByIdAsync(firstCustomer.Id);
            Assert.NotNull(result);
            Assert.Equal(firstCustomer.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNullWhenIdDoesNotExist()
        {
            var result = await Repository.Find(x => x.Id == Guid.Empty);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Find_ReturnsMatchingCustomersWhenPredicateIsValid()
        {
            // Act
            var customers = await Repository.Find(u => u.Email.Contains("example.com"));

            // Assert
            Assert.NotEmpty(customers);
            Assert.Equal(10, customers.Count);
        }

        [Fact]
        public async Task Find_ReturnsEmptyListWhenNoMatchesFound()
        {
            // Act
            var customers = await Repository.Find(u => u.Email.Contains("nonexistent.com"));

            // Assert
            Assert.Empty(customers);
        }

        [Fact]
        public async Task FindWithPagination_ReturnsCorrectPageData()
        {
            // Act
            (List<Customer> pagedResult, int total) = await Repository
                .FindWithPagination(u => true, page: 1, pageSize: 2);

            // Assert
            Assert.Equal(2, pagedResult.Count);
            Assert.Equal(10, total);
        }
    }
}
