using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSPro.CLArgs;
using NLog.Extensions.Logging;



namespace CleanSolution;

public class CLArgsService
{
    private const string SEARCH_PATTERN = "CleanSolution.Command*.dll";
    private readonly IHostEnvironment _env;

    private readonly ILogger<CLArgsService> _logger;



    public CLArgsService(ILogger<CLArgsService> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }



    public Task ExecuteAsync()
    {
        _logger.LogInformation(AssemblyInfo.ToString());
        _logger.LogDebug("Content Root={_envContentRootPath}", _env.ContentRootPath);

        var builder = CommandBuilder.Create(); // = Commander.ResolveCommands(new AssemblyCommandResolver(assemblyFileNames));
        //  builder.Configure(settings => settings.IgnoreCase = true);
        builder.ConfigureCommands(commands =>
        {
            commands.AddAssemblies(new AssemblyCommandResolver2(Directory.GetFiles(
                _env.ContentRootPath, SEARCH_PATTERN, SearchOption.AllDirectories)));
        });
        //builder.ConfigureCommandlineArguments((arguments, settings) =>
        //  arguments.AddArguments(Environment.GetCommandLineArgs(), settings);
        //);

        builder.ConfigureServices((services, settings) =>
            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog("nlog.Commands.config");
            }));
        Commander2 commander = builder.Build();
        commander.Execute();
        return Task.CompletedTask;
    }
}