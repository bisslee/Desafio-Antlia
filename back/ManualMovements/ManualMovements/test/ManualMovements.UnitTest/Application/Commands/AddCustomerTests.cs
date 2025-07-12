using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovements.Application.Commands;
using ManualMovements.Application.Commands.Customers.AddCustomer;
using ManualMovements.Application.Helpers;
using ManualMovements.Domain.Entities;
using ManualMovements.Domain.Exceptions;
using ManualMovements.Domain.Repositories;
using ManualMovements.Domain.Resources;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovements.UnitTest.Application.Commands
{
    public class AddCustomerTests: BaseTest
    {
        private readonly Mock<ILogger<AddCustomerHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<Customer>> WriteRepositoryMock;
        private readonly Mock<ICustomerReadRepository> CustomerReadRepositoryMock;
        private readonly Mock<IValidator<AddCustomerRequest>> ValidatorMock;
        private readonly Mock<IMapper> MapperMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly AddCustomerHandler Handler;

        public AddCustomerTests()
        {
            LoggerMock = new Mock<ILogger<AddCustomerHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<Customer>>();
            CustomerReadRepositoryMock = new Mock<ICustomerReadRepository>();
            ValidatorMock = new Mock<IValidator<AddCustomerRequest>>();
            MapperMock = new Mock<IMapper>();
            ResponseBuilder = new ResponseBuilder();
            Handler = new AddCustomerHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                CustomerReadRepositoryMock.Object,
                ValidatorMock.Object,
                MapperMock.Object,
                ResponseBuilder
            );
        }

        private AddCustomerRequest CreateValidRequest()
        {
            return new AddCustomerRequest
            {
                FullName = "Ivana Santos",
                Email = "ivana@biss.com.br",
                DocumentNumber = "12345678909",
                Gender = "Female",
                BirthDate = new DateTime(1990, 1, 1),
                Phone = "11999999999",
                FavoriteSport = "Futebol",
                FavoriteClub = "Corinthians",
                AcceptTermsUse = true,
                AcceptPrivacyPolicy = true,
                Address = new AddressRequest
                {
                    Street = "Rua Teste",
                    City = "São Paulo",
                    State = "SP",
                    Country = "Brasil",
                    Neighborhood = "Centro",
                    ZipCode = "01000-000",
                    Number = "100",
                    Complement = "Apto 10"
                }
            };
        }

        // Retorna Sucesso
        [Fact]
        public async Task Handle_Should_Add_Customer_Successfully()
        {
            // Arrange
            var request = CreateValidRequest();
            var customer = new Customer 
            { 
                Email = request.Email,
                DocumentNumber = request.DocumentNumber
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mocks to return null for uniqueness checks
            CustomerReadRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Customer?)null);
            CustomerReadRepositoryMock.Setup(r => r.GetByDocumentAsync(request.DocumentNumber))
                .ReturnsAsync((Customer?)null);

            MapperMock.Setup(m => m.Map<Customer>(It.IsAny<AddCustomerRequest>()))
                .Returns(customer);
            
            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<Customer>()))
                .ReturnsAsync(true);

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(201);
            response.Data.Should().NotBeNull();
        }

        // Retorna BadRequest 

        [Fact]
        public async Task Handle_Should_Return_BadRequestWhen_Validation_Fails()
        {
            // Arrange
            var request = CreateValidRequest();
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("FullName",
                ResourceApp.REQUIRED_NAME) };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var response = await Handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(400);
            response.Data.Should().BeNull();
            response.Message.Should().Contain(ResourceApp.REQUIRED_NAME);
        }

        [Fact]
        public async Task Handle_Should_Throw_CustomerEmailAlreadyExistsException_When_Email_Already_Exists()
        {
            // Arrange
            var request = CreateValidRequest();
            var existingCustomer = new Customer { Email = request.Email, Id = Guid.NewGuid() };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mock to return existing customer for email check
            CustomerReadRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(existingCustomer);
            CustomerReadRepositoryMock.Setup(r => r.GetByDocumentAsync(request.DocumentNumber))
                .ReturnsAsync((Customer?)null);

            MapperMock.Setup(m => m.Map<Customer>(It.IsAny<AddCustomerRequest>()))
                .Returns(new Customer { Email = request.Email });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerEmailAlreadyExistsException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("already exists");
            exception.ErrorCode.Should().Be("CUSTOMER_EMAIL_ALREADY_EXISTS");
            exception.StatusCode.Should().Be(409);
        }

        // Retorna BadRequest com CPF duplicado
        [Fact]
        public async Task Handle_Should_Throw_CustomerDocumentAlreadyExistsException_When_Document_Already_Exists()
        {
            // Arrange
            var request = CreateValidRequest();
            var existingCustomer = new Customer { DocumentNumber = request.DocumentNumber, Id = Guid.NewGuid() };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mocks to return existing customer for document check
            CustomerReadRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Customer?)null);
            CustomerReadRepositoryMock.Setup(r => r.GetByDocumentAsync(request.DocumentNumber))
                .ReturnsAsync(existingCustomer);

            MapperMock.Setup(m => m.Map<Customer>(It.IsAny<AddCustomerRequest>()))
                .Returns(new Customer { DocumentNumber = request.DocumentNumber });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerDocumentAlreadyExistsException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("already exists");
            exception.ErrorCode.Should().Be("CUSTOMER_DOCUMENT_ALREADY_EXISTS");
            exception.StatusCode.Should().Be(409);
        }

        // Retorna Excecao com erro interno
        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            // Arrange
            var request = CreateValidRequest();
            var customer = new Customer 
            { 
                Email = request.Email,
                DocumentNumber = request.DocumentNumber
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mocks to return null for uniqueness checks
            CustomerReadRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Customer?)null);
            CustomerReadRepositoryMock.Setup(r => r.GetByDocumentAsync(request.DocumentNumber))
                .ReturnsAsync((Customer?)null);

           MapperMock.Setup(m => m.Map<Customer>(request))
                .Returns(customer);

            WriteRepositoryMock.Setup(r => r.Add(It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
}