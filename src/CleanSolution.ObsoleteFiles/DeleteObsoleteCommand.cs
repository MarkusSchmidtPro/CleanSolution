using System.Diagnostics;
using CleanSolution.Services;
using MSPro.CLArgs;



namespace CleanSolution.ObsoleteFiles;

// '|' represents a line-break when displaying help.
[Command(COMMAND_NAME,
    HelpText = "Delete files from /TaRgetDir which are not referenced from any file in /SourceDir.")]
public class DeleteObsoleteCommand(IServiceProvider serviceProvider, IPrinter print)
    : CommandBase2<CommandContext>(serviceProvider)
{
    private const string COMMAND_NAME = "DeleteObsolete";
    private string _root = null!;



    protected override void BeforeExecute(ErrorDetailList errors)
    {
        base.BeforeExecute(errors);
    }



    protected override void Execute()
    {
        print.WriteLine($"Command '{COMMAND_NAME}'");
        print.WriteLine($"Test   : \'{_context.Test}\'");

        _root = Environment.CurrentDirectory;

        string targetDirPath = Path.GetFullPath(Path.Combine(_root, _context.TargetDir));
        if (!Directory.Exists(targetDirPath))
            throw new DirectoryNotFoundException($"TargetDir {_context.TargetDir} does not exist!");

        string sourceDirPath = Path.GetFullPath(Path.Combine(_root, _context.SourceDir));
        if (!Directory.Exists(sourceDirPath))
            throw new DirectoryNotFoundException($"SourceDir {_context.SourceDir} does not exist!");

        string deletedDirPath = Path.GetFullPath(Path.Combine(targetDirPath, "_Deleted"));
        deletedDirPath = Path.GetFullPath(Path.Combine(deletedDirPath, Guid.NewGuid().ToString()));
        print.WriteLine($"Deleted to: \'{getRelPath(deletedDirPath, _root)}\'");


        List<FileInfo> targets = new DirectoryInfo(_context.TargetDir).EnumerateFiles().ToList();
        print.WriteLine($"{targets.Count()} files in Target directory, which can be deleted.");

        List<FileInfo> contentFiles = new DirectoryInfo(_context.SourceDir).EnumerateFiles("*.md", SearchOption.AllDirectories).ToList();
        print.WriteLine($"{contentFiles.Count()} files in Source directory, which will be checked if they reference a target file..");

        foreach (FileInfo s in contentFiles)
        {
            string content = File.ReadAllText(s.FullName);

            // Check if any target name appears in source file
            foreach (FileInfo t in targets.ToArray())
            {
                if (content.Contains(t.Name))
                {
                    targets.Remove(t);
                }
            }
        }

        print.WriteLine($"{targets.Count()} target files are subject for deletion.");
        foreach (FileInfo t in targets)
        {
            //print.WriteLine($"File not used: {s.Name}");
            moveFile(t.FullName, deletedDirPath);
        }
        //directoryParser.Execute();
        print.WriteLine("--- DONE! ---");
    }



    private void moveFile(string filePath, string targetDirectory)
    {
        string newFilePath = Path.Combine(targetDirectory, Path.GetFileName(filePath));
        print.WriteLine($"Move {getRelPath(filePath, _root)}");

        if (!_context.Test)
        {
            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

            if ((FileAttributes.ReadOnly & File.GetAttributes(filePath)) != 0)
            {
                // Remove read-only attribute before deletion
                File.SetAttributes(filePath, FileAttributes.Normal);
            }

            File.Move(filePath, newFilePath, true);
        }
    }



    private void renameFile(string filePath)
    {
        string newName = Path.Combine(
            Path.GetDirectoryName(filePath)!,
            "_" + Path.GetFileName(filePath));

        string newNameRel = getRelPath(newName, Path.GetDirectoryName(filePath));
        string fileNameRel = getRelPath(filePath, _root);
        print.WriteLine($"REN {fileNameRel} -> {newNameRel}");

        if (!_context.Test)
        {
            if ((FileAttributes.ReadOnly & File.GetAttributes(filePath)) != 0)
            {
                // Remove read-only attribute before deletion
                File.SetAttributes(filePath, FileAttributes.Normal);
            }

            File.Move(filePath, newName, true);
        }
    }



    private static string getRelPath(string targetPath, string sourcePath)
    {
        Debug.Assert(Path.IsPathRooted(targetPath));
        // Result are a path but not readable, like:File ..\..\..\..\.. has not changed|
        // return Path.GetRelativePath(itemPath, basePath);
        string rel = targetPath.Replace(sourcePath, string.Empty, StringComparison.CurrentCultureIgnoreCase);
        return rel.StartsWith("\\") ? rel[1..] : rel;
    }
}