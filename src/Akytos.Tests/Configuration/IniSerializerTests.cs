using Akytos.Assets;
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
}