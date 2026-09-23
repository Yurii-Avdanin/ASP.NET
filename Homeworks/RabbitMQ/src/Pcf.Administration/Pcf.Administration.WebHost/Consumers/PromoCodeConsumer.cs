using MassTransit;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Shared.Contracts;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers;

public class PromoCodeConsumer : IConsumer<PromoCodeMessage>
{
    private readonly IEmployeeService _employeeService;

    public PromoCodeConsumer(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task Consume(ConsumeContext<PromoCodeMessage> context)
    {
        var message = context.Message;
        if (message.PartnerManagerId.HasValue)        
            await _employeeService.UpdateAppliedPromocodesAsync(message.PartnerManagerId.Value);        
    }
}
