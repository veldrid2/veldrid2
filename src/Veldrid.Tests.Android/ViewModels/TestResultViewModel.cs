using System;
using Xunit.Abstractions;

namespace Veldrid.Tests.Android.ViewModels
{
    public class TestResultViewModel : ViewModelBase
    {
        private TestCaseViewModel _testCase;
        private ITestResultMessage? _testResult;

        private TimeSpan duration;
        private string errorMessage = string.Empty;
        private string errorStackTrace = string.Empty;

        public TestResultViewModel(TestCaseViewModel testCase, ITestResultMessage? testResult)
        {
            _testCase = testCase ?? throw new ArgumentNullException(nameof(testCase));
            _testResult = testResult;

            if (_testResult != null)
                _testCase.UpdateTestState(this);
        }

        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                if (Set(ref duration, value))
                {
                    _testCase?.UpdateTestState(this);
                }
            }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { Set(ref errorMessage, value); }
        }

        public string ErrorStackTrace
        {
            get { return errorStackTrace; }
            set { Set(ref errorStackTrace, value); }
        }

        public TestCaseViewModel TestCase
        {
            get { return _testCase; }
            private set { Set(ref _testCase, value); }
        }

        public ITestResultMessage? TestResultMessage
        {
            get { return _testResult; }
            private set { Set(ref _testResult, value); }
        }

        public string Output => TestResultMessage?.Output ?? string.Empty;

        public bool HasOutput => !string.IsNullOrWhiteSpace(Output);
    }
}
