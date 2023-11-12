using System;
using System.Linq;
using Android.Content.Res;

namespace Veldrid.Tests.Android.Utilities
{
    internal static class PlatformHelpers
    {
        public static void TerminateWithSuccess()
        {
            Environment.Exit(0);
        }

        public static System.IO.Stream? ReadConfigJson(string assemblyName)
        {
            string[]? assets = Assets.List(string.Empty);
            if (assets != null)
            {
                if (assets.Contains($"{assemblyName}.xunit.runner.json"))
                    return Assets.Open($"{assemblyName}.xunit.runner.json");

                if (assets.Contains("xunit.runner.json"))
                    return Assets.Open("xunit.runner.json");
            }
            return null;
        }

        public static AssetManager Assets { get; set; }
    }
}
