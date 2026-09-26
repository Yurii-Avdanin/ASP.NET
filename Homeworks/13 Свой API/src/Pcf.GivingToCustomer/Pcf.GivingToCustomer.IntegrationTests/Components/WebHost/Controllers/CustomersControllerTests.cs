using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.WebHost.Controllers;
using Pcf.GivingToCustomer.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Pcf.GivingToCustomer.IntegrationTests.Components.WebHost.Controllers
{
    [Collection(EfDatabaseCollection.DbCollection)]
    public class CustomersControllerTests : IClassFixture<EfDatabaseFixture>
    {
        private readonly CustomersController _customersController;
        private readonly EfRepository<Customer> _customerRepository;
        //private readonly Mock<IPreferencesGateway> _preferencesGatewayMock;
        private readonly Mock<IPreferencesGatewayGrpc> _preferencesGatewayGrpcMock;

        public CustomersControllerTests(EfDatabaseFixture efDatabaseFixture)
        {
            _customerRepository = new EfRepository<Customer>(efDatabaseFixture.DbContext);
            //_preferencesGatewayMock = new Mock<IPreferencesGateway>();
            _preferencesGatewayGrpcMock = new Mock<IPreferencesGatewayGrpc>();

            _customersController = new CustomersController(
                _customerRepository,
                _preferencesGatewayGrpcMock.Object);
        }

        [Fact]
        public async Task CreateCustomerAsync_CanCreateCustomer_ShouldCreateExpectedCustomer()
        {
            //Arrange 
            var preference = new Preference()
            {
                Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                Name = "Театр"
            };

            _preferencesGatewayGrpcMock
                .Setup(x => x.GetPreferencesByIdsAsync(It.Is<List<Guid>>(ids => ids.Contains(preference.Id))))
                .ReturnsAsync(new List<Preference> { preference });

            var request = new CreateOrEditCustomerRequest()
            {
                Email = "some@mail.ru",
                FirstName = "Иван",
                LastName = "Петров",
                PreferenceIds = new List<Guid>()
                {
                    preference.Id
                }
            };

            //Act
            var result = await _customersController.CreateCustomerAsync(request);
            var actionResult = result.Result as CreatedAtActionResult;
            var id = (Guid)actionResult.Value;

            //Assert
            var actual = await _customerRepository.GetByIdAsync(id);

            actual.Email.Should().Be(request.Email);
            actual.FirstName.Should().Be(request.FirstName);
            actual.LastName.Should().Be(request.LastName);
            actual.Preferences.Should()
                .ContainSingle()
                .And
                .Contain(x => x.PreferenceId == preference.Id);
        }
    }
}