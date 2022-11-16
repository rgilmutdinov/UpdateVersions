using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UpdateVersions
{
    public class CsInformationFile : TextInformationFile
    {
        public CsInformationFile(string path) : base(path)
        {
        }

        protected override bool ProcessFileVersion(string line, Version ver, out string newLine)
        {
            Regex regex = new Regex(@"(AssemblyFileVersion)([^\d-]*)((?<major>-?\d+)(,|\.)(\s*)(?<minor>-?\d+)(,|\.)(\s*)(?<build>-?\d+)(,|\.)(\s*)(?<revision>-?\d+))");

            return ProcessVersion(line, regex, ver, out newLine);
        }

        protected override bool ProcessVersion(string line, Version ver, out string newLine)
        {
            Regex regex = new Regex(@"(AssemblyVersion)([^\d-]*)((?<major>-?\d+)(,|\.)(\s*)(?<minor>-?\d+)(,|\.)(\s*)(?<build>-?\d+)(,|\.)(\s*)(?<revision>-?\d+))");

            return ProcessVersion(line, regex, ver, out newLine);
        }

        protected override bool ProcessProductVersion(string line, Version ver, out string newLine)
        {
            newLine = string.Empty;
            return false;
        }

        protected override bool ProcessCopyright(string line, string copyright, out string newLine)
        {
            newLine = null;

            Regex regex = new Regex(@"AssemblyCopyright\(\""(?<copyright>[^\""\\]*)\""\)");
            Match match = regex.Match(line);
            if (match.Success)
            {
                newLine = ReplaceGroups(line, match, new Dictionary<string, string> { { "copyright", copyright } });
                return true;
            }

            return false;
        }
    }
}
