using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;



namespace CleanSolution.Command.Services
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///     https://stackoverflow.com/questions/652037/how-do-i-check-if-a-filename-matches-a-wildcard-pattern
    /// </remarks>
    internal class PatternMatch
    {
        private static readonly Regex _hasQuestionMarkRegEx = new Regex(@"\?", RegexOptions.Compiled);

        private static readonly Regex _illegalCharactersRegex =
            new Regex("[" + @"\/:<>|" + "\"]", RegexOptions.Compiled);

        private static readonly Regex _catchExtentionRegex = new Regex(@"^\s*.+\.([^\.]+)\s*$", RegexOptions.Compiled);
        private static readonly string _nonDotCharacters = @"[^.]*";

        private readonly Regex _regex;

        public string Pattern => _regex.ToString();

        
        public PatternMatch(string pattern) => _regex = patternToRegEx(pattern);


        public IEnumerable<string> GetMatches(IEnumerable<string> names) 
            => names.Where(s => _regex.IsMatch(s));  
        
        public bool IsMatches(string s) => _regex.IsMatch(s);


        private Regex patternToRegEx(string pattern)
        {
            if (pattern == null) throw new ArgumentNullException();
            pattern = pattern.Trim();
            if (pattern.Length == 0) throw new ArgumentException("Pattern is empty.");
            if (_illegalCharactersRegex.IsMatch(pattern))
                throw new ArgumentException("Pattern contains illegal characters.");
            bool hasExtension = _catchExtentionRegex.IsMatch(pattern);
            bool matchExact = false;
            if (_hasQuestionMarkRegEx.IsMatch(pattern)) matchExact = true;
            else if (hasExtension) matchExact = _catchExtentionRegex.Match(pattern).Groups[1].Length != 3;
            string regexString = Regex.Escape(pattern);
            regexString = "^" + Regex.Replace(regexString, @"\\\*", ".*");
            regexString = Regex.Replace(regexString, @"\\\?", ".");
            if (!matchExact && hasExtension) regexString += _nonDotCharacters;
            regexString += "$";
            return new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}