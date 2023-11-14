using System.Threading;
using System.Threading.Tasks;

namespace Veldrid.Tests.Android.Sinks
{
    public interface IResultChannel : ITestListener
    {
        Task<bool> OpenChannelAsync(CancellationToken cancellationToken, string? message = null);

        Task CloseChannelAsync();
    }
}
