using System;
using System.Collections.Generic;
using MSPro.CLArgs;
using NLog;



namespace CleanSolution.Command
{
    [Command(COMMAND_NAME,
        HelpText =
            "Clean a directory by deleting all files that match a given pattern."+
            "|You can perfectly use this command"+
            "|to clean a Visual Studio Solution directory before you ship it."+
            "|Delete, for example, all .git, .vs folders or *.user files.")]
    internal class Command : CommandBase<CommandContext>
    {
        private const string COMMAND_NAME = "Fetch.Get";
        private static ILogger Log => LogManager.GetLogger("Program");

        protected override void Execute(CommandContext context)
        {
            Log.Info($"Executing command '{COMMAND_NAME}'");
            ErrorDetailList errors = new ErrorDetailList();

            Log.Info("--- DONE! ---");
        }



        protected override void BeforeExecute(
            CommandContext context,
            HashSet<string> unresolvedPropertyNames,
            ErrorDetailList errors)
        {
  
        }
    }
}