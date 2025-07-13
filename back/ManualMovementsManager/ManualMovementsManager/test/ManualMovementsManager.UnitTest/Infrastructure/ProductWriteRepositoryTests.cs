using Bogus;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class ProductWriteRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly WriteRepository<Product> Repository;
        private readonly Faker Faker;

        public ProductWriteRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite("Filename=:memory:")
               .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);
            Context.Database.OpenConnection();
            Context.Database.EnsureCreated();
            Repository = new WriteRepository<Product>(Context);
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

        [Fact]
        public async Task AddProduct_Should_Add_ProductSuccessfully()
        {
            var product = CreateProduct();

            var result = await Repository.Add(product);

            Assert.True(result);
            Assert.NotNull(await Context.Products.FindAsync(product.Id));
        }

        [Fact]
        public async Task Add_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Add(null));
        }

        [Fact]
        public async Task UpdateProduct_Should_Update_ProductSuccessfully()
        {
            var product = CreateProduct();
            await Repository.Add(product);

            var newDescription = Faker.Commerce.ProductName();
            var newStatus = DataStatus.Inactive;

            product.Description = newDescription;
            product.Status = newStatus;

            var result = await Repository.Update(product);

            Assert.True(result);
            var updatedProduct = await Context.Products.FindAsync(product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(newDescription, updatedProduct.Description);
            Assert.Equal(newStatus, updatedProduct.Status);
        }

        [Fact]
        public async Task Update_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Update(null));
        }

        [Fact]
        public async Task RemoveProduct_Should_Remove_ProductSuccessfully()
        {
            var product = CreateProduct();
            await Repository.Add(product);

            var result = await Repository.Delete(product);

            Assert.True(result);
            Assert.Null(await Context.Products.FindAsync(product.Id));
        }

        [Fact]
        public async Task Remove_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Delete(null));
        }

        [Fact]
        public async Task ExecuteSql_ExecutesRawSqlCommand()
        {
            var sql = "SELECT @p0 as id, @p1 as code, @p2 as description";
            var productId = Guid.NewGuid();
            var code = "TEST001";
            var description = "Test Product";

            var parameters = new object[] { productId, code, description };

            var result = await Repository.ExecuteSql(sql, parameters);

            Assert.True(result);
        }
    }
} 