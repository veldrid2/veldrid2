using System;
using System.Reflection;
using System.Threading.Tasks;
using Veldrid.Tests.Android.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Veldrid.Tests.Android.ViewModels
{
    public class TestCaseViewModel : ViewModelBase
    {
        private readonly Navigator _navigator;
        private readonly DeviceRunner _runner;

        private TestState result;
        private RunStatus runStatus;
        private string stackTrace;

        private string message;
        private string output;

        private ITestCase testCase;
        private TestResultViewModel testResult;

        internal TestCaseViewModel(Assembly assembly, ITestCase testCase, Navigator navigator, DeviceRunner runner)
        {
            _navigator = navigator;
            _runner = runner;

            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.testCase = testCase ?? throw new ArgumentNullException(nameof(testCase));

            result = TestState.NotRun;
            runStatus = RunStatus.NotRun;
            stackTrace = "";
            message = "🔷 not run";
            output = "";

            // Create an initial result representing not run
            testResult = new TestResultViewModel(this, null);
        }

        public Assembly Assembly { get; }

        public string DisplayName => TestCase.DisplayName;

        // This should be raised on a UI thread as listeners will likely be UI elements

        public string Message
        {
            get { return message; }
            private set { Set(ref message, value); }
        }

        public string Output
        {
            get { return output; }
            private set { Set(ref output, value); }
        }

        public TestState Result
        {
            get { return result; }
            private set { Set(ref result, value); }
        }

        public RunStatus RunStatus
        {
            get { return runStatus; }
            set { Set(ref runStatus, value); }
        }

        public string StackTrace
        {
            get { return stackTrace; }
            private set { Set(ref stackTrace, value); }
        }

        public ITestCase TestCase
        {
            get { return testCase; }
            private set
            {
                if (Set(ref testCase, value))
                {
                    RaisePropertyChanged(nameof(DisplayName));
                }
            }
        }

        public TestResultViewModel TestResult
        {
            get { return testResult; }
            private set { Set(ref testResult, value); }
        }


        internal void UpdateTestState(TestResultViewModel message)
        {
            TestResult = message;

            Output = message.TestResultMessage?.Output ?? string.Empty;

            string msg = string.Empty;
            string stackTrace = string.Empty;
            RunStatus rs = RunStatus.NotRun;

            if (message.TestResultMessage is ITestPassed)
            {
                Result = TestState.Passed;
                msg = $"✔ Success! {TestResult.Duration.TotalMilliseconds} ms";
                rs = RunStatus.Ok;
            }
            if (message.TestResultMessage is ITestFailed failedMessage)
            {
                Result = TestState.Failed;
                msg = $"⛔ {ExceptionUtility.CombineMessages(failedMessage)}";
                stackTrace = ExceptionUtility.CombineStackTraces(failedMessage);
                rs = RunStatus.Failed;
            }
            if (message.TestResultMessage is ITestSkipped skipped)
            {
                Result = TestState.Skipped;
                msg = $"⚠ {skipped.Reason}";
                rs = RunStatus.Skipped;
            }

            Message = msg;
            StackTrace = stackTrace;
            RunStatus = rs;
        }

        public async Task NavigateToResultsPageAsync()
        {
            await _runner.RunAsync(this);

            await _navigator.NavigateToAsync(NavigationPage.TestResult, TestResult);
        }
    }
}
