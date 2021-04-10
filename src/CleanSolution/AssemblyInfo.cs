using System;
using System.Diagnostics;
using System.Reflection;



namespace CleanSolution
{
    internal static class AssemblyInfo
    {
        public new static string ToString()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            Debug.Assert(assemblyName.Version != null, "assemblyName.Version != null");
            var buildDate = new DateTime(2000, 1, 1)
                           .AddDays(assemblyName.Version.Build).AddSeconds(assemblyName.Version.Revision * 2);
            return
                $"{assemblyName.Name} v{assemblyName.Version} Isx64={Environment.Is64BitProcess} Build={BUILD}, {buildDate.ToUniversalTime()}";
        }



#if DEBUG
        private const string BUILD = "DEBUG";
        public const string ENVIRONMENT_NAME = "Development";
#else
        private const string BUILD = "RELEASE";
        public const string ENVIRONMENT_NAME = "Production";
#endif
    }
}