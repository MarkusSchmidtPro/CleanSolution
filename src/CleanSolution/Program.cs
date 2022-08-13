using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSPro.CLArgs;
using NLog.Extensions.Logging;



const string SEARCH_PATTERN = "CleanSolution.Command*.dll";

var builder = CommandBuilder.Create();
builder.ConfigureCommands(commands =>
{
    commands.AddAssemblies(new AssemblyCommandResolver2(
                               Directory.GetFiles(Environment.CurrentDirectory, SEARCH_PATTERN, SearchOption.AllDirectories)));
});
builder.ConfigureServices((services, settings) =>
{
    services.AddLogging(loggingBuilder =>
    {
        // By default CLArgs uses .NET Console logger for logging. You may override this and
        // provide your own logging provider, e.g. NLog
        loggingBuilder.ClearProviders();
        loggingBuilder.AddNLog();
    });
});
Commander2 commander = builder.Build();
commander.Execute();