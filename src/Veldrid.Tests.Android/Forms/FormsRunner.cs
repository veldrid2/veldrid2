using System.Collections.Generic;
using System.Reflection;
using Veldrid.Tests.Android.Forms.Pages;
using Veldrid.Tests.Android.Sinks;
using Veldrid.Tests.Android.ViewModels;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms
{
    class FormsRunner : Application
    {
        readonly IReadOnlyCollection<Assembly> testAssemblies;
        readonly IResultChannel resultChannel;

        public FormsRunner(IReadOnlyCollection<Assembly> testAssemblies, IResultChannel resultChannel)
        {
            this.testAssemblies = testAssemblies;
            this.resultChannel = resultChannel;

            MainPage = GetMainPage();
        }

        Page GetMainPage()
        {
            HomePage hp = new();
            Navigator nav = new(hp.Navigation);

            DeviceRunner runner = new(testAssemblies, nav, resultChannel);
            HomeViewModel vm = new(nav, runner);

            hp.BindingContext = vm;

            return new Xamarin.Forms.NavigationPage(hp);
        }
    }
}
