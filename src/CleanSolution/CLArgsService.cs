using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSPro.CLArgs;

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
        _logger.LogDebug($"Content Root={_env.ContentRootPath}");
        string[] assemblyFileNames = Directory.GetFiles(
            _env.ContentRootPath, SEARCH_PATTERN, SearchOption.AllDirectories);


        var builder = CommandBuilder.Create(); // = Commander.ResolveCommands(new AssemblyCommandResolver(assemblyFileNames));
        builder.Configure(settings =>
        {
            settings.IgnoreCase = true;
        });
        builder.ConfigureCommands(commands =>
        {
            commands.AddDescriptor(new CommandDescriptor2("VERB1", typeof(MyCommand)));
            commands.AddAssemblies(new AssemblyCommandResolver2(assemblyFileNames));
        });
        builder.ConfigureCommandlineArguments((arguments, settings) =>
        {
            arguments.AddArguments(Environment.GetCommandLineArgs(), settings);
        });
        Commander2 commander = builder.Build();
        commander.ExecuteCommand();
        return Task.CompletedTask;
    }
}

public class MyCommand : ICommand2
{
    public MyCommand(ICommandlineArgumentCollection clArgs)
    {
        
    }

    public void Execute()
    {
    }
}