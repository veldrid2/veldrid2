using System;
using System.Windows.Input;
using Veldrid.Tests.Android.Utilities;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms.Pages
{
    partial class CreditsPage : ContentPage
    {
        public ICommand LaunchUrl { get; } = new DelegateCommand<string>(OnLaunchUrl);

        public CreditsPage()
        {
            InitializeComponent();
        }

        static void OnLaunchUrl(string str)
        {
            Device.OpenUri(new Uri(str));
        }
    }
}
