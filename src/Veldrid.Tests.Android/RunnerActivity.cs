using System;
using System.Collections.Generic;
using System.Reflection;
using Android.OS;
using Veldrid.Tests.Android.Forms;
using Veldrid.Tests.Android.Sinks;
using Veldrid.Tests.Android.Utilities;
using Xamarin.Forms.Platform.Android;

namespace Veldrid.Tests.Android
{
    public abstract class RunnerActivity : FormsApplicationActivity
    {
        readonly List<Assembly> testAssemblies = new();

        FormsRunner? runner;

        protected bool Initialized { get; private set; }

        protected IResultChannel? ResultChannel { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            PlatformHelpers.Assets = Assets;

            Xamarin.Forms.Forms.Init(this, bundle, typeof(RunnerActivity).Assembly);

            runner = new FormsRunner(testAssemblies, ResultChannel ?? new NullResultChannel());

            Initialized = true;

            LoadApplication(runner);
        }

        protected void AddTestAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            if (!Initialized)
            {
                testAssemblies.Add(assembly);
            }
        }
    }
}
