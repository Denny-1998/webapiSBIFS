namespace webapiSBIFS.Tools
{
    public class ConfigFile : TextFile
    {
        public IEnumerable<KeyValuePair<string, string>> GetProperties(string path)
        {
            string configs = base.GetAllTextFromFile(path);
            string[] lines = configs.Split('\n');

            foreach (string line in lines)
            {
                if (line == "" || line == " " || line == null) continue;
                int index = line.IndexOf('=');
                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();

                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
}
