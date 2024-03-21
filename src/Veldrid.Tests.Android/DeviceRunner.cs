using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Veldrid.Tests.Android.Forms;
using Veldrid.Tests.Android.Sinks;
using Veldrid.Tests.Android.Utilities;
using Veldrid.Tests.Android.ViewModels;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Veldrid.Tests.Android
{
    public class DeviceRunner : ITestListener
    {
        readonly SynchronizationContext? context = SynchronizationContext.Current;

        readonly AsyncLock _executionLock = new();
        readonly Navigator _navigation;
        readonly IResultChannel _resultChannel;

        readonly CancellationTokenSource _cancellationSource = new();

        public DeviceRunner(IReadOnlyCollection<Assembly> testAssemblies, Navigator navigator, IResultChannel resultChannel)
        {
            _navigation = navigator;
            TestAssemblies = testAssemblies;
            _resultChannel = resultChannel;
        }

        public event Action<string>? OnDiagnosticMessage;

        public IReadOnlyCollection<Assembly> TestAssemblies { get; }

        public ValueTask RecordResultAsync(TestResultViewModel result, CancellationToken cancellationToken)
        {
            return _resultChannel.RecordResultAsync(result, cancellationToken);
        }

        public Task RunAsync(TestCaseViewModel test)
        {
            return RunAsync(new[] { test });
        }

        public Task RunAsync(IEnumerable<TestCaseViewModel> tests, string? message = null)
        {
            List<AssemblyRunInfo> groups = tests
                .GroupBy(t => t.Assembly)
                .Select(g => new AssemblyRunInfo(g.Key, GetConfiguration(g.Key.GetName().Name), g.ToList()))
                .ToList();

            return RunAsync(groups, message);
        }

        public async Task RunAsync(IReadOnlyList<AssemblyRunInfo> runInfos, string? message = null)
        {
            using (await _executionLock.LockAsync())
            {
                if (message == null)
                {
                    List<TestCaseViewModel>? testCases = runInfos.FirstOrDefault()?.TestCases;
                    message = runInfos.Count > 1 || testCases?.Count > 1
                        ? "Run Multiple Tests"
                        : testCases?.FirstOrDefault()?.DisplayName;
                }

                if (await _resultChannel.OpenChannelAsync(_cancellationSource.Token, message))
                {
                    try
                    {
                        await RunTestsAsync(runInfos, _cancellationSource.Token);
                    }
                    finally
                    {
                        await _resultChannel.CloseChannelAsync();
                    }
                }
            }
        }

        public async IAsyncEnumerable<TestAssemblyViewModel> Discover()
        {
            await foreach (AssemblyRunInfo runInfo in DiscoverTestsInAssemblies())
            {
                yield return new TestAssemblyViewModel(runInfo, this);
            }
        }

        private async IAsyncEnumerable<AssemblyRunInfo> DiscoverTestsInAssemblies()
        {
            foreach (Assembly assembly in TestAssemblies)
            {
                AssemblyRunInfo info = await Task.Run(() =>
                {
                    TestAssemblyConfiguration configuration = GetConfiguration(assembly.GetName().Name);
                    ITestFrameworkDiscoveryOptions discoveryOptions = TestFrameworkOptions.ForDiscovery(configuration);

                    if (_cancellationSource.IsCancellationRequested)
                    {
                        return null!;
                    }

                    using Xunit2Controller controller = new(new ReflectionAssemblyInfo(assembly));
                    using TestDiscoverySink sink = new(() => _cancellationSource.IsCancellationRequested);

                    controller.Find(false, sink, discoveryOptions);
                    sink.Finished.WaitOne();

                    List<TestCaseViewModel> testCases = sink.TestCases
                        .Select(tc => new TestCaseViewModel(assembly, tc, _navigation, this))
                        .ToList();

                    return new AssemblyRunInfo(assembly, configuration, testCases);
                });

                if (info != null)
                {
                    yield return info;
                }
            }
        }

        private static TestAssemblyConfiguration GetConfiguration(string? assemblyName)
        {
            if (assemblyName != null)
            {
                Stream? stream = GetConfigurationStreamForAssembly(assemblyName);
                if (stream != null)
                {
                    using (stream)
                    {
                        return ConfigReader.Load(stream);
                    }
                }
            }
            return new TestAssemblyConfiguration();
        }

        private static Stream? GetConfigurationStreamForAssembly(string assemblyName)
        {
            return PlatformHelpers.ReadConfigJson(assemblyName);
        }

        private async Task RunTestsAsync(IEnumerable<AssemblyRunInfo> testCases, CancellationToken cancellationToken)
        {
            List<Task> tasks = new();
            List<AssemblyRunInfo> syncInfos = new();

            foreach (AssemblyRunInfo runInfo in testCases)
            {
                if (runInfo.Configuration.ParallelizeTestCollectionsOrDefault)
                {
                    tasks.Add(Task.Run(() => RunTestsInAssembly(runInfo, cancellationToken), cancellationToken));
                }
                else
                {
                    syncInfos.Add(runInfo);
                }
            }

            await Task.WhenAll(tasks);

            foreach (AssemblyRunInfo runInfo in syncInfos)
            {
                RunTestsInAssembly(runInfo, cancellationToken);
            }
        }

        private void RunTestsInAssembly(AssemblyRunInfo runInfo, CancellationToken cancellationToken)
        {
            int longRunningSeconds = runInfo.Configuration.LongRunningTestSecondsOrDefault;

            using Xunit2Controller controller = new(new ReflectionAssemblyInfo(runInfo.Assembly));

            Dictionary<ITestCase, TestCaseViewModel> xunitTestCases = runInfo.TestCases
                .Select(tc => (vm: tc, tc: tc.TestCase))
                .Where(tc => tc.tc.UniqueID != null)
                .ToDictionary(tc => tc.tc, tc => tc.vm);

            ITestFrameworkExecutionOptions executionOptions = TestFrameworkOptions.ForExecution(runInfo.Configuration);

            DeviceExecutionSink deviceExecSink = new(xunitTestCases, this, context);

            IExecutionSink resultsSink = new DelegatingExecutionSummarySink(deviceExecSink, () => cancellationToken.IsCancellationRequested);
            if (longRunningSeconds > 0)
            {
                DiagnosticMessageSink diagSink = new(
                    d => context.Post(_ => OnDiagnosticMessage?.Invoke(d), null),
                    runInfo.Assembly.GetName().Name, executionOptions.GetDiagnosticMessagesOrDefault());

                resultsSink = new DelegatingLongRunningTestDetectionSink(resultsSink, TimeSpan.FromSeconds(longRunningSeconds), diagSink);
            }

            string? asmName = runInfo.Assembly.GetName().Name;
            string asmPath = runInfo.Assembly.Location;
            if (asmPath == "")
            {
                asmPath = Path.Combine(AppContext.BaseDirectory, Path.GetFileName(asmName) ?? "");
            }

            XunitProjectAssembly assm = new() { AssemblyFilename = asmPath };
            deviceExecSink.OnMessage(new TestAssemblyExecutionStarting(assm, executionOptions));

            controller.RunTests(xunitTestCases.Select(tc => tc.Value.TestCase).ToList(), resultsSink, executionOptions);
            resultsSink.Finished.WaitOne();

            deviceExecSink.OnMessage(new TestAssemblyExecutionFinished(assm, executionOptions, resultsSink.ExecutionSummary));
        }
    }
}
