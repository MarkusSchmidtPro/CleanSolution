using MSPro.CLArgs;



namespace CleanSolution.ObsoleteFiles;

/// <summary>
///     Represent the command's context.
/// </summary>
/// <remarks>
///     A <i>CommandContext</i> is populated based on the command line
///     argument during run-time. <i>CLArgs</i> does the work, like magic, to turn
///     command-line arguments into a context.
/// </remarks>
public class CommandContext
{
    [OptionDescriptor('t', "TargetDir", true,
        helpText: "The directory where files should be deleted.")]
    public string TargetDir { get; set; }

    [OptionDescriptor('s', "SourceDir", false, ".",
        "The directory where files are scanned if the contain link to a target file.")]
    public string SourceDir { get; set; }

    [OptionDescriptor('r', "Recurse", false, true,
        "Scan source directory recursively.")]
    public bool Recurse { get; set; }

    //[OptionDescriptor("Pattern", new[] {"p"},
    //                  // Multiple Patterns will be resolved into IncludePatterns
    //                  // CLArgs does the magic job of splitting arguments. Your code simply uses
    //                  // the 'multiple property' (IncludePatterns), only.
    //                  AllowMultiple = nameof(IncludePatterns),     
    //                  // Allow to specify multiple values in o separated list, like "obj;bin"
    //                  AllowMultipleSplit = ",;",
    //                  Default = "obj;bin;build;gen;*.user",
    //                  Required = false,
    //                  HelpText =
    //                      "Specify one or more (; separated) patterns for files or directories that will be deleted."+
    //                      "|For example: *\\obj or *.user" +
    //                      "|With CLArgs support for config files (collection of command-line parameters) you can"+
    //                      " create your custom clean-profiles for different situations. One example profile is included: @sln.profile.")]
    //public string IncludePattern { get; set; }


    [OptionDescriptor("Test", "t", Default = true,
        HelpText =
            "Specify /t=false if you want to delete files and directories." +
            " Otherwise, by default, the deletions are printed to Console, only, but not performed.")]
    public bool Test { get; set; }
}