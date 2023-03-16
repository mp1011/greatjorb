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

    [TestCase("https://www.simplyhired.com/search?q=c%23", "/job/B7URbcUGdh0Ba4xNtFHM9PrNheh4lTL-YF9p_E12ch9KMAdwq8Aw4w?jobCardTrackingKey=3-0-1gpj5bdklkhql801-1gpj5bdl1gaji800-d937f18fb43cdfc1-",        "https://www.simplyhired.com/job/B7URbcUGdh0Ba4xNtFHM9PrNheh4lTL-YF9p_E12ch9KMAdwq8Aw4w?jobCardTrackingKey=3-0-1gpj5bdklkhql801-1gpj5bdl1gaji800-d937f18fb43cdfc1-")]    
    public void TestChangeRelativePath(string originalUrl, string relativePath, string expectedUrl)
    {
        Assert.AreEqual(expectedUrl, originalUrl.ChangeRelativePath(relativePath));
    }

    [TestCase("https://www.google.com/search?q=c%23+jobs&source=hp&ei=yUnyY7KVJ-2nptQPxP6SqAI&iflsig=AK50M_UAAAAAY_JX2eHgY5l2DnIue6cZxYurMaPTcvNq&uact=5&oq=c%23+jobs&gs_lcp=Cgdnd3Mtd2l6EANQAFgaYB9oAHAAeACAAQCIAQCSAQCYAQCgAQE&sclient=gws-wiz&ibp=htl;jobs&sa=X&ved=2ahUKEwjUxJi0_KH9AhW_rIkEHYR-AMQQutcGKAF6BAgIEAU#htivrt=jobs&htidocid=kfeT03Odsg0AAAAAAAAAAA%3D%3D&fpstate=tldetail", "htidocid", "kfeT03Odsg0AAAAAAAAAAA==")]
    [TestCase("https://www.google.com/search?q=c%23+jobs&source=hp&ei=yUnyY7KVJ-2nptQPxP6SqAI&iflsig=AK50M_UAAAAAY_JX2eHgY5l2DnIue6cZxYurMaPTcvNq&uact=5&oq=c%23+jobs&gs_lcp=Cgdnd3Mtd2l6EANQAFgaYB9oAHAAeACAAQCIAQCSAQCYAQCgAQE&sclient=gws-wiz&ibp=htl;jobs&sa=X&ved=2ahUKEwjUxJi0_KH9AhW_rIkEHYR-AMQQutcGKAF6BAgIEAU#htivrt=jobs&htidocid=kfeT03Odsg0AAAAAAAAAAA%3D%3D&fpstate=tldetail", "source", "hp")]
    [TestCase("https://www.google.com/search?ibp=htl;jobs&q=c%23+jobs&htidocid=kfeT03Odsg0AAAAAAAAAAA%3D%3D&hl=en-US&kgs=ca15b7bea4686526&shndl=-1&source=sh/x/im/textlists/detail/1&ibp=htl;jobs&htidocid=cniPbX55XEsAAAAAAAAAAA%3D%3D#fpstate=tldetail&htivrt=jobs&htiq=c%23+jobs&htidocid=cniPbX55XEsAAAAAAAAAAA%3D%3D","htidocid", "cniPbX55XEsAAAAAAAAAAA==")]
    public void TestGetQueryStringOrHashValue(string url, string key, string expectedValue)
    {
        Assert.AreEqual(expectedValue, url.GetQuerystringOrHashValue(key));
    }

    [TestCase(".net", ".net framework", true)]
    [TestCase(".net", "xyz/.net/c#", true)]
    [TestCase("java", "javascript", false)]
    [TestCase("java", "javascript, java", true)]
    [TestCase("ny", "many", false)]
    [TestCase("ny", "states:ny,ca,pa", true)]
    public void TestContainsWord(string word, string text, bool shouldContain)
    {
        Assert.AreEqual(shouldContain, text.ContainsWord(word));
    }
}
