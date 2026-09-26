using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<Employee> _employeeRepository;

    public EmployeeService(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Employee> UpdateAppliedPromocodesAsync(Guid employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            return null;

        employee.AppliedPromocodesCount++;
        await _employeeRepository.UpdateAsync(employee);

        return employee;
    }
}