using Bogus;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class ManualMovementReadRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly ManualMovementReadRepository Repository;
        private readonly Faker Faker;

        public ManualMovementReadRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);

            Repository = new ManualMovementReadRepository(Context);
            SeedData();
        }

        private void SeedData()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                ProductCode = Faker.Random.AlphaNumeric(10),
                Description = Faker.Commerce.ProductName(),
                Status = DataStatus.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = "testes",
                UpdatedAt = DateTime.Now,
                UpdatedBy = "testes"
            };

            var productCosif = new ProductCosif
            {
                Id = Guid.NewGuid(),
                ProductCode = product.ProductCode,
                CosifCode = Faker.Random.AlphaNumeric(10),
                ClassificationCode = Faker.Random.AlphaNumeric(5),
                Status = DataStatus.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = "testes",
                UpdatedAt = DateTime.Now,
                UpdatedBy = "testes"
            };

            var manualMovement = new ManualMovement
            {
                Id = Guid.NewGuid(),
                Month = Faker.Random.Int(1, 12),
                Year = Faker.Random.Int(2020, 2024),
                LaunchNumber = 1,
                ProductCode = product.ProductCode,
                CosifCode = productCosif.CosifCode,
                Description = Faker.Lorem.Sentence(),
                MovementDate = DateTime.Now,
                UserCode = "TESTUSER",
                Value = Faker.Random.Decimal(100, 10000),
                CreatedAt = DateTime.Now,
                CreatedBy = "testes",
                UpdatedAt = DateTime.Now,
                UpdatedBy = "testes"
            };

            Context.Products.Add(product);
            Context.ProductCosifs.Add(productCosif);
            Context.ManualMovements.Add(manualMovement);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetManualMovementsByMonthAndYearAsync_Should_Return_MovementsForPeriod()
        {
            var movement = Context.ManualMovements.First();
            var result = await Repository.GetByMonthAndYearAsync(movement.Month, movement.Year);

            Assert.NotNull(result);
            Assert.All(result, m => 
            {
                Assert.Equal(movement.Month, m.Month);
                Assert.Equal(movement.Year, m.Year);
            });
        }

        [Fact]
        public async Task GetManualMovementsByMonthAndYearAsync_Should_Return_EmptyWhen_NoMovementsForPeriod()
        {
            var result = await Repository.GetByMonthAndYearAsync(13, 2025);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetManualMovementsByProductAsync_Should_Return_MovementsForProduct()
        {
            var movement = Context.ManualMovements.First();
            var result = await Repository.GetByProductAsync(movement.ProductCode);

            Assert.NotNull(result);
            Assert.All(result, m => Assert.Equal(movement.ProductCode, m.ProductCode));
        }

        [Fact]
        public async Task GetManualMovementsByProductAsync_Should_Return_EmptyWhen_NoMovementsForProduct()
        {
            var result = await Repository.GetByProductAsync("NONEXISTENT");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetNextLaunchNumberAsync_Should_Return_NextNumber()
        {
            var movement = Context.ManualMovements.First();
            var result = await Repository.GetNextLaunchNumberAsync(movement.Month, movement.Year);

            Assert.Equal(movement.LaunchNumber + 1, result);
        }

        [Fact]
        public async Task GetNextLaunchNumberAsync_Should_Return_OneWhen_NoMovements()
        {
            var result = await Repository.GetNextLaunchNumberAsync(13, 2025);

            Assert.Equal(1, result);
        }
    }
} 