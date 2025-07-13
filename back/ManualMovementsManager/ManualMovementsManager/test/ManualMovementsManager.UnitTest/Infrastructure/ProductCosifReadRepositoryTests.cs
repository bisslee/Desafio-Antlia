using Bogus;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class ProductCosifReadRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly ProductCosifReadRepository Repository;
        private readonly Faker Faker;

        public ProductCosifReadRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);

            Repository = new ProductCosifReadRepository(Context);
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

            Context.Products.Add(product);
            Context.ProductCosifs.Add(productCosif);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetProductCosifByCodeAsync_Should_Return_ProductCosif()
        {
            var productCosif = Context.ProductCosifs.First();
            var product = Context.Products.First();

            var result = await Repository.GetByProductCodeAndCosifCodeAsync(product.ProductCode, productCosif.CosifCode);

            Assert.NotNull(result);
            Assert.Equal(productCosif.Id, result.Id);
            Assert.Equal(productCosif.CosifCode, result.CosifCode);
            Assert.Equal(productCosif.ClassificationCode, result.ClassificationCode);
        }

        [Fact]
        public async Task GetProductCosifByCodeAsync_Should_Return_NullWhen_NotFound()
        {
            var result = await Repository.GetByProductCodeAndCosifCodeAsync("NONEXISTENT", "NONEXISTENT");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductCosifsByProductCodeAsync_Should_Return_ProductCosifsForProduct()
        {
            var product = Context.Products.First();
            var result = await Repository.GetByProductCodeAsync(product.ProductCode);

            Assert.NotNull(result);
            Assert.All(result, productCosif => Assert.Equal(product.ProductCode, productCosif.ProductCode));
        }

        [Fact]
        public async Task GetProductCosifsByProductCodeAsync_Should_Return_EmptyWhen_NoProductCosifsForProduct()
        {
            var result = await Repository.GetByProductCodeAsync("NONEXISTENT");

            Assert.Empty(result);
        }
    }
} 