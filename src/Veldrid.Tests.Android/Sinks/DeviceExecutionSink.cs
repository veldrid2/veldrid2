using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Veldrid.Tests.Android.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace Veldrid.Tests.Android.Sinks
{
    class DeviceExecutionSink : TestMessageSink
    {
        readonly SynchronizationContext context;
        readonly ITestListener listener;
        readonly Dictionary<ITestCase, TestCaseViewModel> testCases;

        public DeviceExecutionSink(
            Dictionary<ITestCase, TestCaseViewModel> testCases,
            ITestListener listener,
            SynchronizationContext context)
        {
            this.testCases = testCases ?? throw new ArgumentNullException(nameof(testCases));
            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
            this.context = context ?? throw new ArgumentNullException(nameof(context));

            Execution.TestFailedEvent += HandleTestFailed;
            Execution.TestPassedEvent += HandleTestPassed;
            Execution.TestSkippedEvent += HandleTestSkipped;
        }

        void HandleTestFailed(MessageHandlerArgs<ITestFailed> args)
        {
            _ = MakeTestResultViewModel(args.Message, TestState.Failed);
        }

        void HandleTestPassed(MessageHandlerArgs<ITestPassed> args)
        {
            _ = MakeTestResultViewModel(args.Message, TestState.Passed);
        }

        void HandleTestSkipped(MessageHandlerArgs<ITestSkipped> args)
        {
            _ = MakeTestResultViewModel(args.Message, TestState.Skipped);
        }

        async Task MakeTestResultViewModel(ITestResultMessage testResult, TestState outcome)
        {
            if (!testCases.TryGetValue(testResult.TestCase, out TestCaseViewModel? testCase))
            {
                // no matching reference, search by Unique ID as a fallback
                testCase = testCases.FirstOrDefault(kvp => kvp.Key.UniqueID?.Equals(testResult.TestCase.UniqueID) ?? false).Value;
                if (testCase == null)
                {
                    return;
                }
            }

            TaskCompletionSource<TestResultViewModel> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

            // Create the result VM on the UI thread as it updates properties
            context.Post(_ =>
            {
                TestResultViewModel result = new(testCase, testResult)
                {
                    Duration = TimeSpan.FromSeconds((double)testResult.ExecutionTime)
                };

                if (outcome == TestState.Failed)
                {
                    result.ErrorMessage = ExceptionUtility.CombineMessages((ITestFailed)testResult);
                    result.ErrorStackTrace = ExceptionUtility.CombineStackTraces((ITestFailed)testResult);
                }

                tcs.TrySetResult(result);
            }, null);

            TestResultViewModel r = await tcs.Task;
            await listener.RecordResultAsync(r, CancellationToken.None); // bring it back to the threadpool thread
        }

    }
}
