using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Veldrid.Tests.Android.Forms;
using Veldrid.Tests.Android.Utilities;

namespace Veldrid.Tests.Android.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        readonly Navigator _navigator;
        readonly DeviceRunner _runner;

        readonly DelegateCommand runEverythingCommand;

        public event EventHandler? ScanComplete;

        bool isBusy;

        internal HomeViewModel(Navigator navigator, DeviceRunner runner)
        {
            _navigator = navigator;
            _runner = runner;

            TestAssemblies = new ObservableCollection<TestAssemblyViewModel>();

            OptionsCommand = new DelegateCommand(OptionsExecute);
            CreditsCommand = new DelegateCommand(CreditsExecute);
            runEverythingCommand = new DelegateCommand(RunEverythingExecute, () => !isBusy);
            NavigateToTestAssemblyCommand = new DelegateCommand<object>(async vm => await navigator.NavigateToAsync(NavigationPage.AssemblyTestList, vm));

            runner.OnDiagnosticMessage += RunnerOnOnDiagnosticMessage;

            StartAssemblyScan();
        }

        void RunnerOnOnDiagnosticMessage(string s)
        {
            DiagnosticMessages += $"{s}{Environment.NewLine}{Environment.NewLine}";
        }

        public ObservableCollection<TestAssemblyViewModel> TestAssemblies { get; }

        private string diagnosticMessages = string.Empty;
        public string DiagnosticMessages
        {
            get { return diagnosticMessages; }
            set { Set(ref diagnosticMessages, value); }
        }

        void OptionsExecute()
        {
            Debug.WriteLine("Options");
        }

        async void CreditsExecute()
        {
            await _navigator.NavigateToAsync(NavigationPage.Credits);
        }

        async void RunEverythingExecute()
        {
            try
            {
                IsBusy = true;
                await Run();
            }
            finally
            {
                IsBusy = false;
            }
        }


        public ICommand OptionsCommand { get; private set; }
        public ICommand CreditsCommand { get; private set; }

        public ICommand RunEverythingCommand => runEverythingCommand;

        public ICommand NavigateToTestAssemblyCommand { get; private set; }

        public bool IsBusy
        {
            get { return isBusy; }
            private set
            {
                if (Set(ref isBusy, value))
                {
                    runEverythingCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public async void StartAssemblyScan()
        {
            IsBusy = true;
            try
            {
                IAsyncEnumerable<TestAssemblyViewModel> allTests = _runner.Discover();

                // Back on UI thread
                await foreach (TestAssemblyViewModel vm in allTests)
                {
                    TestAssemblies.Add(vm);
                }

                ScanComplete?.Invoke(this, EventArgs.Empty);

            }
            finally
            {
                IsBusy = false;
            }

            if (RunnerOptions.Current.AutoStart)
            {
                await Run();

                if (RunnerOptions.Current.TerminateAfterExecution)
                {
                    PlatformHelpers.TerminateWithSuccess();
                }
            }
        }

        Task Run()
        {
            if (!string.IsNullOrWhiteSpace(DiagnosticMessages))
            {
                DiagnosticMessages += $"-----------{Environment.NewLine}";
            }
            return _runner.RunAsync(TestAssemblies.Select(t => t.RunInfo).ToList(), "Run Everything");
        }
    }
}
