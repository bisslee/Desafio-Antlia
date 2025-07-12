using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Commands.Products.AddProduct;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class AddProductTests : BaseTest
    {
        private readonly Mock<ILogger<AddProductHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<Product>> WriteRepositoryMock;
        private readonly Mock<IProductReadRepository> ProductReadRepositoryMock;
        private readonly Mock<IValidator<AddProductRequest>> ValidatorMock;
        private readonly Mock<IMapper> MapperMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly AddProductHandler Handler;

        public AddProductTests()
        {
            LoggerMock = new Mock<ILogger<AddProductHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<Product>>();
            ProductReadRepositoryMock = new Mock<IProductReadRepository>();
            ValidatorMock = new Mock<IValidator<AddProductRequest>>();
            MapperMock = new Mock<IMapper>();
            ResponseBuilder = new ResponseBuilder();
            Handler = new AddProductHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                ProductReadRepositoryMock.Object,
                ValidatorMock.Object,
                MapperMock.Object,
                ResponseBuilder
            );
        }

        private AddProductRequest CreateValidRequest()
        {
            return new AddProductRequest
            {
                ProductCode = "0001",
                Description = "Produto Teste"
            };
        }

        [Fact]
        public async Task Handle_Should_Add_Product_Successfully()
        {
            // Arrange
            var request = CreateValidRequest();
            var product = new Product 
            { 
                ProductCode = request.ProductCode,
                Description = request.Description
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mock to return null for uniqueness check
            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync((Product?)null);

            MapperMock.Setup(m => m.Map<Product>(It.IsAny<AddProductRequest>()))
                .Returns(product);
            
            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                .ReturnsAsync(true);

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(201);
            response.Data.Should().NotBeNull();
            response.Data.ProductCode.Should().Be(request.ProductCode);
            response.Data.Description.Should().Be(request.Description);
        }

        [Fact]
        public async Task Handle_Should_Return_BadRequestWhen_Validation_Fails()
        {
            // Arrange
            var request = CreateValidRequest();
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("ProductCode", "Product code is required") };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(400);
            response.Data.Should().BeNull();
            response.Message.Should().Contain("Product code is required");
        }

        [Fact]
        public async Task Handle_Should_Throw_ProductCodeAlreadyExistsException_When_ProductCode_Already_Exists()
        {
            // Arrange
            var request = CreateValidRequest();
            var existingProduct = new Product { ProductCode = request.ProductCode, Id = Guid.NewGuid() };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mock to return existing product for uniqueness check
            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(existingProduct);

            MapperMock.Setup(m => m.Map<Product>(It.IsAny<AddProductRequest>()))
                .Returns(new Product { ProductCode = request.ProductCode });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductCodeAlreadyExistsException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("already exists");
            exception.ErrorCode.Should().Be("PRODUCT_CODE_ALREADY_EXISTS");
            exception.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task Handle_Should_Return_ErrorResponse_When_Repository_Add_Fails()
        {
            // Arrange
            var request = CreateValidRequest();
            var product = new Product 
            { 
                ProductCode = request.ProductCode,
                Description = request.Description
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync((Product?)null);

            MapperMock.Setup(m => m.Map<Product>(It.IsAny<AddProductRequest>()))
                .Returns(product);

            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                .ReturnsAsync(false);

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
            response.Message.Should().Be("Failed to create product");
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            // Arrange
            var request = CreateValidRequest();
            var product = new Product 
            { 
                ProductCode = request.ProductCode,
                Description = request.Description
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync((Product?)null);

            MapperMock.Setup(m => m.Map<Product>(It.IsAny<AddProductRequest>()))
                .Returns(product);

            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<Product>()))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
} 