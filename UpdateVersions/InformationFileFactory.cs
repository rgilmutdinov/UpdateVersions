using System;
using System.IO;

namespace UpdateVersions
{
    public class InformationFileFactory
    {
        public InformationFile GetFile(string path)
        {
            string extension = Path.GetExtension(path)?.ToLower();
            if (extension == ".cs")
            {
                return new CsInformationFile(path);
            }

            if (extension == ".rc")
            {
                return new RcInformationFile(path);
            }

            throw new Exception($"Can\'t read information file: '{path}'");
        }
    }
}
