using Bogus;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class ProductReadRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly ProductReadRepository Repository;
        private readonly Faker Faker;

        public ProductReadRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);

            Repository = new ProductReadRepository(Context);
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

            Context.Products.Add(product);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetProductByCodeAsync_Should_Return_Product()
        {
            var product = Context.Products.First();

            var result = await Repository.GetByProductCodeAsync(product.ProductCode);

            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.ProductCode, result.ProductCode);
            Assert.Equal(product.Description, result.Description);
        }

        [Fact]
        public async Task GetProductByCodeAsync_Should_Return_NullWhen_NotFound()
        {
            var result = await Repository.GetByProductCodeAsync("NONEXISTENT");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllActiveAsync_Should_Return_ActiveProducts()
        {
            var result = await Repository.GetAllActiveAsync();

            Assert.NotNull(result);
            Assert.All(result, product => Assert.Equal(DataStatus.Active, product.Status));
        }
    }
} 