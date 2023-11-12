using System.Threading;
using System.Threading.Tasks;
using Veldrid.Tests.Android.ViewModels;

namespace Veldrid.Tests.Android.Sinks
{
    public class NullResultChannel : IResultChannel
    {
        public Task CloseChannelAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> OpenChannelAsync(CancellationToken cancellationToken, string? message = null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<bool>(cancellationToken);
            }
            return Task.FromResult(true);
        }

        public ValueTask RecordResultAsync(TestResultViewModel result, CancellationToken cancellationToken)
        {
            return ValueTask.CompletedTask;
        }
    }
}
