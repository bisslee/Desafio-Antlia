using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Commands;
using ManualMovementsManager.Application.Commands.Customers.ChangeCustomer;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Resources;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class ChangeCustomerTests : BaseTest
    {
        private readonly Mock<ILogger<ChangeCustomerHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<Customer>> WriteRepositoryMock;
        private readonly Mock<ICustomerReadRepository> CustomerReadRepositoryMock;
        private readonly Mock<IValidator<ChangeCustomerRequest>> ValidatorMock;
        private readonly Mock<IMapper> MapperMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly ChangeCustomerHandler Handler;

        public ChangeCustomerTests()
        {
            LoggerMock = new Mock<ILogger<ChangeCustomerHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<Customer>>();
            CustomerReadRepositoryMock = new Mock<ICustomerReadRepository>();
            ValidatorMock = new Mock<IValidator<ChangeCustomerRequest>>();
            MapperMock = new Mock<IMapper>();
            ResponseBuilder = new ResponseBuilder();
            Handler = new ChangeCustomerHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                CustomerReadRepositoryMock.Object,
                ValidatorMock.Object,
                MapperMock.Object,
                ResponseBuilder
            );
        }

        private ChangeCustomerRequest CreateValidRequest()
        {
            return new ChangeCustomerRequest
            {
                Id = Guid.NewGuid(),
                FullName = "Ivana Batista",
                Email = "ivana@biss.com.br",
                DocumentNumber = "12345678900",
                Gender = "Female",
                BirthDate = new DateTime(1990, 1, 1),
                Phone = "11999999999",
                FavoriteSport = "Vôlei",
                FavoriteClub = "Corinthians",
                AcceptTermsUse = true,
                AcceptPrivacyPolicy = true,
                Address = new AddressRequest
                {
                    Street = "Rua Exemplo",
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

        [Fact]
        public async Task Handle_Should_Update_Customer_Successfully()
        {
            var request = CreateValidRequest();
            var customer = new Customer { Id = request.Id };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(customer);
            CustomerReadRepositoryMock.Setup(r => r.GetCustomerWithAddressByIdAsync(request.Id))
                .ReturnsAsync(customer);
            // Setup mocks to return null for uniqueness checks (excluding current customer)
            CustomerReadRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Customer?)null);
            CustomerReadRepositoryMock.Setup(r => r.GetByDocumentAsync(request.DocumentNumber))
                .ReturnsAsync((Customer?)null);

            WriteRepositoryMock.Setup(r => r.Update(It.IsAny<Customer>()))
                .ReturnsAsync(true);

            // Ajuste: garantir que o customer atualizado tenha os campos do request
            MapperMock.Setup(m => m.Map(It.IsAny<ChangeCustomerRequest>(), It.IsAny<Customer>()))
                .Callback<ChangeCustomerRequest, Customer>((req, cust) => {
                    cust.Email = req.Email;
                    cust.DocumentNumber = req.DocumentNumber;
                });

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_BadRequestWhen_Validation_Fails()
        {
            var request = CreateValidRequest();
            var validationFailures = new List<ValidationFailure> {
               new ValidationFailure("FullName", ResourceApp.REQUIRED_NAME) };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(400);
            response.Data.Should().BeNull();
            response.Message.Should().Contain(ResourceApp.REQUIRED_NAME);
        }

        [Fact]
        public async Task Handle_Should_Throw_CustomerNotFoundException_When_Customer_Not_Found()
        {
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync((Customer?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerNotFoundException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("was not found");
            exception.ErrorCode.Should().Be("CUSTOMER_NOT_FOUND");
            exception.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Handle_Should_Throw_CustomerEmailAlreadyExistsException_When_Email_Duplicated()
        {
            var request = CreateValidRequest();
            var customer = new Customer { Id = request.Id };
            var existingCustomer = new Customer { Email = request.Email, Id = Guid.NewGuid() };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(customer);
            CustomerReadRepositoryMock.Setup(r => r.GetCustomerWithAddressByIdAsync(request.Id))
                .ReturnsAsync(customer);
            
            // Setup mock to return existing customer for email check
            CustomerReadRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(existingCustomer);
            
            // Setup mock to return null for document check
            CustomerReadRepositoryMock.Setup(r => r.GetByDocumentAsync(request.DocumentNumber))
                .ReturnsAsync((Customer?)null);

            // Setup mapper to properly set email and document number
            MapperMock.Setup(m => m.Map(It.IsAny<ChangeCustomerRequest>(), It.IsAny<Customer>()))
                .Callback<ChangeCustomerRequest, Customer>((req, cust) => {
                    cust.Email = req.Email;
                    cust.DocumentNumber = req.DocumentNumber;
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerEmailAlreadyExistsException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("already exists");
            exception.ErrorCode.Should().Be("CUSTOMER_EMAIL_ALREADY_EXISTS");
            exception.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task Handle_Should_Throw_CustomerDocumentAlreadyExistsException_When_Document_Duplicated()
        {
            var request = CreateValidRequest();
            var customer = new Customer { Id = request.Id };
            var existingCustomer = new Customer { DocumentNumber = request.DocumentNumber, Id = Guid.NewGuid() };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(customer);
            CustomerReadRepositoryMock.Setup(r => r.GetCustomerWithAddressByIdAsync(request.Id))
                .ReturnsAsync(customer);
            
            // Setup mock to return null for email check
            CustomerReadRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Customer?)null);
            
            // Setup mock to return existing customer for document check
            CustomerReadRepositoryMock.Setup(r => r.GetByDocumentAsync(request.DocumentNumber))
                .ReturnsAsync(existingCustomer);

            // Setup mapper to properly set email and document number
            MapperMock.Setup(m => m.Map(It.IsAny<ChangeCustomerRequest>(), It.IsAny<Customer>()))
                .Callback<ChangeCustomerRequest, Customer>((req, cust) => {
                    cust.Email = req.Email;
                    cust.DocumentNumber = req.DocumentNumber;
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerDocumentAlreadyExistsException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("already exists");
            exception.ErrorCode.Should().Be("CUSTOMER_DOCUMENT_ALREADY_EXISTS");
            exception.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
}
