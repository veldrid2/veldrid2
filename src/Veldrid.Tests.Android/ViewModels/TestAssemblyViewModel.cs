using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Veldrid.Tests.Android.Utilities;

namespace Veldrid.Tests.Android.ViewModels
{
    public class TestAssemblyViewModel : ViewModelBase
    {
        readonly ObservableCollection<TestCaseViewModel> allTests;
        readonly FilteredCollectionView<TestCaseViewModel, (string?, TestState)> filteredTests;
        readonly DelegateCommand runAllTestsCommand;
        readonly DelegateCommand runFilteredTestsCommand;

        readonly DeviceRunner runner;
        string? detailText;
        string? displayName;
        CancellationTokenSource? filterCancellationTokenSource;
        bool isBusy;
        TestState result;
        TestState resultFilter;
        RunStatus runStatus;
        string? searchQuery;
        private int _notRun;
        private int _passed;
        private int _failed;
        private int _skipped;

        internal TestAssemblyViewModel(AssemblyRunInfo runInfo, DeviceRunner runner)
        {
            RunInfo = runInfo;
            this.runner = runner;

            runAllTestsCommand = new DelegateCommand(RunAllTests, () => !isBusy);
            runFilteredTestsCommand = new DelegateCommand(RunFilteredTests, () => !isBusy);

            DisplayName = runInfo.Assembly.GetName().Name;

            allTests = new ObservableCollection<TestCaseViewModel>(runInfo.TestCases);

            filteredTests = new FilteredCollectionView<TestCaseViewModel, (string?, TestState)>(
                allTests,
                IsTestFilterMatch,
                (SearchQuery, ResultFilter),
                new TestComparer());

            filteredTests.ItemChanged += (sender, args) => UpdateCaption();
            filteredTests.CollectionChanged += (sender, args) => UpdateCaption();

            Result = TestState.NotRun;
            RunStatus = RunStatus.NotRun;

            UpdateCaption();
        }

        public ICommand RunAllTestsCommand => runAllTestsCommand;

        public ICommand RunFilteredTestsCommand => runFilteredTestsCommand;

        public IList<TestCaseViewModel> TestCases => filteredTests;

        public string? DetailText
        {
            get { return detailText; }
            private set { Set(ref detailText, value); }
        }

        public string? DisplayName
        {
            get { return displayName; }
            private set { Set(ref displayName, value); }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            private set
            {
                if (Set(ref isBusy, value))
                {
                    runAllTestsCommand.RaiseCanExecuteChanged();
                    runFilteredTestsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public TestState Result
        {
            get { return result; }
            set { Set(ref result, value); }
        }

        public TestState ResultFilter
        {
            get { return resultFilter; }
            set
            {
                if (Set(ref resultFilter, value))
                {
                    FilterAfterDelay();
                }
            }
        }

        public RunStatus RunStatus
        {
            get { return runStatus; }
            private set { Set(ref runStatus, value); }
        }

        public string? SearchQuery
        {
            get { return searchQuery; }
            set
            {
                if (Set(ref searchQuery, value))
                {
                    FilterAfterDelay();
                }
            }
        }

        internal AssemblyRunInfo RunInfo { get; }

        void FilterAfterDelay()
        {
            filterCancellationTokenSource?.Cancel();

            filterCancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = filterCancellationTokenSource.Token;

            Task.Delay(500, token)
                .ContinueWith(
                    x => { filteredTests.FilterArgument = (SearchQuery, ResultFilter); },
                    token,
                    TaskContinuationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        static bool IsTestFilterMatch(TestCaseViewModel test, (string?, TestState) query)
        {
            if (test == null) throw new ArgumentNullException(nameof(test));
            TestState? requiredTestState = query.Item2 switch
            {
                TestState.All => null,
                TestState.Passed => (TestState?)TestState.Passed,
                TestState.Failed => (TestState?)TestState.Failed,
                TestState.Skipped => (TestState?)TestState.Skipped,
                TestState.NotRun => (TestState?)TestState.NotRun,
                _ => throw new ArgumentException(null, nameof(query)),
            };
            if (requiredTestState.HasValue && test.Result != requiredTestState.Value)
            {
                return false;
            }

            string? pattern = query.Item1;
            return string.IsNullOrWhiteSpace(pattern) || test.DisplayName.Contains(pattern.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        async void RunAllTests()
        {
            try
            {
                IsBusy = true;
                await runner.RunAsync(new[] { RunInfo });
            }
            finally
            {
                IsBusy = false;
            }
        }

        async void RunFilteredTests()
        {
            try
            {
                IsBusy = true;
                await runner.RunAsync(filteredTests);
            }
            finally
            {
                IsBusy = false;
            }
        }


        public int NotRun { get => _notRun; set => Set(ref _notRun, value); }
        public int Passed { get => _passed; set => Set(ref _passed, value); }
        public int Failed { get => _failed; set => Set(ref _failed, value); }
        public int Skipped { get => _skipped; set => Set(ref _skipped, value); }

        void UpdateCaption()
        {
            int count = allTests.Count;
            if (count == 0)
            {
                DetailText = "no test was found inside this assembly";

                RunStatus = RunStatus.NoTests;
            }
            else
            {
                IEnumerable<IGrouping<TestState, TestCaseViewModel>> outcomes = allTests.GroupBy(r => r.Result);
                Dictionary<TestState, int> results = outcomes.ToDictionary(k => k.Key, v => v.Count());

                results.TryGetValue(TestState.Passed, out int positive);
                results.TryGetValue(TestState.Failed, out int failure);
                results.TryGetValue(TestState.Skipped, out int skipped);
                results.TryGetValue(TestState.NotRun, out int notRun);

                Passed = positive;
                Failed = failure;
                Skipped = skipped;
                NotRun = notRun;

                string prefix = notRun == 0 ? "Complete - " : string.Empty;

                // No failures and all run
                if (failure == 0 && notRun == 0)
                {
                    DetailText = $"{prefix}✔ {positive}";
                    RunStatus = RunStatus.Ok;

                    Result = TestState.Passed;
                }
                else if (failure > 0 || notRun > 0 && notRun < count)
                {
                    // we either have failures or some of the tests are not run
                    DetailText = $"{prefix}✔ {positive}, ⛔ {failure}, ⚠ {skipped}, 🔷 {notRun}";

                    if (failure > 0) // always show a fail
                    {
                        RunStatus = RunStatus.Failed;
                        Result = TestState.Failed;
                    }
                    else
                    {
                        if (positive > 0)
                        {
                            RunStatus = RunStatus.Ok;
                            Result = TestState.Passed;
                        }
                        else if (skipped > 0)
                        {
                            RunStatus = RunStatus.Skipped;
                            Result = TestState.Skipped;
                        }
                        else
                        {
                            // just not run
                            RunStatus = RunStatus.NotRun;
                            Result = TestState.NotRun;
                        }

                    }

                }
                else if (Result == TestState.NotRun)
                {
                    DetailText = $"🔷 {count}, {Result}";
                    RunStatus = RunStatus.NotRun;
                }
            }
        }

        class TestComparer : IComparer<TestCaseViewModel>
        {
            public int Compare(TestCaseViewModel? x, TestCaseViewModel? y)
            {
                int compare = string.Compare(x.DisplayName, y.DisplayName, StringComparison.OrdinalIgnoreCase);
                return compare;
            }
        }
    }
}
