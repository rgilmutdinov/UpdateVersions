namespace UpdateVersions
{
    public class Version
    {
        public Version(string input)
        {
            string[] parts = input.Split('.', ',');

            if (parts.Length > 0) Major    = parts[0];
            if (parts.Length > 1) Minor    = parts[1];
            if (parts.Length > 2) Build    = parts[2];
            if (parts.Length > 3) Revision = parts[3];
        }

        public string Major    { get; set; } = "+0";
        public string Minor    { get; set; } = "+0";
        public string Build    { get; set; } = "+0";
        public string Revision { get; set; } = "+0";
    }
}
