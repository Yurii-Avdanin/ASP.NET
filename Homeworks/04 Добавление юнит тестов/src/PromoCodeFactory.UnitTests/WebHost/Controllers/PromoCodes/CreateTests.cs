using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodesRepositoryMock;
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;

    private readonly PromoCodesController _promoCodesController;

    public CreateTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _customerPromoCodesRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();

        _promoCodesController = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            _customersRepositoryMock.Object,
            _customerPromoCodesRepositoryMock.Object,
            _partnersRepositoryMock.Object,
            _preferencesRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        #region Arrange
        var request = new AutoFaker<PromoCodeCreateRequest>().Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);
        #endregion

        #region Act        
        var resultPromoCode = await _promoCodesController.Create(request, CancellationToken.None);
        #endregion

        #region Assert
        resultPromoCode.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)resultPromoCode.Result;        
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value;
        problemDetails.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {request.PartnerId} not found.");
        #endregion
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        #region Arrange
        var request = new AutoFaker<PromoCodeCreateRequest>().Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, request.PartnerId)
            .Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);
        #endregion

        #region Act        
        var resultPromoCode = await _promoCodesController.Create(request, CancellationToken.None);
        #endregion

        #region Assert
        resultPromoCode.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)resultPromoCode.Result;        
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value;
        problemDetails.Title.Should().Be("Preference not found");
        problemDetails.Detail.Should().Be($"Preference with Id {request.PreferenceId} not found.");
        #endregion
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        #region Arrange
        var request = new AutoFaker<PromoCodeCreateRequest>().Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, request.PartnerId)
            .Generate();
        partner.PartnerLimits.Add(
            new AutoFaker<PartnerPromoCodeLimit>()
                .RuleFor(l => l.Partner, partner)
                .RuleFor(l => l.CanceledAt, DateTimeOffset.UtcNow.AddDays(-4))
                .RuleFor(l => l.CreatedAt, DateTimeOffset.UtcNow.AddDays(-5))
                .RuleFor(l => l.EndAt, DateTimeOffset.UtcNow.AddDays(1))
                .Generate());

        var preference = new AutoFaker<Preference>()
            .RuleFor(p => p.Id, request.PreferenceId)
            .Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(c => c.Preferences.Any(p => p.Id == request.PreferenceId), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>()); 
        #endregion

        #region Act        
        var resultPromoCode = await _promoCodesController.Create(request, CancellationToken.None);
        #endregion

        #region Assert
        resultPromoCode.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)resultPromoCode.Result;
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value;
        problemDetails.Title.Should().Be("No active limit");
        problemDetails.Detail.Should().Be("Partner has no active promo code limit.");
        #endregion
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        #region Arrange
        var request = new AutoFaker<PromoCodeCreateRequest>().Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, request.PartnerId)
            .Generate();

        var partnerPromoCodeLimit = new AutoFaker<PartnerPromoCodeLimit>()
                .RuleFor(l => l.Partner, partner)
                .RuleFor(l => l.CanceledAt, (DateTimeOffset?)null)
                .RuleFor(l => l.CreatedAt, DateTimeOffset.UtcNow.AddDays(-5))
                .RuleFor(l => l.EndAt, DateTimeOffset.UtcNow.AddDays(1))
                .RuleFor(l => l.Limit, 2)
                .RuleFor(l => l.IssuedCount, 2)
                .Generate();

        partner.PartnerLimits.Add(partnerPromoCodeLimit);

        var preference = new AutoFaker<Preference>()
           .RuleFor(p => p.Id, request.PreferenceId)
           .Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(c => c.Preferences.Any(p => p.Id == request.PreferenceId), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());
        #endregion

        #region Act        
        var resultPromoCode = await _promoCodesController.Create(request, CancellationToken.None);
        #endregion

        #region Assert
        resultPromoCode.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)resultPromoCode.Result;
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value;
        problemDetails.Title.Should().Be("Limit exceeded");
        problemDetails.Detail.Should().Be($"Cannot create promo code. Limit would be exceeded (current: {partnerPromoCodeLimit.IssuedCount}/{partnerPromoCodeLimit.Limit}).");
        #endregion
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        #region Arrange
        var request = new AutoFaker<PromoCodeCreateRequest>().Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, request.PartnerId)
            .Generate();

        var activeLimit = new AutoFaker<PartnerPromoCodeLimit>()
                .RuleFor(l => l.Partner, partner)
                .RuleFor(l => l.CanceledAt, (DateTimeOffset?)null)
                .RuleFor(l => l.CreatedAt, DateTimeOffset.UtcNow.AddDays(-5))
                .RuleFor(l => l.EndAt, DateTimeOffset.UtcNow.AddDays(1))
                .RuleFor(l => l.Limit, 2)
                .RuleFor(l => l.IssuedCount, 0)
                .Generate();

        partner.PartnerLimits.Add(activeLimit);

        var preference = new AutoFaker<Preference>()
           .RuleFor(p => p.Id, request.PreferenceId)
           .Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(request.PartnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(c => c.Preferences.Any(p => p.Id == request.PreferenceId), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>() { new AutoFaker<Customer>().RuleFor(c => c.Preferences, new List<Preference> { preference }).Generate() } );

        _promoCodesRepositoryMock
            .Setup(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        #endregion

        #region Act        
        var resultPromoCode = await _promoCodesController.Create(request, CancellationToken.None);
        #endregion

        #region Assert
        resultPromoCode.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)resultPromoCode.Result;

        _promoCodesRepositoryMock.Verify(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()), Times.Once);
        _partnersRepositoryMock.Verify(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()), Times.Once);

        createdAtActionResult.Value.Should().BeOfType<PromoCodeShortResponse>();
        var promoCodeShortResponse = (PromoCodeShortResponse)createdAtActionResult.Value;        

        promoCodeShortResponse.PartnerId.Should().Be(request.PartnerId);
        activeLimit.IssuedCount.Should().Be(1);
        #endregion
    }
}
