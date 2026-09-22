using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;

public interface IPreferencesGateway
{
    Task<IEnumerable<Preference>> GetAllAsync();

    Task<Preference> GetByIdAsync(Guid id);
}
