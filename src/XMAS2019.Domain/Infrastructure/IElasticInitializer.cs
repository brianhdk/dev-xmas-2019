using System.Threading;
using System.Threading.Tasks;

namespace XMAS2019.Domain.Infrastructure
{
    public interface IElasticInitializer
    {
        Task Initialize(CancellationToken token);
    }
}