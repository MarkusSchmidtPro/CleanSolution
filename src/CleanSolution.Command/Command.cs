using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanSolution.Command.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSPro.CLArgs;



namespace CleanSolution.Command
{
    // '|' represents a line-break when displaying help.



    [Command(COMMAND_NAME,
        HelpText =
            "Clean a directory" +
            "|by deleting all files that match a given pattern." +
            "|You can perfectly use this command to clean a Visual Studio Solution directory before you ship it." +
            " Delete, for example, all .git, .vs folders or *.user files.")]
    public class Command : CommandBase2<CommandContext>
    {
        private const string COMMAND_NAME = "CleanSolution";
        private readonly ILogger<Command> _logger;
        private string _root;



        public Command(IServiceProvider serviceProvider, ILogger<Command> logger) : base(serviceProvider)
        {
            _logger = logger;
        }



        protected override void Execute()
        {
            _logger.LogInformation($"Command '{COMMAND_NAME}'");
            _logger.LogInformation($"Pattern: '{string.Join(';', this.Context.IncludePattern)}'");
            _logger.LogInformation($"Ignore : '{string.Join(';', this.Context.ExcludePatterns)}'");
            _logger.LogInformation($"Test   : '{this.Context.Test}'");

            _root = this.Context.Directories.First();
            _logger.LogInformation($"Root   : {_root}");

            ParseDirectory directoryParser = new(_root, this.Context.IncludePatterns, this.Context.ExcludePatterns,
                deleteDirectory, excludeDirectory, deleteFile, _logger
            );

            directoryParser.Execute();
            _logger.LogInformation("--- DONE! ---");
        }



        private void deleteFile(string fileRelativePath)
        {
            _logger.LogInformation($"DEL {fileRelativePath}");
            if (!this.Context.Test) { File.Delete(getFullPath(fileRelativePath)); }
        }



        private void excludeDirectory(string dirRelativePath) 
            => _logger.LogInformation($"EX: {dirRelativePath}");



        private void deleteDirectory(string dirRelativePath)
        {
            _logger.LogInformation($"RD: {dirRelativePath}");
            if (!this.Context.Test) { Directory.Delete(getFullPath(dirRelativePath), true); }
        }



        private string getFullPath(string relativePath) 
            => Path.Combine(_root, !relativePath.StartsWith("\\") ? relativePath : relativePath.Substring(1));



        /// <summary>
        ///     Check command-line arguments - passed to the Command as a typed Context - before
        ///     the Command itself is actually executed.
        /// </summary>
        /// <remarks>
        ///     Add errors to the <paramref name="errors"></paramref> list, instead of throwing exceptions.
        ///     Errors can be collected and many errors can be reported to the user. CLArgs will takes care
        ///     of reporting errors. Simply collect them.
        /// </remarks>
        protected override void BeforeExecute(
            HashSet<string> unresolvedPropertyNames,
            ErrorDetailList errors)
        {
            if (this.Context.Directories.Count == 0)
            {
                _logger.LogInformation($"No target directory specified, using '{Environment.CurrentDirectory}'");
                this.Context.Directories.Add(Environment.CurrentDirectory);
            }

            string firstTarget = this.Context.Directories.First();
            if (!Directory.Exists(firstTarget))
                errors.AddError(nameof(this.Context.Directories), $"Specified target directory '{firstTarget}' not found!");
        }
    }
}