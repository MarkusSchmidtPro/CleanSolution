using System.Collections.Generic;
using MSPro.CLArgs;



namespace CleanSolution.Command;

/// <summary>
/// Represent the command's context.
/// </summary>
/// <remarks>
/// A <i>CommandContext</i> is populated based on the command line
/// argument during run-time. <i>CLArgs</i> does the work, like magic, to turn
/// command-line arguments into a context.
/// </remarks>
public class CommandContext
{
    [OptionDescriptor('r', "DeleteReadOnly", defaultValue:true, helpText:
        "Delete files that have read-only attribute set.")]
    public bool DeleteReadOnly { get; set; }


    [Targets]
    public List<string> Directories { get; set; } = new();


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
                          "Specify one or more (; separated) patterns for files or directories that will be deleted."+
                          "|For example: *\\obj or *.user" +
                          "|With CLArgs support for config files (collection of command-line parameters) you can"+
                          " create your custom clean-profiles for different situations. One example profile is included: @sln.profile.")]
    public string IncludePattern { get; set; }


    /// <summary>
    /// A list of specified patterns for deletion.
    /// </summary>
    /// <remarks>
    /// This property is not directly bound to a command-line Option.
    /// However, it is referenced from the 'Pattern' option to take multiple 'Patterns'
    /// when specified in the command line.
    /// </remarks>
    public List<string> IncludePatterns { get; set; } = new();


    [OptionDescriptor("ExcludePattern", new[] {"e"},
                      AllowMultiple = nameof(ExcludePatterns),     
                      AllowMultipleSplit = ",;",
                      Default = ".git;.vs",
                      Required = false,
                      HelpText = "Specify one or more file and/or directory patterns"+
                                 " that will be excluded from deletion.")]
    public string ExcludePattern { get; set; }

    public List<string> ExcludePatterns { get; set; } = new();


    [OptionDescriptor("Test", new[] {"t"},
                      Default = true,
                      HelpText =
                          "Specify /t=false if you want to delete files and directories." +
                          " Otherwise, by default, the deletions are printed to Console, only, but not performed.")]
    public bool Test { get; set; }
}