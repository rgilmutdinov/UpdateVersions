namespace UpdateVersions
{
    public interface IInformationFile
    {
        void UpdateVersions(Version fileVersion, Version productVersion, string copyright);

        void Save(string path = null);
    }
}
