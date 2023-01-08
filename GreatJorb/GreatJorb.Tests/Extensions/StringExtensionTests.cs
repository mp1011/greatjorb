namespace GreatJorb.Tests.Extensions;

[Category(TestType.UnitTest)]
public class StringExtensionTests
{
    [TestCase("Hello World", "*hello*", true)]
    [TestCase("Hello World", "*ello w*", true)]
    [TestCase("Hello World", "*ello g*", false)]
    [TestCase("Hello World", "*ello*rl*", true)]
    [TestCase("Hello World", "*ello*rg*", false)]
    public void TestWildcardMatch(string text, string pattern, bool shouldMatch)
    {
        Assert.AreEqual(text.IsWildcardMatch(pattern), shouldMatch);
    }
}
