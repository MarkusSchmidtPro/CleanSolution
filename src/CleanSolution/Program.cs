using System;
using System.Diagnostics;
using CleanSolution;
using CleanSolution.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSPro.CLArgs;
using NLog;
using NLog.Extensions.Logging;
using ILogger = NLog.ILogger;



string nLogConfig = $"nLog.{AssemblyInfo.EnvironmentName}.config";
//
// The Host uses and registers NLog as the preferred logger.
// The Host uses its own logger to display messages on the console.
//
LogManager.LoadConfiguration(nLogConfig);
ILogger nLogger = LogManager.GetLogger("Console.Host");
nLogger.Info($"*** App Start ({AssemblyInfo.EnvironmentName}) ***");
nLogger.Info(AssemblyInfo.ToString);
var sw = Stopwatch.StartNew();

try
{
    CommandHostBuilder builder = CommandHostBuilder.Create(args);
    builder.ConfigureCommands(commands => { commands.AddAssembly(typeof(CleanSolutionCommand).Assembly); });
    builder.ConfigureServices(services =>
    {
        services.AddLogging(loggingBuilder =>
        {
            // By default CLArgs uses .NET Console logger for logging.
            // You may override this and provide your own logging provider, e.g. NLog
            loggingBuilder.ClearProviders();
            loggingBuilder.AddNLog(nLogConfig);
        });
    });
    IHost commandHost = builder.Build();
    commandHost.Start();
}catch (Exception ex)
{
    nLogger.Error(ex);
}



nLogger.Info($"*** App Stop (total time {sw.Elapsed:mm\\:ss\\.fff}) ***");
LogManager.Shutdown();


#if DEBUG
    Console.WriteLine();
    Console.WriteLine("Press 'eniki'...");
    Console.ReadKey();
#endif