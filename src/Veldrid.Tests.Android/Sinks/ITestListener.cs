using System.Threading;
using System.Threading.Tasks;
using Veldrid.Tests.Android.ViewModels;

namespace Veldrid.Tests.Android.Sinks
{
    public interface ITestListener
    {
        ValueTask RecordResultAsync(TestResultViewModel result, CancellationToken cancellationToken);
    }
}
