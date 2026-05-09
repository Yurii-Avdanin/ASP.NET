using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController : BaseController
{

    private readonly IRepository<Employee> _employeeEfRepository;
    private readonly IRepository<Customer> _customerEfRepository;
    private readonly IRepository<PromoCode> _promoCodeEfRepository;
    private readonly IRepository<Preference> _preferenceEfRepository;
    private readonly IRepository<CustomerPromoCode> _customerPromoCodeEfRepository;

    public PromoCodesController(
        IRepository<Employee> employeeEfRepository,
        IRepository<Customer> customerEfRepository,
        IRepository<PromoCode> promoCodeEfRepository,
        IRepository<Preference> preferenceEfRepository,
        IRepository<CustomerPromoCode> customerPromoCodeEfRepository
        )
    {
        _employeeEfRepository = employeeEfRepository;
        _customerEfRepository = customerEfRepository;
        _promoCodeEfRepository = promoCodeEfRepository;
        _preferenceEfRepository = preferenceEfRepository;
        _customerPromoCodeEfRepository = customerPromoCodeEfRepository;
    }
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await _promoCodeEfRepository.GetAll(true, ct);
        return Ok(promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse));        
    }

    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promoCode = await _promoCodeEfRepository.GetById(id, true, ct);
        if (promoCode is null)
            return NotFound($"PromoCode с Id {id} не найден.");        

        return Ok(PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }

    /// <summary>
    /// Создать промокод и выдать его клиентам с указанным предпочтением
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> Create(PromoCodeCreateRequest request, CancellationToken ct)
    {
        var partnerManager = await _employeeEfRepository.GetById(request.PartnerManagerId, true, ct);
        if (partnerManager is null)
            return NotFound($"Employee с Id {request.PartnerManagerId} не найден.");

        var preference = await _preferenceEfRepository.GetById(request.PreferenceId, true, ct);
        if (preference is null)
            return NotFound($"Preference с Id {request.PartnerManagerId} не найден.");        
        
        var customerPreference = await _customerEfRepository
            .GetWhere(c => c.Preferences.Any(p => p.Id == request.PreferenceId), ct: ct);

        var promoCodeId = Guid.NewGuid();

        var customerPromoCodes = customerPreference.Select(cp => new CustomerPromoCode
        {
            Id = Guid.NewGuid(),
            CustomerId = cp.Id,
            PromoCodeId = promoCodeId,
            CreatedAt = DateTime.UtcNow,
            AppliedAt = null
        }).ToList();

        var promoCode = new PromoCode
        {
            Id = promoCodeId,
            Code = request.Code,
            ServiceInfo = request.ServiceInfo,
            BeginDate = request.BeginDate.UtcDateTime,
            EndDate = request.EndDate.UtcDateTime,
            PartnerName = request.PartnerName,
            PartnerManager = partnerManager,
            Preference = preference,
            CustomerPromoCodes = customerPromoCodes
        };

        await _promoCodeEfRepository.Add(promoCode, ct);

        return CreatedAtAction(
            nameof(Create),
            new { id = promoCode.Id },
            PromoCodesMapper.ToPromoCodeShortResponse(promoCode));        
    }

    /// <summary>
    /// Применить промокод (отметить, что клиент использовал промокод)
    /// </summary>
    [HttpPost("{id:guid}/apply")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Apply(
        [FromRoute] Guid id,
        [FromBody] PromoCodeApplyRequest request,
        CancellationToken ct)
    {
        var customerPromoCodes = await _customerPromoCodeEfRepository
            .GetWhere(cpc => cpc.PromoCodeId == id && cpc.CustomerId == request.CustomerId, ct: ct);

        var customerPromoCode = customerPromoCodes.FirstOrDefault();
        if (customerPromoCode is null)
            return NotFound($"CustomerPromoCode с PromoCodeId {id} и CustomerId {request.CustomerId} не найден.");

        customerPromoCode.AppliedAt = DateTime.UtcNow;

        await _customerPromoCodeEfRepository.Update(customerPromoCode, ct);

        return NoContent();
    }
}
