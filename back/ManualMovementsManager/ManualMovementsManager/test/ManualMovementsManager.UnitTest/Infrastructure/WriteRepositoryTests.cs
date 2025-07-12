using Bogus;
using Bogus.Extensions.Brazil;
using Castle.Core.Resource;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class WriteRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly WriteRepository<Customer> Repository;
        private readonly Faker Faker;

        public WriteRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite("Filename=:memory:") // Banco de dados em memória do SQLite
               .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);
            Context.Database.OpenConnection();
            Context.Database.EnsureCreated();
            Repository = new WriteRepository<Customer>(Context);
        }

        private Customer CreateCustomer()
        {
            var customerId = Guid.NewGuid();
            return new Customer
            {
                Id = customerId,
                FullName = Faker.Name.FullName(),
                Email = Faker.Internet.Email(),
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

        [Fact]
        public async Task AddCustomer()
        {

            var customer = CreateCustomer();

            var result = await Repository.Add(customer);

            Assert.True(result);
            Assert.NotNull(await Context.Customers.FindAsync(customer.Id));
        }

        [Fact]
        public async Task Add_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Add(null));
        }

        [Fact]
        public async Task UpdateCustomer()
        {
            var customer = CreateCustomer();

            await Repository.Add(customer);

            var newEmail = Faker.Internet.Email();
            var newPhone = Faker.Phone.PhoneNumber();

            customer.Email = newEmail;
            customer.Phone = newPhone;

            var result = await Repository.Update(customer);

            Assert.True(result);
            var updatedCustomer = await Context.Customers.FindAsync(customer.Id);
            Assert.NotNull(updatedCustomer);
            Assert.Equal(newEmail, updatedCustomer.Email);
            Assert.Equal(newPhone, updatedCustomer.Phone);
        }

        [Fact]
        public async Task Update_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Update(null));
        }

        [Fact]
        public async Task RemoveCustomer()
        {
            var customer = CreateCustomer();
            await Repository.Add(customer);
            var result = await Repository.Delete(customer);
            Assert.True(result);
            Assert.Null(await Context.Customers.FindAsync(customer.Id));
        }

        [Fact]
        public async Task Remove_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Delete(null));
        }

        [Fact]
        public async Task ExecuteSql_ExecutesRawSqlCommand()
        {
            // Arrange
            var sql = "Select @p0 as id, @p1 as name, @p2 as name";
            var userId = Guid.NewGuid();
            var fullName = "SQL User";
            var email = "sqluser@example.com";

            var parameters = new object[] { userId, fullName, email };

            // Act
            var result = await Repository.ExecuteSql(sql, parameters);

            // Assert
            Assert.True(result);
        }
    }
}

