using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;



namespace CleanSolution.Command.Services
{
    internal class CleanFolder
    {
        private static readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private readonly string _rootPath;
        private readonly IEnumerable<PatternMatch> _deletionPatterns;
        private readonly IEnumerable<PatternMatch> _ignorePatterns;



        public CleanFolder(string rootPath, IEnumerable<string> deletionPatterns, IEnumerable<string> ignorePatterns)
            : this(rootPath, deletionPatterns.Select(p => new PatternMatch(p)),
                ignorePatterns.Select(p => new PatternMatch(p)))
        {
        }



        private CleanFolder(string rootPath, IEnumerable<PatternMatch> deletionPatterns, IEnumerable<PatternMatch> ignorePatterns)
        {
            if (!Path.IsPathRooted(rootPath)) throw new ArgumentException("Path must be rooted", nameof(rootPath));
            _rootPath = rootPath;
            _deletionPatterns = deletionPatterns;
            _ignorePatterns = ignorePatterns;
        }



        public void Execute() => processDirectory(_rootPath);


        void processDirectory(string currentDirFullPath)
        {
            string currentDirPath = Path.GetRelativePath(_rootPath, currentDirFullPath);
            _log.Info($"> {currentDirPath}");

            var currentDirs = Directory.GetDirectories(currentDirFullPath).Select(d => new DirectoryInfo(d));
            foreach (DirectoryInfo di in currentDirs)
            {
                string matchCheckPath = "\\" + Path.GetRelativePath(_rootPath, di.FullName);

                // if directory is an exclude match --> skip it
                if (_ignorePatterns.Any(ip => ip.IsMatches(matchCheckPath)))
                {
                    _log.Debug($"EX: {matchCheckPath}");
                    continue;
                }

                // process current directory
                if (_deletionPatterns.Any(dp => dp.IsMatches(matchCheckPath)))
                {
                    _log.Info($"RD: {matchCheckPath}");
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
                    _log.Info($"DEL {relativeFilePath}");
                }
            }
        }
    }
}