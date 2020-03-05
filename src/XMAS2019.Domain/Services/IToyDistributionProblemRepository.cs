using System;
using System.Threading;
using System.Threading.Tasks;

namespace XMAS2019.Domain.Services
{
    public interface IToyDistributionProblemRepository
    {
        Task<Uri> Save(Attempt attempt, ToyDistributionProblem problem, CancellationToken token);

        Task<ToyDistributionProblem> Get(Attempt attempt, CancellationToken token);
    }
}