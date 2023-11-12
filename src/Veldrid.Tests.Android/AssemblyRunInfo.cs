using System.Collections.Generic;
using System.Reflection;
using Veldrid.Tests.Android.ViewModels;
using Xunit;

namespace Veldrid.Tests.Android
{
    public record AssemblyRunInfo(Assembly Assembly, TestAssemblyConfiguration Configuration, List<TestCaseViewModel> TestCases);
}
