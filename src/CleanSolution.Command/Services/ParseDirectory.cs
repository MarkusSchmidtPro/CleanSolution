using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CleanSolution.Services;



namespace CleanSolution.Command.Services;

internal class ParseDirectory
{
    private readonly Action<string> _deleteDirectoryAction;
    private readonly Action<string> _deleteFileAction;
    private readonly IEnumerable<PatternMatch> _deletionPatterns;
    private readonly Action<string> _excludeDirectoryAction;
    private readonly IEnumerable<PatternMatch> _ignorePatterns;
    private readonly IPrinter _print;
    private readonly string _rootPath;



    public ParseDirectory(
        string rootPath,
        [NotNull] IEnumerable<string> deletionPatterns,
        [NotNull] IEnumerable<string> ignorePatterns,
        Action<string> deleteDirectoryAction,
        Action<string> excludeDirectoryAction,
        Action<string> deleteFileAction,
        IPrinter print
    )
    {
        if (!Path.IsPathRooted(rootPath)) throw new ArgumentException("Path must be rooted", nameof(rootPath));
        _rootPath = rootPath;
        _deleteFileAction = deleteFileAction;
        _print = print;
        _excludeDirectoryAction = excludeDirectoryAction;
        _deleteDirectoryAction = deleteDirectoryAction;

        _deletionPatterns = deletionPatterns.Select(p => new PatternMatch(p));
        _ignorePatterns = ignorePatterns.Select(p => new PatternMatch(p));
    }



    public void Execute() => processDirectory(_rootPath);



    private void processDirectory(string currentDirFullPath)
    {
        IEnumerable<DirectoryInfo> currentDirs = Directory.GetDirectories(currentDirFullPath).Select(d => new DirectoryInfo(d));
        foreach (DirectoryInfo di in currentDirs)
        {
            string matchCheckPath = "\\" + Path.GetRelativePath(_rootPath, di.FullName);

            // if directory is an exclude match --> skip it
            if (_ignorePatterns.Any(ip => ip.IsMatches(matchCheckPath)))
            {
                _excludeDirectoryAction(matchCheckPath);
                continue;
            }

            // process current directory
            if (_deletionPatterns.Any(dp => dp.IsMatches(matchCheckPath)))
            {
                _deleteDirectoryAction(matchCheckPath);
                continue;
            }

            processDirectory(di.FullName);
        }

        // Not excluded, not deleted -- check files
        var currentDi = new DirectoryInfo(currentDirFullPath);
        foreach (FileInfo fi in currentDi.GetFiles())
        {
            string relativeFilePath = Path.GetRelativePath(_rootPath, fi.FullName);
            if (_deletionPatterns.Any(dp => dp.IsMatches(relativeFilePath)))
            {
                _deleteFileAction(relativeFilePath);
            }
        }
    }
}