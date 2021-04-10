using System.Collections.Generic;
using MSPro.CLArgs;



namespace CleanSolution.Command
{
    /// <summary>
    /// Represent the command's context.
    /// </summary>
    /// <remarks>
    /// A <i>CommandContext</i> is populated based on the command line
    /// argument during run-time. <i>CLArgs</i> does the work, like magic, to turn
    /// command-line arguments into a context.
    /// </remarks>
    internal class CommandContext
    {
        [OptionDescriptor("Pattern", new[] {"p"},
            // Multiple Patterns will be resolved into IncludePatterns
            // CLArgs does the magic job of splitting arguments. Your code simply uses
            // the 'multiple property' (IncludePatterns), only.
            AllowMultiple = nameof(IncludePatterns),     
            // Allow to specify multiple values in o separated list, like "obj;bin"
            AllowMultipleSplit = ",;",
            Default = "obj;bin;build;gen;*.user",
            Required = false,
            HelpText =
                "Specify one or more 'Glob' patterns"+
                "|for files or directories that will be deleted."+
                "|See: microsoft.extensions.filesystemglobbing.matcher " +
                "|for more information about patterns.")]
        // ReSharper disable once UnusedMember.Global
        public string IncludePattern { get; set; }


        /// <summary>
        /// A list of specified patterns for deletion.
        /// </summary>
        public List<string> IncludePatterns { get; set; } = new List<string> ();


        [OptionDescriptor("ExcludePattern", new[] {"e"},
            AllowMultiple = nameof(ExcludePatterns),     
            // Allow to specify multiple values in o separated list, like "obj;bin"
            AllowMultipleSplit = ",;",
            Default = ".git;.vs",
            Required = false,
            HelpText =
                "Specify one or more 'Glob' patterns"+
                "|for files or directories that will be excluded from deletion.")]
        // ReSharper disable once UnusedMember.Global
        public string ExcludePattern { get; set; }


        public List<string> ExcludePatterns { get; set; } = new List<string> ();
    }
}