using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanSolution.Command.Services;
using MSPro.CLArgs;
using NLog;



namespace CleanSolution.Command
{
    // '|' represents a line-break when displaying help.



    [Command(COMMAND_NAME,
        HelpText =
            "Clean a directory" +
            "|by deleting all files that match a given pattern." +
            "| " +
            "|You can perfectly use this command to clean a Visual Studio Solution directory before you ship it." +
            "|Delete, for example, all .git, .vs folders or *.user files.")]
    internal class Command : CommandBase<CommandContext>
    {
        private const string COMMAND_NAME = "CleanSolution";
        private CommandContext _context;
        private string _root;
        private static ILogger Log => LogManager.GetLogger("CleanSolution");



        protected override void Execute(CommandContext context)
        {
            _context = context;
            Log.Info($"Ccommand '{COMMAND_NAME}'");
            Log.Info($"Pattern: '{string.Join(';', context.IncludePattern)}'");
            Log.Info($"Ignore : '{string.Join(';', context.ExcludePatterns)}'");
            Log.Info($"Test   : '{context.Test}'");

            _root = context.Directories.First();
            Log.Info($"Root   : {_root}");

            ParseDirectory directoryParser = new( _root,
                context.IncludePatterns, context.ExcludePatterns,
                deleteDirectory, excludeDirectory, deleteFile
            );

            directoryParser.Execute();
            Log.Info("--- DONE! ---");
        }



        private void deleteFile(string fileRelativePath)
        {
            Log.Info($"DEL {fileRelativePath}");
            if (!_context.Test) { File.Delete(getFullPath(fileRelativePath)); }
        }



        private void excludeDirectory(string dirRelativePath)
        {
            Log.Info($"EX: {dirRelativePath}");
        }



        private void deleteDirectory(string dirRelativePath)
        {
            Log.Info($"RD: {dirRelativePath}");
            if (!_context.Test) { Directory.Delete(getFullPath(dirRelativePath)); }
        }



        private string getFullPath(string relativePath) => Path.Combine(_root, relativePath);



        protected override void BeforeExecute(
            CommandContext context,
            HashSet<string> unresolvedPropertyNames,
            ErrorDetailList errors)
        {
            if (context.Directories.Count == 0)
            {
                // "No target directory specified");
                context.Directories.Add(Environment.CurrentDirectory);
            }

            string firstTarget = context.Directories.First();
            if (!Directory.Exists(firstTarget))
                errors.AddError(nameof(context.Directories), $"Specified target directory {firstTarget} not found!");
        }
    }
}