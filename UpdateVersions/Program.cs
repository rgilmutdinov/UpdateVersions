using System;

namespace UpdateVersions
{
    class Program
    {
        private const int ErrorSuccess = 0;
        private const int ErrorFail    = 1;

        const string FileVersion    = "/fileversion:";
        const string ProductVersion = "/productversion:";
        const string Copyright      = "/copyright:";

        static int Main(string[] args)
        {
            string filePath = "";
            if (args.Length > 0)
            {
                filePath = args[0].ToLower();
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return ErrorFail;
            }

            InformationFileFactory fileFactory = new InformationFileFactory();
            IInformationFile infoFile = fileFactory.GetFile(filePath);

            Version fileVersion = null;
            Version productVersion = null;
            string copyright = null;

            for (int i = 1; i < args.Length; ++i)
            {
                string arg = args[i];
                if (arg.IndexOf(FileVersion, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string fileVer = arg.ToLower().Remove(0, FileVersion.Length);
                    fileVersion = new Version(fileVer);
                }
                else if (arg.IndexOf(ProductVersion, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string prodVer = arg.ToLower().Remove(0, ProductVersion.Length);
                    productVersion = new Version(prodVer);
                }
                else if (arg.IndexOf(Copyright, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyright = arg.Remove(0, Copyright.Length);
                }
            }

            infoFile.UpdateVersions(fileVersion, productVersion, copyright);
            infoFile.Save();

            return ErrorSuccess;
        }
    }
}
