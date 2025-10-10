using System;
using System.Diagnostics;
using CleanSolution;
using Microsoft.Extensions.Hosting;
using NLog;



public class Program
{
    public static void Main(string[] args)
    {
#if DEBUG
        string environmentName = Environments.Development;
#else
        string environmentName = Environments.Production;
#endif

//
// Here we need a NLog logger because logging
// is not yet initialized neither is it registered.
//
        LogManager.Setup().LoadConfigurationFromFile($"nLog.{environmentName}.config");
        Logger nLogger = LogManager.GetLogger("Console.Host");
        nLogger.Info($"*** {AssemblyInfo.ToString()} Start ***");

        var sw = Stopwatch.StartNew();

        try
        {
            IHost host = StartUp.BuildHost(environmentName, args);
            host.Start(); // IHost => CommandHost
        }
        catch (Exception ex)
        {
            // Only the Exception message is logged, by default.
            // If the logger defines an Exception layout (nlog.config),
            // also the StackTrace etc. can be logged, 
            // which is the default for the file loggers.
            nLogger.Error(ex);
        }

        nLogger.Info($"*** App Stop (total time {sw.Elapsed:mm\\:ss\\.fff}) ***");
        LogManager.Shutdown();


#if DEBUG
        Console.WriteLine();
        Console.WriteLine("Press 'eniki'...");
        Console.ReadKey();
#endif
    }
}