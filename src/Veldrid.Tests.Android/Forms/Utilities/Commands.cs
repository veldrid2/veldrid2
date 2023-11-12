using System;
using System.Windows.Input;
using Veldrid.Tests.Android.Utilities;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms.Utilities
{
    static class Commands
    {
        public static ICommand LaunchUrl { get; } = new DelegateCommand<string>(OnLaunchUrl);

        static void OnLaunchUrl(string str)
        {
            Device.OpenUri(new Uri(str));
        }
    }
}
