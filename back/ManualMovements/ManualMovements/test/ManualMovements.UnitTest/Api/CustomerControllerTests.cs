using FluentAssertions;
using MediatR;
using ManualMovements.Api.Controllers;
using ManualMovements.Application.Commands.Customers.AddCustomer;
using ManualMovements.Application.Commands.Customers.ChangeCustomer;
using ManualMovements.Application.Commands.Customers.RemoveCustomer;
using ManualMovements.Application.Queries.Customers.GetCustomer;
using ManualMovements.Application.Queries.Customers.GetCustomerByKey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovements.UnitTest.Api.Controllers
{
    public class CustomerControllerTests : BaseTest
    {
        private readonly Mock<IMediator> MediatorMock;
        private readonly Mock<ILogger<CustomerController>> LoggerMock;
        private readonly CustomerController Controller;

        public CustomerControllerTests()
        {
            MediatorMock = new Mock<IMediator>();
            LoggerMock = new Mock<ILogger<CustomerController>>();
            Controller = new CustomerController(
                MediatorMock.Object,
                LoggerMock.Object);
        }

        [Fact]
        public async Task Get_Should_Return_206_When_Customers_Found()
        {
            var request = new GetCustomerRequest();
            var response = new GetCustomerResponse
            {
                Success = true,
                StatusCode = 206,
                Data = new List<ManualMovements.Domain.Entities.Customer> { new() }
            };

            MediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.Get(request);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(206);
        }

        [Fact]
        public async Task GetByKey_Should_Return_200_When_Customer_Found()
        {
            var id = Guid.NewGuid();
            var response = new GetCustomerByKeyResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovements.Domain.Entities.Customer { Id = id }
            };

            MediatorMock.Setup(m => m.Send(It.Is<GetCustomerByKeyRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.GetByKey(id);

            result.Should().BeOfType<OkObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Add_Should_Return_201_When_Customer_Created()
        {
            var request = new AddCustomerRequest();
            var response = new AddCustomerResponse
            {
                Success = true,
                StatusCode = 201,
                Data = new ManualMovements.Domain.Entities.Customer()
            };

            MediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await Controller.Add(request);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Change_Should_Return_200_When_Customer_Updated()
        {
            var id = Guid.NewGuid();
            var request = new ChangeCustomerRequest();
            var response = new ChangeCustomerResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovements.Domain.Entities.Customer()
            };

            MediatorMock.Setup(m => m.Send(It.Is<ChangeCustomerRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.Change(id, request);

            result.Should().BeOfType<OkObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Remove_Should_Return_200_When_Customer_Removed()
        {
            var id = Guid.NewGuid();
            var response = new RemoveCustomerResponse
            {
                Success = true,
                StatusCode = 200,
                Data = true
            };

            MediatorMock.Setup(m => m.Send(It.Is<RemoveCustomerRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);


            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.Remove(id);

            result.Should().BeOfType<OkObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(200);
        }
    }
}
