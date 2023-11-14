using System;
using System.Linq;
using Veldrid.Tests.Android.Forms.Utilities;
using Veldrid.Tests.Android.ViewModels;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms.Pages
{
    partial class HomePage : ContentPage
    {
        private readonly static RunStatusToColorConverter RunStatusConverter = new();

        private HomeViewModel viewModel;

        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (viewModel != null)
            {
                viewModel.ScanComplete -= ScanComplete;
            }

            // Wire up the sections
            viewModel = (HomeViewModel)BindingContext;
            viewModel.ScanComplete += ScanComplete;

            root.Clear();
        }

        private void ScanComplete(object? sender, EventArgs e)
        {
            // Xam Forms requires us to redraw the table root to add new content
            TableRoot tr = new();
            TableSection fs = new("Test Assemblies");
            int i = 0;

            Thickness margin = new(0, 0, 5, 0);
            foreach (TestAssemblyViewModel ta in viewModel.TestAssemblies)
            {
                Label lblAssm = new();
                lblAssm.SetBinding(Label.TextProperty, nameof(ta.DisplayName));
                lblAssm.SetBinding(Label.TextColorProperty, nameof(ta.RunStatus), converter: RunStatusConverter);

                Label lblPassed = new()
                {
                    TextColor = Color.Green,
                    Margin = margin
                };
                lblPassed.SetBinding(Label.TextProperty, nameof(ta.Passed));

                Label lblFailed = new()
                {
                    TextColor = Color.Red,
                    Margin = margin
                };
                lblFailed.SetBinding(Label.TextProperty, nameof(ta.Failed));

                Label lblSkipped = new()
                {
                    TextColor = RunStatusToColorConverter.SkippedColor,
                    Margin = margin
                };
                lblSkipped.SetBinding(Label.TextProperty, nameof(ta.Skipped));

                Label lblNotRun = new()
                {
                    TextColor = Color.DarkGray
                };
                lblNotRun.SetBinding(Label.TextProperty, nameof(ta.NotRun));

                CommandViewCell vs = new()
                {
                    BindingContext = ta,
                    View = new StackLayout
                    {
                        Margin = new Thickness(20, 0, 0, 0),
                        Children =
                        {
                            lblAssm,
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new Label { Text = " ✔ ", TextColor = Color.Green },
                                    lblPassed,
                                    new Label { Text = " ⛔ " },
                                    lblFailed,
                                    new Label { Text = " ⚠ ", TextColor = RunStatusToColorConverter.SkippedColor },
                                    lblSkipped,
                                    new Label { Text = " 🔷 " },
                                    lblNotRun
                                }
                            }
                        }
                    },
                    AutomationId = $"testAssembly_{i}",
                    CommandParameter = ta,
                    Command = viewModel.NavigateToTestAssemblyCommand
                };

                i++;

                fs.Add(vs);
            }
            tr.Add(fs); // add the first section

            AutomationTextCell run = new()
            {
                Text = "Run Everything",
                Command = viewModel.RunEverythingCommand,
                AutomationId = "runEverything"
            };

            table.Root.Skip(1).First().Insert(0, run);
            tr.Add(table.Root.Skip(1)); // Skip the first section and add the others

            table.Root = tr;
        }
    }
}
