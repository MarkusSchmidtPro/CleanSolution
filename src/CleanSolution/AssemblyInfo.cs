using System;
using System.Diagnostics;
using System.Reflection;



namespace CleanSolution;







internal static class AssemblyInfo
{
    public new static string ToString()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        Debug.Assert(assemblyName.Version != null, "assemblyName.Version != null");
        var buildDate = new DateTime(2000, 1, 1)
                       .AddDays(assemblyName.Version.Build).AddSeconds(assemblyName.Version.Revision * 2);
        return
            $"{assemblyName.Name} v{assemblyName.Version} Isx64={Environment.Is64BitProcess}, {buildDate.ToUniversalTime()}";
    }
    
#if DEBUG
    public const string EnvironmentName = "Development";
#else
    public const string EnvironmentName = "Production";
#endif
}