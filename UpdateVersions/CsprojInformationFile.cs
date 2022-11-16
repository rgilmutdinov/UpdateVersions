using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UpdateVersions
{
    public class CsprojInformationFile : IInformationFile
    {
        private XDocument xml;
        private string origPath;

        public CsprojInformationFile(string path)
        {
            origPath = path;

            xml = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        }

        public void UpdateVersions(Version fileVersion, Version productVersion, string copyright)
        {
            var assemblyVersionNode = xml.XPathSelectElement("//AssemblyVersion");
            if (assemblyVersionNode != null)
            {
                ApplyVersion(assemblyVersionNode, fileVersion);
            }

            var fileVersionNode = xml.XPathSelectElement("//FileVersion");
            if (fileVersionNode != null)
            {
                ApplyVersion(fileVersionNode, fileVersion);
            }

            var copyrightNode = xml.XPathSelectElement("//Copyright");
            if (copyrightNode != null)
            {
                copyrightNode.Value = copyright;
            }
        }

        public void Save(string path = null)
        {
            string outPath = path ?? origPath;
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = new UTF8Encoding(false)
            };

            using (XmlWriter writer = XmlWriter.Create(outPath, settings))
            {
                xml.Save(writer);
            }
            //xml.Save(outPath, SaveOptions.DisableFormatting);
        }

        private void ApplyVersion(XElement versionNode, Version fileVersion)
        {
            string[] parts = versionNode.Value.Split('.');

            int major    = parts.Length > 0 ? int.Parse(parts[0]) : 0;
            int minor    = parts.Length > 1 ? int.Parse(parts[1]) : 0;
            int build    = parts.Length > 2 ? int.Parse(parts[2]) : 0;
            int revision = parts.Length > 3 ? int.Parse(parts[3]) : 0;

            int newMajor    = ApplyChange(major, fileVersion.Major);
            int newMinor    = ApplyChange(minor, fileVersion.Minor);
            int newBuild    = ApplyChange(build, fileVersion.Build);
            int newRevision = ApplyChange(revision, fileVersion.Revision);

            versionNode.Value = string.Join(".", newMajor, newMinor, newBuild, newRevision);
        }

        private int ApplyChange(int version, string change)
        {
            if (!string.IsNullOrEmpty(change) && change != "+0")
            {
                int changeVal = int.Parse(change);
                if (change.IndexOfAny(new[] { '+', '-' }) == 0)
                {
                    changeVal = version + changeVal;
                }

                version = changeVal;
            }
            return version;
        }
    }
}
