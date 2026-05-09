using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController : BaseController
{
    private readonly IRepository<Customer> _customerEfRepository;    
    private readonly IRepository<PromoCode> _promoCodeEfRepository;
    private readonly IRepository<Preference> _preferenceEfRepository;

    public CustomersController(
        IRepository<Customer> customerEfRepository,        
        IRepository<PromoCode> promoCodeEfRepository,
        IRepository<Preference> preferenceEfRepository)
    {
        _customerEfRepository = customerEfRepository;        
        _promoCodeEfRepository = promoCodeEfRepository;
        _preferenceEfRepository = preferenceEfRepository;
    }

    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var listCustomer =  await _customerEfRepository.GetAll(true, ct);        
        return Ok(listCustomer.Select(CustomersMapper.ToCustomerShortResponse));
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await _customerEfRepository.GetById(id, true, ct);
        if (customer is null)
            return NotFound($"Customer с Id {id} не найден.");

        var promoCodeIds = customer.CustomerPromoCodes.Select(x => x.PromoCodeId).ToList();       
        var promoCodes = await _promoCodeEfRepository.GetByRangeId(promoCodeIds, true, ct);        

        return Ok(CustomersMapper.ToCustomerResponse(customer, promoCodes));
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        var preferences = new List<Preference>();
        foreach (var id in request.PreferenceIds )
        {
            var preference = await _preferenceEfRepository.GetById(id, ct: ct);
            if (preference is null)
                return BadRequest($"Preference с Id {id} не найден.");

            preferences.Add(preference);
        }

        var customer = CustomersMapper.ToCustomer(request, preferences);

        await _customerEfRepository.Add(customer, ct);

        return CreatedAtAction(
            nameof(Create),
            new { id = customer.Id },
            CustomersMapper.ToCustomerShortResponse(customer));
    }

    /// <summary>
    /// Обновить клиента
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerShortResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] CustomerUpdateRequest request,
        CancellationToken ct)
    {
        var customer = await _customerEfRepository.GetById(id, true, ct);
        if (customer is null)
            return BadRequest($"Customer с Id {id} не найден.");

        var preferences = new List<Preference>();
        foreach (var preferenceId in request.PreferenceIds)
        {
            var preference = await _preferenceEfRepository.GetById(preferenceId, ct: ct);
            if (preference is null)
                return BadRequest($"Preference с Id {preferenceId} не найден.");

            preferences.Add(preference);
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Preferences = preferences;

        await _customerEfRepository.Update(customer, ct);

        return Ok(CustomersMapper.ToCustomerShortResponse(customer));
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var customer = await _customerEfRepository.GetById(id, true, ct);
        if (customer is null)
            return NotFound($"Customer с Id {id} не найден.");

        await _customerEfRepository.Delete(id, ct);

        return NoContent();
    }
}
