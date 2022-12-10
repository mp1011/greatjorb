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
        Assert.IsTrue(searchResult.All(p => p.Url != null)); 
    }

    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#")]
    public async Task CanExtractDetailFromJob(string name, string url, string query)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsQuery(loggedInPage.Data, query, PageSize:1));

        Assert.IsTrue(searchResult.Any(p => p.Company != null));
        Assert.IsTrue(searchResult.Any(p => p.DescriptionHtml != null));
        Assert.IsTrue(searchResult.Any(p => p.Location != null));
        Assert.IsTrue(searchResult.Any(p => p.Title != null));
        Assert.IsTrue(searchResult.Any(p => p.JobType != JobType.Unknown));
        Assert.IsTrue(searchResult.Any(p => p.JobLevel != JobLevel.Unknown));
        Assert.IsTrue(searchResult.Any(p => p.WorkplaceType != WorkplaceType.Unknown));
    }

    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#")]
    public async Task CanExtractSalaryFromJob(string name, string url, string query)
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

        Assert.IsTrue(searchResult.Any(p => p.SalaryMin != null));
        Assert.IsTrue(searchResult.Any(p => p.SalaryMax != null));
    }
}
