using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Veldrid.Tests.Android
{
    /// <summary>
    /// This class be used to do discovery of xUnit.net v2 tests, via any implementation
    /// of <see cref="IAssemblyInfo"/>, including AST-based runners like CodeRush and
    /// Resharper. Runner authors who are not using AST-based discovery are strongly
    /// encouraged to use <see cref="XunitFrontController"/> instead.
    /// </summary>
    public class Xunit2Controller : Xunit.LongLivedMarshalByRefObject, IFrontController, ITestCaseDescriptorProvider
    {
        ITestCaseDescriptorProvider? defaultTestCaseDescriptorProvider;
        readonly Lazy<ITestFrameworkDiscoverer> remoteDiscoverer;
        readonly Lazy<ITestFrameworkExecutor> remoteExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Xunit2Discoverer"/> class.
        /// </summary>
        /// <param name="assemblyInfo">The assembly to use for discovery</param>
        /// <param name="sourceInformationProvider">The source code information provider.</param>
        /// <param name="diagnosticMessageSink">The message sink which receives <see cref="IDiagnosticMessage"/> messages.</param>
        public Xunit2Controller(
            IReflectionAssemblyInfo assemblyInfo,
            ISourceInformationProvider? sourceInformationProvider = null,
            IMessageSink? diagnosticMessageSink = null)
        {
            ArgumentNullException.ThrowIfNull(assemblyInfo);

            MessageSink = diagnosticMessageSink ?? new Xunit.NullMessageSink();
            SourceInfoProvider = sourceInformationProvider ?? new NullSourceInformationProvider();

            Framework = new TestFrameworkProxy(assemblyInfo, SourceInfoProvider, MessageSink);

            remoteDiscoverer = new(() => Framework.GetDiscoverer(assemblyInfo));
            remoteExecutor = new(() => Framework.GetExecutor(assemblyInfo.Assembly.GetName()));
        }

        /// <remarks>Always false.</remarks>
        /// <inheritdoc/>
        public bool CanUseAppDomains => false;

        /// <summary>
        /// Gets the message sink used to report diagnostic messages.
        /// </summary>
        public IMessageSink MessageSink { get; }

        /// <summary>
        /// Gets the source information provider.
        /// </summary>
        public ISourceInformationProvider SourceInfoProvider { get; }

        /// <summary>
        /// Returns the test framework from the remote app domain.
        /// </summary>
        public ITestFramework Framework { get; }

        internal ITestFrameworkDiscoverer RemoteDiscoverer => remoteDiscoverer.Value;

        internal ITestFrameworkExecutor RemoteExecutor => remoteExecutor.Value;

        /// <inheritdoc/>
        public string TargetFramework => RemoteDiscoverer.TargetFramework;

        /// <inheritdoc/>
        public string TestFrameworkDisplayName => RemoteDiscoverer.TestFrameworkDisplayName;

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            if (remoteDiscoverer.IsValueCreated)
            {
                remoteDiscoverer.Value.Dispose();
            }
            if (remoteExecutor.IsValueCreated)
            {
                remoteExecutor.Value.Dispose();
            }
            Framework?.Dispose();
        }

        /// <inheritdoc/>
        public ITestCase Deserialize(string value)
        {
            return RemoteExecutor.Deserialize(value);
        }

        /// <summary>
        /// Starts the process of finding all xUnit.net v2 tests in an assembly.
        /// </summary>
        /// <inheritdoc/>
        public void Find(bool includeSourceInformation, IMessageSink messageSink, ITestFrameworkDiscoveryOptions discoveryOptions)
        {
            RemoteDiscoverer.Find(includeSourceInformation, messageSink, discoveryOptions);
        }

        /// <summary>
        /// Starts the process of finding all xUnit.net v2 tests in a class.
        /// </summary>
        /// <inheritdoc/>
        public void Find(string typeName, bool includeSourceInformation, IMessageSink messageSink, ITestFrameworkDiscoveryOptions discoveryOptions)
        {
            RemoteDiscoverer.Find(typeName, includeSourceInformation, messageSink, discoveryOptions);
        }

        /// <summary>
        /// Starts the process of running all the xUnit.net v2 tests in the assembly.
        /// </summary>
        /// <inheritdoc/>
        public void RunAll(IMessageSink messageSink, ITestFrameworkDiscoveryOptions discoveryOptions, ITestFrameworkExecutionOptions executionOptions)
        {
            RemoteExecutor.RunAll(messageSink, discoveryOptions, executionOptions);
        }

        /// <summary>
        /// Starts the process of running the selected xUnit.net v2 tests.
        /// </summary>
        /// <inheritdoc/>
        public void RunTests(IEnumerable<ITestCase> testCases, IMessageSink messageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            RemoteExecutor.RunTests(testCases, messageSink, executionOptions);
        }

        /// <inheritdoc/>
        public List<TestCaseDescriptor> GetTestCaseDescriptors(List<ITestCase> testCases, bool includeSerialization)
        {
            defaultTestCaseDescriptorProvider ??= new DefaultTestCaseDescriptorProvider(RemoteDiscoverer);

            return defaultTestCaseDescriptorProvider.GetTestCaseDescriptors(testCases, includeSerialization);
        }

        /// <inheritdoc/>
        public string Serialize(ITestCase testCase)
        {
            return RemoteDiscoverer.Serialize(testCase);
        }
    }
}
