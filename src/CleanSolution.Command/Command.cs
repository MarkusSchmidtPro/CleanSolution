using System;
using System.IO;
using System.Linq;
using CleanSolution.Command.Services;
using CleanSolution.Services;
using MSPro.CLArgs;



namespace CleanSolution.Command;

// '|' represents a line-break when displaying help.
[Command(COMMAND_NAME,
    HelpText =
        "Clean a directory" +
        "|by deleting all files that match a given pattern." +
        "|You can perfectly use this command to clean a Visual Studio Solution directory before you ship it." +
        " Delete, for example, all .git, .vs folders or *.user files.")]
public class CleanSolutionCommand(IServiceProvider serviceProvider, IPrinter print)
    : CommandBase2<CommandContext>(serviceProvider)
{
    private const string COMMAND_NAME = "CleanSolution";
    private string _root;



    protected override void Execute()
    {
        print.WriteLine($"Command '{COMMAND_NAME}'");
        print.WriteLine($"Pattern: '{string.Join(';', _context.IncludePatterns)}'");
        print.WriteLine($"Ignore : \'{string.Join(';', _context.ExcludePatterns)}\'");
        print.WriteLine($"Test   : \'{_context.Test}\'");

        _root = _context.Directories.First();
        print.WriteLine($"Root   : {_root}");

        ParseDirectory directoryParser = new(_root, _context.IncludePatterns, _context.ExcludePatterns,
            deleteDirectory, excludeDirectory, deleteFile, print
        );

        directoryParser.Execute();
        print.WriteLine("--- DONE! ---");
    }



    private void deleteFile(string fileRelativePath)
    {
        print.WriteLine($"DEL {fileRelativePath}");
        if (!_context.Test)
        {
            if (_context.DeleteReadOnly && (FileAttributes.ReadOnly & File.GetAttributes(fileRelativePath)) != 0)
            {
                // Remove read-only attribute before deletion
                File.SetAttributes(fileRelativePath, FileAttributes.Normal);
                print.WriteLine($"Removing r/o attribute: {fileRelativePath}");
            }

            File.Delete(getFullPath(fileRelativePath));
        }
    }



    private void excludeDirectory(string dirRelativePath)
        => print.WriteLine($"EX: {dirRelativePath}");



    private void deleteDirectory(string dirRelativePath)
    {
        print.WriteLine($"RD: {dirRelativePath}");
        if (!_context.Test)
        {
            string fullPath = getFullPath(dirRelativePath);

            // Remove read-only attributes from all directories
            // which have read-only set!
            var directory = new DirectoryInfo(fullPath) { Attributes = FileAttributes.Normal };
            foreach (FileSystemInfo info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                string relPath =
                    Path.Combine(dirRelativePath,
                        Path.GetRelativePath(fullPath, info.FullName));
                //print.WriteLine($"Removing r/o attribute: {relPath}");
                info.Attributes = FileAttributes.Normal;
            }

            Directory.Delete(fullPath, true);
        }
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
    protected override void BeforeExecute(ErrorDetailList errors)
    {
        if (_context.Directories.Count == 0)
        {
            print.WriteLine($"No target directory specified, using '{Environment.CurrentDirectory}'");
            _context.Directories.Add(Environment.CurrentDirectory);
        }

        string firstTarget = _context.Directories.First();
        if (!Directory.Exists(firstTarget))
            errors.AddError(nameof(_context.Directories), $"Specified target directory '{firstTarget}' not found!");
    }
}