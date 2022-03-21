using Akytos.Utilities;
using Xunit;

namespace Akytos.Tests.Utilities;

public class StringExtensionsTest
{
    [Theory]
    [InlineData("camelCase", "Camel Case")]
    [InlineData("m_camelCase", "Camel Case")]
    [InlineData("PascalCase", "Pascal")]
    public void SplitCamelCase_ShouldWork(string camelCase, string result)
    {
        Assert.Equal(result, camelCase.SplitCamelCase());
    }
}