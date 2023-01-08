namespace GreatJorb.Tests.Features;

[Category(TestType.WebTest)]
public class LoginQueryTests
{
    [TestCase("LinkedIn", "https://www.linkedin.com/", "https://www.linkedin.com/feed")]
    [TestCase("Google Jobs", "https://www.google.com", "https://www.google.com")]

    public async Task CanLoginToWebsite(string name, string url, string loggedInUrl)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var result = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsTrue(result.Data.Page!.Url.StartsWith(loggedInUrl));
    }
}
