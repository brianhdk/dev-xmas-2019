using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XMAS2019.Domain.Services
{
    public interface IZoneService
    {
        Task<IEnumerable<Zone>> GetZonesFor(Attempt attempt, int count, CancellationToken token);
    }
}