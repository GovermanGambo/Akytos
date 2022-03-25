namespace Akytos.Configuration;

public class IniSerializer
{
    public string Serialize(Dictionary<string, Dictionary<string, string>> dictionary)
    {
        string fileContent = "";
        
        foreach (string category in dictionary.Keys)
        {
            var categoryDictionary = dictionary[category];
            string content = string.Join(PlatformConstants.NewLine, categoryDictionary.Select(c => c.Key + " = " + c.Value));

            fileContent += string.Concat(PlatformConstants.NewLine, $"[{category}]", PlatformConstants.NewLine, content, PlatformConstants.NewLine);
        }

        return fileContent;
    }
    
    public Dictionary<string, Dictionary<string, string>> Deserialize(string ini)
    {
        string[] lines = ini.Split(PlatformConstants.NewLine);

        var dictionary = new Dictionary<string, Dictionary<string, string>>();
        
        if (lines.Length == 1 && lines[0] == "")
        {
            return dictionary;
        }
        
        string currentCategory = "";
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
            {
                continue;
            }
            
            if (line.Contains('[') && line.Contains(']'))
            {
                currentCategory = line.Trim()[1..^1];
                continue;
            }

            string key = line.Split(" ")[0];
            string value = line.Split(" ")[2];

            if (!dictionary.ContainsKey(currentCategory))
            {
                dictionary.Add(currentCategory, new Dictionary<string, string>());
            }

            dictionary[currentCategory][key] = value;
        }

        return dictionary;
    }
}