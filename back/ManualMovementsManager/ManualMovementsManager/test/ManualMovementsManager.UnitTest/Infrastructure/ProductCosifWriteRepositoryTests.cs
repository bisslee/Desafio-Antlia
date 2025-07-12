using Bogus;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class ProductCosifWriteRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly WriteRepository<ProductCosif> Repository;
        private readonly Faker Faker;

        public ProductCosifWriteRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite("Filename=:memory:")
               .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);
            Context.Database.OpenConnection();
            Context.Database.EnsureCreated();
            Repository = new WriteRepository<ProductCosif>(Context);
        }

        private Product CreateProduct()
        {
            return new Product
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
        }

        private ProductCosif CreateProductCosif(string productCode)
        {
            return new ProductCosif
            {
                Id = Guid.NewGuid(),
                ProductCode = productCode,
                CosifCode = Faker.Random.AlphaNumeric(10),
                ClassificationCode = Faker.Random.AlphaNumeric(5),
                Status = DataStatus.Active,
                CreatedAt = DateTime.Now,
                CreatedBy = "testes",
                UpdatedAt = DateTime.Now,
                UpdatedBy = "testes"
            };
        }

        [Fact]
        public async Task AddProductCosif_Should_Add_ProductCosifSuccessfully()
        {
            var product = CreateProduct();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var productCosif = CreateProductCosif(product.ProductCode);

            var result = await Repository.Add(productCosif);

            Assert.True(result);
            Assert.NotNull(await Context.ProductCosifs.FindAsync(productCosif.Id));
        }

        [Fact]
        public async Task Add_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Add(null));
        }

        [Fact]
        public async Task UpdateProductCosif_Should_Update_ProductCosifSuccessfully()
        {
            var product = CreateProduct();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var productCosif = CreateProductCosif(product.ProductCode);
            await Repository.Add(productCosif);

            var newClassificationCode = Faker.Random.AlphaNumeric(5);
            var newStatus = DataStatus.Inactive;

            productCosif.ClassificationCode = newClassificationCode;
            productCosif.Status = newStatus;

            var result = await Repository.Update(productCosif);

            Assert.True(result);
            var updatedProductCosif = await Context.ProductCosifs.FindAsync(productCosif.Id);
            Assert.NotNull(updatedProductCosif);
            Assert.Equal(newClassificationCode, updatedProductCosif.ClassificationCode);
            Assert.Equal(newStatus, updatedProductCosif.Status);
        }

        [Fact]
        public async Task Update_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Update(null));
        }

        [Fact]
        public async Task RemoveProductCosif_Should_Remove_ProductCosifSuccessfully()
        {
            var product = CreateProduct();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var productCosif = CreateProductCosif(product.ProductCode);
            await Repository.Add(productCosif);

            var result = await Repository.Delete(productCosif);

            Assert.True(result);
            Assert.Null(await Context.ProductCosifs.FindAsync(productCosif.Id));
        }

        [Fact]
        public async Task Remove_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Delete(null));
        }

        [Fact]
        public async Task ExecuteSql_ExecutesRawSqlCommand()
        {
            var sql = "SELECT @p0 as id, @p1 as cosif_code, @p2 as description";
            var productCosifId = Guid.NewGuid();
            var cosifCode = "COSIF001";
            var description = "Test Product Cosif";

            var parameters = new object[] { productCosifId, cosifCode, description };

            var result = await Repository.ExecuteSql(sql, parameters);

            Assert.True(result);
        }
    }
} 