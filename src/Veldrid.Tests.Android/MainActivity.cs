using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Veldrid.Tests.Android
{
    [Activity(
        Theme = "@android:style/Theme.DeviceDefault.NoActionBar",
        Label = "@string/app_name",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : RunnerActivity
    {
        public static MainActivity Current;

        protected override void OnCreate(Bundle bundle)
        {
#if DEBUG
            //StrictMode.EnableDefaults();
#endif

            Current = this;

            AddTestAssembly(typeof(MainActivity).Assembly);

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);

            TestShaders.ShaderProvider = new AssetsShaderProvider(Assets);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        }
    }
}
