using RunProcessAsTask;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AD.CAAPSTaskWrapper
{
    public static class AppRun
    {
        public static async Task<ProgramOutput> ExecuteProgramAsync(string cmdLine, string cmdLineParams, int timeoutMS)
        {
            ProgramOutput Output = new ProgramOutput();
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMS);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = cmdLine,
                Arguments = cmdLineParams,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource(timeout);
                var processResults = await ProcessEx.RunAsync(processStartInfo, cancellationTokenSource.Token).ConfigureAwait(false);
                Output.ExitCode = processResults.ExitCode;
                Output.StdErr = string.Join(Environment.NewLine, processResults.StandardError);
                Output.StdOut = string.Join(Environment.NewLine, processResults.StandardOutput);
            }
            catch (OperationCanceledException)
            {
                Output.ExitCode = -1;
                Output.StdErr = string.Format("The configured timeout of {0}ms was reached while trying to run \"{1}\" {2}.\r\nThe program was terminated. Refer to the program log file for details and increase the timeout if necessary.", timeoutMS, cmdLine, cmdLineParams);
            }
            return Output;
        }
    }
}
