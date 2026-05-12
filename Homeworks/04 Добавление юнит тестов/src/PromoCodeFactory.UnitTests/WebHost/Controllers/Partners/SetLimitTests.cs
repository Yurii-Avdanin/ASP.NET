using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.Partners;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;
    
    private readonly PartnersController _partnersController;

    public SetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();
        _partnersController = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        #region Arrange
        var partnerId = Guid.NewGuid();
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>().Generate();

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);
        #endregion

        #region Act        
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);
        #endregion

        #region Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value;
        problemDetails.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");
        #endregion
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        #region Arrange        
        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.IsActive, _ => false ).Generate();
        
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>().Generate();

        _partnersRepositoryMock
           .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
           .ReturnsAsync(partner);
        #endregion

        #region Act
        var result = await _partnersController.CreateLimit(partner.Id, request, CancellationToken.None);
        #endregion

        #region Assert
        result.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        var objectResult = (UnprocessableEntityObjectResult)result.Result;        
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be("Partner blocked");
        problemDetails.Detail.Should().Be("Cannot create limit for a blocked partner.");
        #endregion
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        #region Arrange
        var endAt = DateTimeOffset.UtcNow.AddDays(30);
        var limitAt = 5;
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>()
            .RuleFor(p => p.Limit, limitAt)
            .RuleFor(p => p.EndAt, endAt)
            .Generate();

        var partner = CreatePartnerWithLimit(true);

        _partnersRepositoryMock
           .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
           .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        #endregion

        #region Act
        var result = await _partnersController.CreateLimit(partner.Id, request, CancellationToken.None);
        #endregion

        #region  Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result.Result;       

        createdAtActionResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();
        var partnerPromoCodeLimitResponse = (PartnerPromoCodeLimitResponse)createdAtActionResult.Value;

        partnerPromoCodeLimitResponse.Limit.Should().Be(limitAt);
        partnerPromoCodeLimitResponse.EndAt.Should().Be(endAt);

        _partnerLimitsRepositoryMock.Verify(repo => repo.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Once);
        #endregion
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        #region Arrange
        var endAt = DateTimeOffset.UtcNow.AddDays(30);
        var limitAt = 5;
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>()
            .RuleFor(p => p.Limit, limitAt)
            .RuleFor(p => p.EndAt, endAt)
            .Generate();

        var partner = CreatePartnerWithLimit(true);

        _partnersRepositoryMock
           .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
           .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        #endregion

        #region Act
        var result = await _partnersController.CreateLimit(partner.Id, request, It.IsAny<CancellationToken>());
        #endregion

        #region Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result.Result;

        // Проверяем, что Update был вызван один раз (если выл вызван метод Update, то в коллекции лимитов есть как минимум один элемент)
        _partnersRepositoryMock.Verify(repo => repo.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()), Times.Once);

        // Все лимиты должны быть отменены (CanceledAt == null) => false
        partner.PartnerLimits.Any(l => l.CanceledAt == null).Should().BeFalse();

        createdAtActionResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();
        var partnerPromoCodeLimitResponse = (PartnerPromoCodeLimitResponse)createdAtActionResult.Value;

        partnerPromoCodeLimitResponse.Limit.Should().Be(limitAt);
        partnerPromoCodeLimitResponse.EndAt.Should().Be(endAt);

        _partnerLimitsRepositoryMock.Verify(repo => repo.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Once);
        #endregion
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        #region Arrange
        var request = new AutoFaker<PartnerPromoCodeLimitCreateRequest>().Generate();
        var partner = CreatePartnerWithLimit(true);        

        _partnersRepositoryMock
           .Setup(r => r.GetById(partner.Id, true, It.IsAny<CancellationToken>()))
           .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(typeof(Partner), partner.Id));
        #endregion

        #region Act
        var result = await _partnersController.CreateLimit(partner.Id, request, It.IsAny<CancellationToken>());
        #endregion

        #region Assert
        result.Result.Should().BeOfType<NotFoundResult>();        

        // Проверяем, что Update был вызван один раз 
        _partnersRepositoryMock.Verify(repo => repo.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()), Times.Once);

        // Проверяем, что Add не был вызван, т.к. метод прервался на Update
        _partnerLimitsRepositoryMock.Verify(repo => repo.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Never);
        #endregion
    }    

    private static Partner CreatePartnerWithLimit(bool isActive, DateTimeOffset? canceledAt = null)
    {
        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.IsActive, isActive)
            .RuleFor(p => p.PartnerLimits, new List<PartnerPromoCodeLimit>())
            .Generate();

        partner.PartnerLimits.Add(
            new AutoFaker<PartnerPromoCodeLimit>()
                .RuleFor(l => l.Partner, partner)
                .RuleFor(l => l.CanceledAt, canceledAt)
                .RuleFor(l => l.CreatedAt, DateTimeOffset.UtcNow.AddDays(-5))
                .RuleFor(l => l.EndAt, DateTimeOffset.UtcNow.AddDays(10))
                .Generate());

        return partner;
    }
}
