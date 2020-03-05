using System;
using System.Threading;
using System.Threading.Tasks;

namespace XMAS2019.Domain.Services
{
    public interface IAttemptService
    {
        Task<Attempt> Get(Guid id, int expectedVersion, CancellationToken token);

        Task Save(Attempt attempt, int expectedVersion, CancellationToken token);

        Task<SantaTracking> GetSantaTracking(Attempt attempt, CancellationToken token);

        Task<Statistics> GetStatistics(Attempt lastAttempt, CancellationToken token);
    }
}