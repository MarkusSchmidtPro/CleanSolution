using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using ILogger = NLog.ILogger;

namespace CleanSolution;

/// <summary>
///     The program as a console Application acts as a host, only.
/// </summary>
/// <remarks>
///     It takes the command line arguments, scans for available Command
///     implementations in the application's directory (see SEARCH_PATTERN).
///     Finally, it runs the command as specified in the command line (ref Verb).
/// </remarks>
internal class Program
{
    public static async Task MainX(string[] args)
    {
#if DEBUG
        string environment = "Development";
#else
        string environment = "Production";
#endif
        var nLogApplicationConfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .Build();


        // Provides default configuration for the app
        // https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
        IHostBuilder builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddTransient<CLArgsService>();
                services.AddLogging(loggingBuilder =>
                {
                    // configure Logging with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddNLog(nLogApplicationConfig);
                });
            })
        .UseConsoleLifetime();

        ILogger logger = LogManager.GetCurrentClassLogger();

        try
        {
            var host = builder.Build();
            logger.Info("App Starts..");
            using var serviceScope = host.Services.CreateScope();
            {
                var services = serviceScope.ServiceProvider;
                var myService = services.GetRequiredService<CLArgsService>();
                await myService.ExecuteAsync();
            }
        }

        #region Exception Handling

        catch (AggregateException ex)
        {
            logger.Error(ex);
            logger.Error<Exception>("Unexpected termination!", ex);
            foreach (var innerException in ex.InnerExceptions) logger.Warn(innerException.Message);
        }

        catch (ApplicationException ex)
        {
            logger.Error(ex.Message);
        }  
        catch (Exception ex)
        {
            logger.Error(ex);
            logger.Error(ex.Message);
        }
        finally
        {
            LogManager.Shutdown();
        }

        #endregion


#if DEBUG
        Console.WriteLine();
        Console.WriteLine(@"Press 'eniki'...");
        Console.ReadKey();
#endif
    }
}