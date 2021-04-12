using System;
using System.IO;
using MSPro.CLArgs;
using NLog;
using NLog.Config;



namespace CleanSolution
{
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
        private const string SEARCH_PATTERN = "CleanSolution.Command*.dll";
        private static ILogger ConsoleLog => LogManager.GetLogger("Console");
        private static ILogger Log => LogManager.GetLogger("Program");



        private static int Main(string[] args)
        {
#if DEBUG
            string configFileName = "nlog.debug.config";
#else
            string configFileName = "nlog.release.config";
#endif
            string fileName = Helper.FindFile(configFileName, Environment.CurrentDirectory, Helper.BinDir);
            LogManager.Configuration = new XmlLoggingConfiguration(fileName);
            Log.Debug($"Bin Dir={Helper.BinDir}");

            AppReturnCode appResult;
            ConsoleLog.Info(AssemblyInfo.ToString());
            try
            {
                // The directory of the AppDomain - does not work for self-contained executables
                var assemblyFileNames = Directory.GetFiles(
                    AppDomain.CurrentDomain.BaseDirectory, SEARCH_PATTERN,
                    SearchOption.AllDirectories);

                Commander.ExecuteCommand(args, new Settings
                {
                    CommandResolver = new AssemblyCommandResolver(assemblyFileNames),
                    IgnoreCase = true,
                    OptionValueTags = new[] {'=', ':'} // <<< support, targets to Context
                });

                appResult = AppReturnCode.Success;
            }



            #region Exception Handling

            catch (AggregateException ex)
            {
                Log.Error(ex);
                appResult = AppReturnCode.AppException;
                ConsoleLog.Error<Exception>("Unexpected termination!", ex);
                foreach (Exception innerException in ex.InnerExceptions)
                {
                    //Console.WriteLine(innerException.Message);
                    ConsoleLog.Warn(innerException.Message);
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
                ConsoleLog.Error(ex.Message);
                appResult = AppReturnCode.AppException;
            }

            #endregion



#if DEBUG
            Console.WriteLine();
            Console.WriteLine(@"Press 'eniki'...");
            Console.ReadKey();
#endif
            return Convert.ToInt32(appResult);
        }
    }



    internal enum AppReturnCode
    {
        Success = 0,
        AppException = -1
    }
}