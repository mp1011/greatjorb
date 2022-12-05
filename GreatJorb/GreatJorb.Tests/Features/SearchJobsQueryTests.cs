namespace GreatJorb.Tests.Features;

public class SearchJobsQueryTests
{
    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#")]
    public async Task CanSearchJobs(string name, string url, string query)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsQuery(loggedInPage.Data, query));

        Assert.IsNotEmpty(searchResult);
    }
}
