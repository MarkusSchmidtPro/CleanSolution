using CleanSolution.Services;
using Microsoft.Extensions.DependencyInjection;



public static class ServicesExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IPrinter, ConsolePrinter>();
    }
}