using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using CleanSolution.Command;
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
            "|You can perfectly use this command to clean a Visual Studio Solution directory before you ship it." +
            " Delete, for example, all .git, .vs folders or *.user files.")]
    public class Command : CommandBase2<CommandContext>
    {
        private const string COMMAND_NAME = "CleanSolution";
        private string _root;
        private static ILogger Log => LogManager.GetLogger("CleanSolution");

        public Command(ContextBuilder contextBuilder) : base(contextBuilder)
        {
            
        }
        protected override void Execute()
        {
            Log.Info($"Ccommand '{COMMAND_NAME}'");
            Log.Info($"Pattern: '{string.Join(';', Context.IncludePattern)}'");
            Log.Info($"Ignore : '{string.Join(';', Context.ExcludePatterns)}'");
            Log.Info($"Test   : '{Context.Test}'");

            _root = Context.Directories.First();
            Log.Info($"Root   : {_root}");

            ParseDirectory directoryParser = new(_root,
                Context.IncludePatterns,
                Context.ExcludePatterns,
                deleteDirectory, excludeDirectory, deleteFile
            );

            directoryParser.Execute();
            Log.Info("--- DONE! ---");
        }



        private void deleteFile(string fileRelativePath)
        {
            Log.Info($"DEL {fileRelativePath}");
            if (!Context.Test) { File.Delete(getFullPath(fileRelativePath)); }
        }



        private void excludeDirectory(string dirRelativePath)
        {
            Log.Info($"EX: {dirRelativePath}");
        }



        private void deleteDirectory(string dirRelativePath)
        {
            Log.Info($"RD: {dirRelativePath}");
            if (!Context.Test) { Directory.Delete(getFullPath(dirRelativePath),true); }
        }



        private string getFullPath(string relativePath) => Path.Combine(_root, !relativePath.StartsWith("\\")?relativePath:relativePath.Substring(1));


        /// <summary>
        /// Check command-line arguments - passed to the Command as a typed Context - before
        /// the Command itself is actually executed.
        /// </summary>
        /// <remarks>
        /// Add errors to the <paramref name="errors"></paramref> list, instead of throwing exceptions.
        /// Errors can be collected and many errors can be reported to the user. CLArgs will takes care
        /// of reporting errors. Simply collect them.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="unresolvedPropertyNames"></param>
        /// <param name="errors"></param>
        /*protected override void BeforeExecute(
            CommandContext context,
            HashSet<string> unresolvedPropertyNames,
            ErrorDetailList errors)
        {
            if (context.Directories.Count == 0)
            {
                Log.Info($"No target directory specified, using '{Environment.CurrentDirectory}'");
                context.Directories.Add(Environment.CurrentDirectory);
            }

            string firstTarget = context.Directories.First();
            if (!Directory.Exists(firstTarget))
                errors.AddError(nameof(context.Directories), $"Specified target directory '{firstTarget}' not found!");
        }*/

     /*   public Command( ContextBuilder contextBuilder)
        {
            var errors = contextBuilder.TryConvert(typeof(CommandContext),
                out object executionContext,
                out HashSet<string> unresolvedPropertyNames);
            CommandContext context = (CommandContext)executionContext;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }*/
    }
}

public abstract class CommandBase2<TContext> : ICommand2
{
    private readonly ContextBuilder _contextBuilder;

    protected CommandBase2( ContextBuilder contextBuilder)
    {
        _contextBuilder = contextBuilder;
    }

    protected TContext Context { get; set; }

    void ICommand2.Execute()
    {
        var errors = _contextBuilder.TryConvert(typeof(TContext),
            out object executionContext,
            out HashSet<string> unresolvedPropertyNames);
        Context = (TContext)executionContext;
        this.Execute();
    }

    protected abstract void Execute();
}