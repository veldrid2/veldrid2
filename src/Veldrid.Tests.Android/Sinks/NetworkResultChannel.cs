using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using Veldrid.Tests.Android.Utilities;
using Veldrid.Tests.Android.ViewModels;

namespace Veldrid.Tests.Android.Sinks
{
    public class NetworkResultChannel : IResultChannel
    {
        private readonly string _endPoint;
        private readonly int _port;
        private readonly AsyncLock _mutex;

        private StreamWriter? _writer;
        private int _failed;
        private int _passed;
        private int _skipped;

        public NetworkResultChannel(string hostName, int port)
        {
            _endPoint = hostName ?? throw new ArgumentNullException(nameof(hostName));
            _port = port;

            _mutex = new AsyncLock();
        }

        public async ValueTask RecordResultAsync(TestResultViewModel result, CancellationToken cancellationToken)
        {
            using (await _mutex.LockAsync())
            {
                StringBuilder builder = new();
                if (result.TestCase.Result == TestState.Passed)
                {
                    builder.Append("\t[PASS] ");
                    _passed++;
                }
                else if (result.TestCase.Result == TestState.Skipped)
                {
                    builder.Append("\t[SKIPPED] ");
                    _skipped++;
                }
                else if (result.TestCase.Result == TestState.Failed)
                {
                    builder.Append("\t[FAIL] ");
                    _failed++;
                }
                else
                {
                    builder.Append("\t[INFO] ");
                }
                builder.Append(result.TestCase.DisplayName);

                string message = result.ErrorMessage;
                if (!string.IsNullOrEmpty(message))
                {
                    builder.Append($" : {message.Replace("\r\n", "\\r\\n")}");
                }
                builder.AppendLine();

                string stacktrace = result.ErrorStackTrace;
                if (!string.IsNullOrEmpty(result.ErrorStackTrace))
                {
                    string[] lines = stacktrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        builder.AppendLine($"\t\t{line}");
                    }
                }

                await WriteAsync(builder, cancellationToken);
            }
        }

        public async Task CloseChannelAsync()
        {
            using (await _mutex.LockAsync())
            {
                int total = _passed + _failed; // ignored are *not* run
                StringBuilder builder = new();
                builder.AppendLine($"Tests run: {total} Passed: {_passed} Failed: {_failed} Skipped: {_skipped}");

                await WriteAsync(builder, CancellationToken.None);

                await _writer!.DisposeAsync();
                _writer = null;
            }
        }

        public async Task<bool> OpenChannelAsync(CancellationToken cancellationToken, string? message = null)
        {
            using (await _mutex.LockAsync())
            {
                DateTime now = DateTime.Now;

                System.Diagnostics.Debug.WriteLine(
                    $"[{now}] Sending '{message}' results to {_endPoint}:{_port}");

                try
                {
                    TcpClient client = new();
                    await client.ConnectAsync(_endPoint, _port, cancellationToken);
                    _writer = new StreamWriter(client.GetStream());
                }
                catch (SocketException)
                {
                    string msg =
                        $"Cannot connect to {_endPoint}:{_port}. " +
                        $"Start network service or disable network logger option";
                    Toast.MakeText(Application.Context, msg, ToastLength.Long)?.Show();
                    return false;
                }

                StringBuilder builder = new();

                builder.AppendLine($"[Runner executing:\t{message}]");
                builder.AppendLine($"[M4A Version:\t{"???"}]");  // FIXME
                builder.AppendLine($"[Board:\t\t{Build.Board}]");
                builder.AppendLine($"[Bootloader:\t{Build.Bootloader}]");
                builder.AppendLine($"[Brand:\t\t{Build.Brand}]");
                builder.AppendLine($"[SupportedAbis:\t{string.Join(", ", Build.SupportedAbis ?? Array.Empty<string>())}]");
                builder.AppendLine($"[Supported32BitAbis:\t{string.Join(", ", Build.Supported32BitAbis ?? Array.Empty<string>())}]");
                builder.AppendLine($"[Supported64BitAbis:\t{string.Join(", ", Build.Supported64BitAbis ?? Array.Empty<string>())}]");
                builder.AppendLine($"[Device:\t{Build.Device}]");
                builder.AppendLine($"[Display:\t{Build.Display}]");
                builder.AppendLine($"[Fingerprint:\t{Build.Fingerprint}]");
                builder.AppendLine($"[Hardware:\t{Build.Hardware}]");
                builder.AppendLine($"[Host:\t\t{Build.Host}]");
                builder.AppendLine($"[Id:\t\t{Build.Id}]");
                builder.AppendLine($"[Manufacturer:\t{Build.Manufacturer}]");
                builder.AppendLine($"[Model:\t\t{Build.Model}]");
                builder.AppendLine($"[Product:\t{Build.Product}]");
                builder.AppendLine($"[RadioVersion:\t\t{Build.RadioVersion}]");
                builder.AppendLine($"[Tags:\t\t{Build.Tags}]");
                builder.AppendLine($"[Time:\t\t{Build.Time}]");
                builder.AppendLine($"[Type:\t\t{Build.Type}]");
                builder.AppendLine($"[User:\t\t{Build.User}]");
                builder.AppendLine($"[VERSION.Codename:\t{Build.VERSION.Codename}]");
                builder.AppendLine($"[VERSION.Incremental:\t{Build.VERSION.Incremental}]");
                builder.AppendLine($"[VERSION.Release:\t{Build.VERSION.Release}]");
                builder.AppendLine($"[VERSION.SdkInt:\t{Build.VERSION.SdkInt}]");
                builder.AppendLine($"[VERSION.SecurityPatch:\t{Build.VERSION.SecurityPatch}]");

                if (OperatingSystem.IsAndroidVersionAtLeast(31))
                {
                    builder.AppendLine($"[SocModel:\t\t{Build.SocModel}]");
                    builder.AppendLine($"[SocManufacturer:\t\t{Build.SocManufacturer}]");
                    builder.AppendLine($"[Sku:\t\t{Build.Sku}]");
                    builder.AppendLine($"[OdmSku:\t\t{Build.OdmSku}]");
                }

                builder.AppendLine($"[Device Date/Time:\t{now}]"); // to match earlier C.WL output

                // FIXME: add data about how the app was compiled (e.g. ARMvX, LLVM, Linker options)

                await WriteAsync(builder, cancellationToken);

                _failed = _passed = _skipped = 0;
                return true;
            }
        }

        private async ValueTask WriteAsync(StringBuilder builder, CancellationToken cancellationToken)
        {
            if (_writer == null)
            {
                throw new InvalidOperationException("Not connected.");
            }
            await _writer.WriteAsync(builder, cancellationToken);
        }
    }
}
