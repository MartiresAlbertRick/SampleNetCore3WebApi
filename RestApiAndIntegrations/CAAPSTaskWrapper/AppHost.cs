using CommandLine;
using NLog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AD.CAAPS.Common.CommandLine;
using Microsoft.Extensions.Configuration;

namespace AD.CAAPSTaskWrapper
{
    public class AppHost
    {
        private IConfiguration Configuration { get; }
        private ILogger Logger { get; }
        private IMailService MailService { get; }

        public AppHost(IConfiguration configuration, IMailService mailService, ILogger logger)
        {
            this.Configuration = configuration;
            this.MailService = mailService;
            this.Logger = logger;
        }

        public async Task<int> Run(string[] args)
        {
            ProgramOutput Output;
            DateTime StartTime = DateTime.Now;
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                using var parser = CommandLineUtils.GetParser();
                Output = await parser.ParseArguments<RunOptions>(args)
                    .MapResult(
                        async (RunOptions options) =>
                        {
                            Logger.Debug($"Running: \"{options.CmdLine}\"");
                            Logger.Debug($"Params: \"{options.Params}\"");
                            try
                            {
                                // default time out is 5 minutes
                                int timeoutMS = options.TimeoutMS;
                                if (timeoutMS == 0)
                                {
                                    timeoutMS = Configuration.GetValue<int>("AppSettings:TimeoutMS");
                                    if (timeoutMS == 0)
                                    {
                                        timeoutMS = 5 * 60 * 1000;
                                    }
                                }
                                Logger.Debug($"TimeoutMS: \"{timeoutMS}\"");
                                if (options.Verbose)
                                {
                                    Console.WriteLine($"(wrapper) Executed with options: CmdLine: \"{options.CmdLine}\". Params: \"{options.Params}\". TimeoutMS: {timeoutMS:N0}");
                                }
                                Output = await AppRun.ExecuteProgramAsync(options.CmdLine, options.Params, timeoutMS).ConfigureAwait(false);
                                Logger.Debug($"Done. ExitCode: {Output.ExitCode}");
                            }
#pragma warning disable CA1031 // Do not catch general exception types - We are catching and logging all exceptions here.
                            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                            {
                                Logger.Error(ex, $"Failed. ExitCode: {ex.Message}");
                                Output = new ProgramOutput()
                                {
                                    ExitCode = -1,
                                    StdOut = null,
                                    StdErr = ex.ToString(),
                                };
                            }
                            Logger.Trace($"Sending email notification");
                            if (!await MailService.SendEmailReport(options.CmdLine, options.Params, StartTime, Output.ExitCode, Output.StdOut, Output.StdErr, options.Verbose))
                            {
                                Logger.Error("Program failed to run or failed to send email notifications.");
                            }
                            return Output;
                        },
                        async (errs) =>
                        {
                            Console.Error.WriteLine($"ERROR(S): Could not parse command-line parameters.");
                            foreach (Error err in errs)
                            {
                                Console.Error.WriteLine(err);
                            }
                            return await Task.FromResult(
                                new ProgramOutput()
                                {
                                    ExitCode = -1,
                                    StdOut = null,
                                    StdErr = null,
                                }
                            ).ConfigureAwait(false);
                        }
                   ).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types - We are catching and logging all exceptions here.
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Logger.Error(ex, $"Failed to run task: {ex.Message}");
                Console.Error.WriteLine($"Failed to run task: {ex.Message}");
                Console.Error.WriteLine(ex);
                Output = new ProgramOutput()
                {
                    ExitCode = -1,
                    StdOut = null,
                    StdErr = ex.ToString(),
                };
            }
            if (Output.ExitCode != 0) {
                Console.Error.WriteLine($"Failed to run task. ExitCode: {Output.ExitCode}");
                if (!string.IsNullOrWhiteSpace(Output.StdErr))
                {
                    Console.Error.WriteLine(Output.StdErr);
                }
            }
            return Output.ExitCode;
        }
    }
    }
