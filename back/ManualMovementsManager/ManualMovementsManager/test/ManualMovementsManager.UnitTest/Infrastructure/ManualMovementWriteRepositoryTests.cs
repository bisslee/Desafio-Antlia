using Bogus;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.UnitTest.Infrastructure
{
    public class ManualMovementWriteRepositoryTests : BaseTest
    {
        private readonly AppDbContext Context;
        private readonly WriteRepository<ManualMovement> Repository;
        private readonly Faker Faker;

        public ManualMovementWriteRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite("Filename=:memory:")
               .Options;

            Faker = new Faker("pt_BR");
            Context = new AppDbContext(options);
            Context.Database.OpenConnection();
            Context.Database.EnsureCreated();
            Repository = new WriteRepository<ManualMovement>(Context);
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

        private ManualMovement CreateManualMovement(string productCode, string cosifCode)
        {
            return new ManualMovement
            {
                Id = Guid.NewGuid(),
                Month = Faker.Random.Int(1, 12),
                Year = Faker.Random.Int(2020, 2024),
                LaunchNumber = 1,
                ProductCode = productCode,
                CosifCode = cosifCode,
                Description = Faker.Lorem.Sentence(),
                MovementDate = DateTime.Now,
                UserCode = "TESTUSER",
                Value = Faker.Random.Decimal(100, 10000),
                CreatedAt = DateTime.Now,
                CreatedBy = "testes",
                UpdatedAt = DateTime.Now,
                UpdatedBy = "testes"
            };
        }

        [Fact]
        public async Task AddManualMovement_Should_Add_ManualMovementSuccessfully()
        {
            var product = CreateProduct();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var productCosif = CreateProductCosif(product.ProductCode);
            Context.ProductCosifs.Add(productCosif);
            await Context.SaveChangesAsync();

            var manualMovement = CreateManualMovement(product.ProductCode, productCosif.CosifCode);

            var result = await Repository.Add(manualMovement);

            Assert.True(result);
            Assert.NotNull(await Context.ManualMovements.FindAsync(manualMovement.Id));
        }

        [Fact]
        public async Task Add_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Add(null));
        }

        [Fact]
        public async Task UpdateManualMovement_Should_Update_ManualMovementSuccessfully()
        {
            var product = CreateProduct();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var productCosif = CreateProductCosif(product.ProductCode);
            Context.ProductCosifs.Add(productCosif);
            await Context.SaveChangesAsync();

            var manualMovement = CreateManualMovement(product.ProductCode, productCosif.CosifCode);
            await Repository.Add(manualMovement);

            var newValue = Faker.Random.Decimal(5000, 20000);
            var newDescription = Faker.Lorem.Sentence();

            manualMovement.Value = newValue;
            manualMovement.Description = newDescription;

            var result = await Repository.Update(manualMovement);

            Assert.True(result);
            var updatedManualMovement = await Context.ManualMovements.FindAsync(manualMovement.Id);
            Assert.NotNull(updatedManualMovement);
            Assert.Equal(newValue, updatedManualMovement.Value);
            Assert.Equal(newDescription, updatedManualMovement.Description);
        }

        [Fact]
        public async Task Update_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Update(null));
        }

        [Fact]
        public async Task RemoveManualMovement_Should_Remove_ManualMovementSuccessfully()
        {
            var product = CreateProduct();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var productCosif = CreateProductCosif(product.ProductCode);
            Context.ProductCosifs.Add(productCosif);
            await Context.SaveChangesAsync();

            var manualMovement = CreateManualMovement(product.ProductCode, productCosif.CosifCode);
            await Repository.Add(manualMovement);

            var result = await Repository.Delete(manualMovement);

            Assert.True(result);
            Assert.Null(await Context.ManualMovements.FindAsync(manualMovement.Id));
        }

        [Fact]
        public async Task Remove_ThrowsArgumentNullExceptionWhenEntityIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.Delete(null));
        }

        [Fact]
        public async Task ExecuteSql_ExecutesRawSqlCommand()
        {
            var sql = "SELECT @p0 as id, @p1 as month, @p2 as year, @p3 as value";
            var movementId = Guid.NewGuid();
            var month = 6;
            var year = 2024;
            var value = 1500.50m;

            var parameters = new object[] { movementId, month, year, value };

            var result = await Repository.ExecuteSql(sql, parameters);

            Assert.True(result);
        }
    }
} 