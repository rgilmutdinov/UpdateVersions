using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UpdateVersions
{
    public abstract class InformationFile
    {
        private static IEnumerable<string> ReadAllLines(Stream fs, Encoding encoding)
        {
            using (StreamReader fileReader = new StreamReader(fs, encoding))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public InformationFile(string path)
        {
            FilePath = path;
            Stream fs = File.OpenRead(path);
            Encoding = DetectFileEncoding(fs);

            Lines = ReadAllLines(fs, Encoding).ToArray();
        }

        public string   FilePath { get; }
        public string[] Lines    { get; }
        public Encoding Encoding { get; }

        public void UpdateVersions(Version fileVersion, Version productVersion, string copyright)
        {
            for (int index = 0; index < Lines.Length; index++)
            {
                string line = Lines[index];

                string newLine;
                if (fileVersion != null)
                {
                    if (ProcessFileVersion(line, fileVersion, out newLine))
                    {
                        Lines[index] = newLine;
                    }
                }

                if (productVersion != null)
                {
                    if (ProcessProductVersion(line, productVersion, out newLine))
                    {
                        Lines[index] = newLine;
                    }
                }

                if (!string.IsNullOrWhiteSpace(copyright))
                {
                    if (ProcessCopyright(line, copyright, out newLine))
                    {
                        Lines[index] = newLine;
                    }
                }
            }
        }

        protected abstract bool ProcessFileVersion(string line, Version ver, out string newLine);
        protected abstract bool ProcessProductVersion(string line, Version ver, out string newLine);
        protected abstract bool ProcessCopyright(string line, string copyright, out string newLine);

        protected bool ProcessVersion(string line, Regex regex, Version ver, out string newLine)
        {
            newLine = null;

            Match match = regex.Match(line);
            if (match.Success)
            {
                int major    = int.Parse(match.Groups["major"].Value);
                int minor    = int.Parse(match.Groups["minor"].Value);
                int build    = int.Parse(match.Groups["build"].Value);
                int revision = int.Parse(match.Groups["revision"].Value);

                int newMajor    = ApplyChange(major, ver.Major);
                int newMinor    = ApplyChange(minor, ver.Minor);
                int newBuild    = ApplyChange(build, ver.Build);
                int newRevision = ApplyChange(revision, ver.Revision);

                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    {"major",    $"{newMajor}"},
                    {"minor",    $"{newMinor}"},
                    {"build",    $"{newBuild}"},
                    {"revision", $"{newRevision}"}
                };

                newLine = ReplaceGroups(line, match, replacements);
                return true;
            }

            return false;
        }

        protected int ApplyChange(int version, string change)
        {
            if (!string.IsNullOrEmpty(change) && change != "+0")
            {
                int changeVal = int.Parse(change);
                if (change.IndexOfAny(new[] {'+', '-'}) == 0)
                {
                    changeVal = version + changeVal;
                }

                version = changeVal;
            }
            return version;
        }

        protected string ReplaceGroups(string line, Match match, Dictionary<string, string> replacements)
        {
            string result = line;
            foreach (KeyValuePair<string, string> replacement in replacements.OrderByDescending(r => match.Groups[r.Key].Index))
            {
                string groupName = replacement.Key;
                string replaceWith = replacement.Value;
                CaptureCollection captures = match.Groups[groupName]?.Captures;
                if (captures != null)
                {
                    foreach (Capture cap in captures)
                    {
                        result = result.Remove(cap.Index, cap.Length);
                        result = result.Insert(cap.Index, replaceWith);
                    }
                }
            }

            return result;
        }

        public void Save(string path = null)
        {
            string outPath = path ?? FilePath;
            File.WriteAllLines(outPath, Lines, Encoding);
        }

        private static Encoding DetectFileEncoding(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream, Encoding.Default, true, 1024, true))
            {
                reader.Peek();
                Encoding encoding = reader.CurrentEncoding;

                // Rewind the stream
                fileStream.Seek(0, SeekOrigin.Begin);
                return encoding;
            }
        }
    }
}
