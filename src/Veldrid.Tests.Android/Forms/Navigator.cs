using System;
using System.Threading.Tasks;
using Veldrid.Tests.Android.Forms.Pages;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms
{
    public class Navigator
    {
        private INavigation _navigation;

        public Navigator(INavigation navigation)
        {
            _navigation = navigation;
        }

        public Task NavigateToAsync(NavigationPage page, object? dataContext = null)
        {
            ContentPage content = page switch
            {
                NavigationPage.Home => new HomePage(),
                NavigationPage.AssemblyTestList => new AssemblyTestListPage(),
                NavigationPage.TestResult => new TestResultPage(),
                NavigationPage.Credits => new CreditsPage(),
                _ => throw new ArgumentOutOfRangeException(nameof(page)),
            };
            content.BindingContext = dataContext;

            return _navigation.PushAsync(content);
        }
    }
}
