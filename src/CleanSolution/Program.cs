﻿using System;
using System.IO;
using System.Reflection;
using MSPro.CLArgs;
using NLog;
using NLog.Config;



namespace CleanSolution
{
    internal class Program
    {
        private const string SEARCH_PATTERN = "CleanSolution.Command*.dll";
        private static ILogger ConsoleLog => LogManager.GetLogger("Console");
        private static ILogger Log => LogManager.GetLogger("Program");



        private static string resolveFileName(string fileName)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            string l = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            return File.Exists(filePath) ? filePath : Path.Combine(l!, fileName);
        }



        private static int Main(string[] args)
        {
#if DEBUG
            string configFileName = "nlog.debug.config";
#else
            string configFileName = "nlog.release.config";
#endif
            string fileName = resolveFileName(configFileName);
            LogManager.Configuration = new XmlLoggingConfiguration(fileName);

            AppReturnCode appResult;
            ConsoleLog.Info(AssemblyInfo.ToString());
            try
            {
                var assemblyFileNames = Directory.GetFiles(
                    AppDomain.CurrentDomain.BaseDirectory, SEARCH_PATTERN,
                    SearchOption.AllDirectories);
                Commander.ExecuteCommand(args, new Settings
                {
                    CommandResolver = new AssemblyCommandResolver(assemblyFileNames),
                    IgnoreCase      = true
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