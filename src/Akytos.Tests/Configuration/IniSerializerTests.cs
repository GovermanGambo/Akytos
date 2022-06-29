using System.Collections.Generic;
using Akytos.Configuration;
using Xunit;

namespace Akytos.Tests.Configuration;

public class IniSerializerTests
{
    [Fact]
    public void Deserialization_Should_Work()
    {
        var serializer = new IniSerializer();

        const string ini = @"[Projects]
CurrentProject = C:/Users/Bob/Akytos/BestProject
AnotherConfig = test

[User]
CurrentLayout = 0
Language = English
";

        var dictionary = serializer.Deserialize(ini);
        
        Assert.Equal(2, dictionary.Keys.Count);
        
        Assert.Equal(2, dictionary["Projects"].Count);
        Assert.Equal("C:/Users/Bob/Akytos/BestProject", dictionary["Projects"]["CurrentProject"]);
        Assert.Equal("test", dictionary["Projects"]["AnotherConfig"]);
        
        Assert.Equal(2, dictionary["User"].Count);
        Assert.Equal("0", dictionary["User"]["CurrentLayout"]);
        Assert.Equal("English", dictionary["User"]["Language"]);

        string serializedIni = serializer.Serialize(dictionary);
        
        Assert.Equal(ini.Trim(), serializedIni.Trim());
    }

    [Fact]
    public void Serialization_Should_Work()
    {
        var serializer = new IniSerializer();
        
        const string expected = @"
[Projects]
CurrentProject = C:/Users/Bob/Akytos/BestProject
AnotherConfig = test

[User]
CurrentLayout = 0
Language = English
";
        var dictionary = new Dictionary<string, Dictionary<string, string>>();

        var projects = new Dictionary<string, string>();
        projects.Add("CurrentProject", "C:/Users/Bob/Akytos/BestProject");
        projects.Add("AnotherConfig", "test");
        
        var user = new Dictionary<string, string>();
        user.Add("CurrentLayout", "0");
        user.Add("Language", "English");
        
        dictionary.Add("Projects", projects);
        dictionary.Add("User", user);

        string ini = serializer.Serialize(dictionary);
        Assert.Equal(expected, ini);
    }
}