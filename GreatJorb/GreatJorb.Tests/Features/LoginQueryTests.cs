namespace GreatJorb.Tests.Features;

[Category(TestType.WebTest1)]
public class LoginQueryTests
{

    [TestCase("LinkedIn")]
    [TestCase("Google Jobs")]
    [TestCase("Indeed")]
    [TestCase("Simply Hired")]
    [TestCase("Dice")]
    [TestCase("Monster")]

    public async Task CanLoginToWebsite(string siteName)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var sites = await serviceProvider.Mediator.Send(new GetSitesQuery());

        var webSite = sites.First(p => p.Name == siteName);

        var result = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }
}
