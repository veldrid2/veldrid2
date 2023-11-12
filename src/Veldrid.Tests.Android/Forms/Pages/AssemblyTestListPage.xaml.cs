using System;
using Veldrid.Tests.Android.ViewModels;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms.Pages
{
    public partial class AssemblyTestListPage : ContentPage
    {
        public AssemblyTestListPage()
        {
            InitializeComponent();
        }

        private void Picker_SelectedIndexChanged(object? sender, EventArgs eventArgs)
        {
            int i = (sender as Picker)!.SelectedIndex;

            TestState state = i switch
            {
                0 => TestState.All,
                1 => TestState.Passed,
                2 => TestState.Failed,
                3 => TestState.Skipped,
                4 => TestState.NotRun,
                _ => throw new NotImplementedException(),
            };

            if (BindingContext is TestAssemblyViewModel vm)
            {
                vm.ResultFilter = state;
            }
        }

        void Cell_OnTapped(object? sender, EventArgs e)
        {
            _ = ((TestCaseViewModel)((BindableObject)sender!).BindingContext).NavigateToResultsPageAsync();
        }
    }
}
