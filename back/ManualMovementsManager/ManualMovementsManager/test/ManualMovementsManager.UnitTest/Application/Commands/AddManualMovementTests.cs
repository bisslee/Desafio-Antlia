using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Commands.ManualMovements.AddManualMovement;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.DTOs;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class AddManualMovementTests : BaseTest
    {
        private readonly Mock<ILogger<AddManualMovementHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<ManualMovement>> WriteRepositoryMock;
        private readonly Mock<IProductReadRepository> ProductReadRepositoryMock;
        private readonly Mock<IProductCosifReadRepository> ProductCosifReadRepositoryMock;
        private readonly Mock<IManualMovementReadRepository> ManualMovementReadRepositoryMock;
        private readonly Mock<IValidator<AddManualMovementRequest>> ValidatorMock;
        private readonly Mock<IMapper> MapperMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly AddManualMovementHandler Handler;

        public AddManualMovementTests()
        {
            LoggerMock = new Mock<ILogger<AddManualMovementHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<ManualMovement>>();
            ProductReadRepositoryMock = new Mock<IProductReadRepository>();
            ProductCosifReadRepositoryMock = new Mock<IProductCosifReadRepository>();
            ManualMovementReadRepositoryMock = new Mock<IManualMovementReadRepository>();
            ValidatorMock = new Mock<IValidator<AddManualMovementRequest>>();
            MapperMock = new Mock<IMapper>();
            ResponseBuilder = new ResponseBuilder();
            Handler = new AddManualMovementHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                ProductReadRepositoryMock.Object,
                ProductCosifReadRepositoryMock.Object,
                ManualMovementReadRepositoryMock.Object,
                ValidatorMock.Object,
                MapperMock.Object,
                ResponseBuilder
            );
        }

        private AddManualMovementRequest CreateValidRequest()
        {
            return new AddManualMovementRequest
            {
                Month = 6,
                Year = 2024,
                ProductCode = "0001",
                CosifCode = "00000000001",
                Description = "Movimento manual teste",
                MovementDate = DateTime.Now.AddDays(-1),
                UserCode = "USER001",
                Value = 1000.50m
            };
        }

        [Fact]
        public async Task Handle_Should_Add_ManualMovement_Successfully()
        {
            // Arrange
            var request = CreateValidRequest();
            var manualMovement = new ManualMovement 
            { 
                Month = request.Month,
                Year = request.Year,
                ProductCode = request.ProductCode,
                CosifCode = request.CosifCode,
                Description = request.Description,
                MovementDate = request.MovementDate,
                UserCode = request.UserCode,
                Value = request.Value,
                LaunchNumber = 1
            };
            var manualMovementDto = new ManualMovementDto
            {
                Month = request.Month,
                Year = request.Year,
                ProductCode = request.ProductCode,
                CosifCode = request.CosifCode,
                Description = request.Description,
                MovementDate = request.MovementDate,
                UserCode = request.UserCode,
                Value = request.Value,
                LaunchNumber = 1
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mocks for existence checks
            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync(new ProductCosif { ProductCode = request.ProductCode, CosifCode = request.CosifCode });

            ManualMovementReadRepositoryMock.Setup(r => r.GetNextLaunchNumberAsync(request.Month, request.Year))
                .ReturnsAsync(1);

            MapperMock.Setup(m => m.Map<ManualMovement>(It.IsAny<AddManualMovementRequest>()))
                .Returns(manualMovement);
            MapperMock.Setup(m => m.Map<ManualMovementDto>(It.IsAny<ManualMovement>()))
                .Returns(manualMovementDto);
            
            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<ManualMovement>()))
                .ReturnsAsync(true);

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(201);
            response.Data.Should().NotBeNull();
            response.Data.Month.Should().Be(request.Month);
            response.Data.Year.Should().Be(request.Year);
            response.Data.ProductCode.Should().Be(request.ProductCode);
            response.Data.CosifCode.Should().Be(request.CosifCode);
            response.Data.Description.Should().Be(request.Description);
            response.Data.Value.Should().Be(request.Value);
            response.Data.LaunchNumber.Should().Be(1);
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

            MapperMock.Setup(m => m.Map<ManualMovement>(It.IsAny<AddManualMovementRequest>()))
                .Returns(new ManualMovement { ProductCode = request.ProductCode });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("was not found");
            exception.ErrorCode.Should().Be("PRODUCT_NOT_FOUND");
            exception.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Handle_Should_Throw_ProductCosifNotFoundException_When_ProductCosif_Does_Not_Exist()
        {
            // Arrange
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mocks
            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync((ProductCosif?)null);

            MapperMock.Setup(m => m.Map<ManualMovement>(It.IsAny<AddManualMovementRequest>()))
                .Returns(new ManualMovement { ProductCode = request.ProductCode, CosifCode = request.CosifCode });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductCosifNotFoundException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("was not found");
            exception.ErrorCode.Should().Be("PRODUCT_COSIF_NOT_FOUND");
            exception.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Handle_Should_Return_ErrorResponse_When_Repository_Add_Fails()
        {
            // Arrange
            var request = CreateValidRequest();
            var manualMovement = new ManualMovement 
            { 
                Month = request.Month,
                Year = request.Year,
                ProductCode = request.ProductCode,
                CosifCode = request.CosifCode,
                Description = request.Description,
                MovementDate = request.MovementDate,
                UserCode = request.UserCode,
                Value = request.Value
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync(new ProductCosif { ProductCode = request.ProductCode, CosifCode = request.CosifCode });

            ManualMovementReadRepositoryMock.Setup(r => r.GetNextLaunchNumberAsync(request.Month, request.Year))
                .ReturnsAsync(1);

            MapperMock.Setup(m => m.Map<ManualMovement>(It.IsAny<AddManualMovementRequest>()))
                .Returns(manualMovement);

            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<ManualMovement>()))
                .ReturnsAsync(false);

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
            response.Message.Should().Be("Failed to create manual movement");
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            // Arrange
            var request = CreateValidRequest();
            var manualMovement = new ManualMovement 
            { 
                Month = request.Month,
                Year = request.Year,
                ProductCode = request.ProductCode,
                CosifCode = request.CosifCode,
                Description = request.Description,
                MovementDate = request.MovementDate,
                UserCode = request.UserCode,
                Value = request.Value
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ProductReadRepositoryMock.Setup(r => r.GetByProductCodeAsync(request.ProductCode))
                .ReturnsAsync(new Product { ProductCode = request.ProductCode });
            ProductCosifReadRepositoryMock.Setup(r => r.GetByProductCodeAndCosifCodeAsync(request.ProductCode, request.CosifCode))
                .ReturnsAsync(new ProductCosif { ProductCode = request.ProductCode, CosifCode = request.CosifCode });

            ManualMovementReadRepositoryMock.Setup(r => r.GetNextLaunchNumberAsync(request.Month, request.Year))
                .ReturnsAsync(1);

            MapperMock.Setup(m => m.Map<ManualMovement>(It.IsAny<AddManualMovementRequest>()))
                .Returns(manualMovement);

            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<ManualMovement>()))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
} 