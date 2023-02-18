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

    [TestCase("https://www.simplyhired.com/search?q=c%23", "/job/B7URbcUGdh0Ba4xNtFHM9PrNheh4lTL-YF9p_E12ch9KMAdwq8Aw4w?jobCardTrackingKey=3-0-1gpj5bdklkhql801-1gpj5bdl1gaji800-d937f18fb43cdfc1-",
        "https://www.simplyhired.com/job/B7URbcUGdh0Ba4xNtFHM9PrNheh4lTL-YF9p_E12ch9KMAdwq8Aw4w?jobCardTrackingKey=3-0-1gpj5bdklkhql801-1gpj5bdl1gaji800-d937f18fb43cdfc1-")]
    public void TestChangeRelativePath(string originalUrl, string relativePath, string expectedUrl)
    {
        Assert.AreEqual(expectedUrl, originalUrl.ChangeRelativePath(relativePath));
    }
}
