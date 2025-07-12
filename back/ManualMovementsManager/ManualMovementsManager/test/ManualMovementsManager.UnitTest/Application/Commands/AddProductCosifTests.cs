using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Commands.ProductCosifs.AddProductCosif;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class AddProductCosifTests : BaseTest
    {
        private readonly Mock<ILogger<AddProductCosifHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<ProductCosif>> WriteRepositoryMock;
        private readonly Mock<IProductReadRepository> ProductReadRepositoryMock;
        private readonly Mock<IProductCosifReadRepository> ProductCosifReadRepositoryMock;
        private readonly Mock<IValidator<AddProductCosifRequest>> ValidatorMock;
        private readonly Mock<IMapper> MapperMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly AddProductCosifHandler Handler;

        public AddProductCosifTests()
        {
            LoggerMock = new Mock<ILogger<AddProductCosifHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<ProductCosif>>();
            ProductReadRepositoryMock = new Mock<IProductReadRepository>();
            ProductCosifReadRepositoryMock = new Mock<IProductCosifReadRepository>();
            ValidatorMock = new Mock<IValidator<AddProductCosifRequest>>();
            MapperMock = new Mock<IMapper>();
            ResponseBuilder = new ResponseBuilder();
            Handler = new AddProductCosifHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                ProductReadRepositoryMock.Object,
                ProductCosifReadRepositoryMock.Object,
                ValidatorMock.Object,
                MapperMock.Object,
                ResponseBuilder
            );
        }

        private AddProductCosifRequest CreateValidRequest()
        {
            return new AddProductCosifRequest
            {
                ProductCode = "0001",
                CosifCode = "00000000001",
                ClassificationCode = "000001"
            };
        }

        [Fact]
        public async Task Handle_Should_Add_ProductCosif_Successfully()
        {
            // Arrange
            var request = CreateValidRequest();
            var productCosif = new ProductCosif 
            { 
                ProductCode = request.ProductCode,
                CosifCode = request.CosifCode,
                ClassificationCode = request.ClassificationCode
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mocks to return null for uniqueness checks
            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync((ProductCosif?)null);

            MapperMock.Setup(m => m.Map<ProductCosif>(It.IsAny<AddProductCosifRequest>()))
                .Returns(productCosif);
            
            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<ProductCosif>()))
                .ReturnsAsync(true);

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(201);
            response.Data.Should().NotBeNull();
            response.Data.ProductCode.Should().Be(request.ProductCode);
            response.Data.CosifCode.Should().Be(request.CosifCode);
            response.Data.ClassificationCode.Should().Be(request.ClassificationCode);
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
        public async Task Handle_Should_Throw_ProductNotFoundException_When_Product_Does_Not_Exist()
        {
            // Arrange
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mock to return null for product existence check
            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync((Product?)null);

            MapperMock.Setup(m => m.Map<ProductCosif>(It.IsAny<AddProductCosifRequest>()))
                .Returns(new ProductCosif { ProductCode = request.ProductCode });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("was not found");
            exception.ErrorCode.Should().Be("PRODUCT_NOT_FOUND");
            exception.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Handle_Should_Throw_ProductCosifCodeAlreadyExistsException_When_ProductCosif_Already_Exists()
        {
            // Arrange
            var request = CreateValidRequest();
            var existingProductCosif = new ProductCosif { ProductCode = request.ProductCode, CosifCode = request.CosifCode, Id = Guid.NewGuid() };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mocks
            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync(existingProductCosif);

            MapperMock.Setup(m => m.Map<ProductCosif>(It.IsAny<AddProductCosifRequest>()))
                .Returns(new ProductCosif { ProductCode = request.ProductCode, CosifCode = request.CosifCode });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductCosifCodeAlreadyExistsException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("already exists");
            exception.ErrorCode.Should().Be("PRODUCT_COSIF_CODE_ALREADY_EXISTS");
            exception.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task Handle_Should_Return_ErrorResponse_When_Repository_Add_Fails()
        {
            // Arrange
            var request = CreateValidRequest();
            var productCosif = new ProductCosif 
            { 
                ProductCode = request.ProductCode,
                CosifCode = request.CosifCode,
                ClassificationCode = request.ClassificationCode
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync((ProductCosif?)null);

            MapperMock.Setup(m => m.Map<ProductCosif>(It.IsAny<AddProductCosifRequest>()))
                .Returns(productCosif);

            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<ProductCosif>()))
                .ReturnsAsync(false);

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
            response.Message.Should().Be("Failed to create product COSIF");
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            // Arrange
            var request = CreateValidRequest();
            var productCosif = new ProductCosif 
            { 
                ProductCode = request.ProductCode,
                CosifCode = request.CosifCode,
                ClassificationCode = request.ClassificationCode
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync((ProductCosif?)null);

            MapperMock.Setup(m => m.Map<ProductCosif>(It.IsAny<AddProductCosifRequest>()))
                .Returns(productCosif);

            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<ProductCosif>()))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
} 