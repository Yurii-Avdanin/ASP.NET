using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
public class EmployeesController(
    IRepository<Employee> employeeRepository,
    IRepository<Role> roleRepository
    ) : BaseController
{
    /// <summary>
    /// Получить данные всех сотрудников
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeShortResponse>>> Get(CancellationToken ct)
    {
        var employees = await employeeRepository.GetAll(ct);

        var employeesModels = employees.Select(Mapper.ToEmployeeShortResponse).ToList();

        return Ok(employeesModels);
    }

    /// <summary>
    /// Получить данные сотрудника по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var employee = await employeeRepository.GetById(id, ct);

        return employee is null ?
            NotFound(new ProblemDetails { Title = "Ошибка получения", Detail = $"Сотрудник с таким id = \"{id}\" не найден." }) :
            Ok( Mapper.ToEmployeeResponse(employee) );
    }

    /// <summary>
    /// Создать сотрудника
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody] EmployeeCreateRequest request, CancellationToken ct)
    {        
        var roles = await roleRepository.GetAll(ct);
        var role = roles.FirstOrDefault(r => r.Id == request.RoleId);

        if (role is null)
            return BadRequest(new ProblemDetails { Title = "Ошибка создания", Detail = $"Роль с указанным id = \"{request.RoleId}\" не найдена." });
        try
        {
            await employeeRepository.Add(Mapper.ToEmployee(request, role), ct);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Ошибка создания", Detail = ex.Message } );
        }

        return Created();
    }    

    /// <summary>
    /// Обновить сотрудника
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] EmployeeUpdateRequest request,
        CancellationToken ct)
    {        
        var employee = await employeeRepository.GetById(id, ct);

        if (employee is null)
            return NotFound(new ProblemDetails { Title = "Ошибка обновления", Detail = $"Сотрудник с таким id = \"{id}\" не найден." } );
        
        var roles = await roleRepository.GetAll(ct);
        var role = roles.FirstOrDefault(r => r.Id == request.RoleId);
        if (role is null)
            return BadRequest(new ProblemDetails { Title = "Ошибка обновления", Detail = $"Некорректный id = \"{id}\" роли. Роль с таким id не найдена." } );

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.Role = role;

        try
        {
            await employeeRepository.Update(employee, ct);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Ошибка обновления", Detail = ex.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Ошибка обновления", Detail = ex.Message });
        }

        return Ok(Mapper.ToEmployeeResponse(employee));
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var employee = await employeeRepository.GetById(id, ct);

        if (employee is null)
            return NotFound(new ProblemDetails { Title = "Ошибка удаления", Detail = $"Сотрудник с таким id = \"{id}\" не найден." } );

        try
        {
            await employeeRepository.Delete(employee.Id, ct);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Ошибка удаления", Detail = ex.Message });
        }

        return NoContent();
    }
}
