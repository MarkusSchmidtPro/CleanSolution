using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSPro.CLArgs;
using NLog.Extensions.Logging;





#if DEBUG
string environment = "Development";
#else
string environment = "Production";
#endif

// Set the Assembly filename pattern to define which Assemblies may contain [ICommand2] implementations.
// If not specified, only the GetExecutingAssembly() is used to resolve commands.
const string SEARCH_PATTERN = "CleanSolution.Command*.dll";

var builder = CommandBuilder.Create();
builder.ConfigureCommands(commands =>
{
    commands.AddAssembly(Assembly.GetExecutingAssembly());
    commands.AddAssemblies( 
        Directory.GetFiles(Environment.CurrentDirectory, SEARCH_PATTERN, SearchOption.AllDirectories));
});
builder.ConfigureServices((services, _) =>
{
    services.AddLogging(loggingBuilder =>
    {
        // By default CLArgs uses .NET Console logger for logging.
        // You may override this and provide your own logging provider, e.g. NLog
        loggingBuilder.ClearProviders();
        loggingBuilder.AddNLog($"nLog.{environment}.config");
    });
});
Commander2 commander = builder.Build();
commander.Execute();