using System;
using System.IO;
using CleanSolution.Command;
using CleanSolution.ObsoleteFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSPro.CLArgs;
using NLog.Extensions.Logging;



namespace CleanSolution;

internal class StartUp
{
    internal static IHost BuildHost(string environmentName, string[] args)
    {
        /*  CommandHostBuilder
        Tell CLArgs where to find Commands.
        A Command specifies CommandAttribute and it implements ICommand2.
        Later, when the CommandHostBuilder is started its
        [public IHost Build()] function is called.
        Then, all CommandDescriptor2 are registered as
        AddScoped(typeof(ICommand2), commandDescriptor.Type);
    */
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        //
        // Init Configuration
        //
        builder.Configuration.Sources.Clear();
        builder.Configuration
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), false, false)
#if DEBUG
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.Development.json"), true, false)
#endif
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), true, false);

        // IOptions Pattern
        // Register app settings and use as IOptions<AppSettings>
        // either by constructor injection or by 
        // var x =_host.Services.GetService <IOptions<AppSettings>>();
        //builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings")!);



        builder.Services.AddLogging(loggingBuilder =>
        {
            // By default CLArgs uses .NET Console logger for logging.
            // You may override this and provide your own logging provider, e.g. NLog
            loggingBuilder.ClearProviders();
            loggingBuilder.AddNLog();
        });
        // ILogger (with no type) must be manually registered
        builder.Services.AddScoped(typeof(ILogger), serviceProvider
            => serviceProvider.GetRequiredService<ILogger<Program>>());



        builder.ConfigureCommands(args, commands =>
        {
            commands.AddAssembly(typeof(CleanSolutionCommand).Assembly);
            commands.AddAssembly(typeof(DeleteObsoleteCommand).Assembly);
        });

        builder.Services.RegisterServices();
        return builder.Build();
    }
}