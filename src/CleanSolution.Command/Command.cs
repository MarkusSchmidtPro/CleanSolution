using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanSolution.Command.Services;
using MSPro.CLArgs;
using NLog;



namespace CleanSolution.Command
{
    [Command(COMMAND_NAME,
        HelpText =
            "Clean a directory by deleting all files that match a given pattern." +
            "|You can perfectly use this command" +
            "|to clean a Visual Studio Solution directory before you ship it." +
            "|Delete, for example, all .git, .vs folders or *.user files.")]
    internal class Command : CommandBase<CommandContext>
    {
        private const string COMMAND_NAME = "CleanSolution";
        private static ILogger Log => LogManager.GetLogger("CleanSolution");
        private string _root;


        protected override void Execute(CommandContext context)
        {
            
            Log.Info($"Executing command '{COMMAND_NAME}'");
            Log.Info($"DIR: {_root}");

            CleanFolder cf = new (_root, context.IncludePatterns, context.ExcludePatterns);
            cf.Execute();
            Log.Info("--- DONE! ---");
        }



        protected override void BeforeArgumentConversion(CommandLineArguments commandLineArguments)
        {
            if (commandLineArguments.Targets == null || commandLineArguments.Targets.Count == 0)
                throw new ArgumentException("No target provided.");

            _root = commandLineArguments.Targets.First();
            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"Directory {_root} not found!");
        }

        protected override void BeforeExecute(
            CommandContext context,
            HashSet<string> unresolvedPropertyNames,
            ErrorDetailList errors)
        {
        }
    }
}