namespace Veldrid.Tests.Android
{
    public class RunnerOptions
    {
        public static readonly RunnerOptions Current = new();

        public RunnerOptions()
        {
            EnableNetwork = false;
            HostName = string.Empty;
            HostPort = 0;
        }

        public bool AutoStart { get; set; }
        public bool EnableNetwork { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }
        public bool TerminateAfterExecution { get; set; }

        public bool UseNetworkLogger => EnableNetwork && !string.IsNullOrWhiteSpace(HostName) && HostPort > 0;
    }
}
