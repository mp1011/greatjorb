namespace GreatJorb.Tests.Features;

public class SearchJobsQueryTests
{
    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#", 1)]
    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#", 2)]
    public async Task CanSearchJobs(string name, string url, string query, int numberOfPages)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsQuery(loggedInPage.Data, new JobFilter(query), numberOfPages));

        Assert.IsNotEmpty(searchResult.MatchesFilter);
    }

    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#", 70000, WorkplaceType.OnSite)]
    public async Task CanSearchJobsWithFilters(string name, string url, string query, decimal salaryMin, WorkplaceType workplaceTypeFilter)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
           includeConfiguration: true,
           includeMediator: true,
           includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        JobFilter filter = new()
        {
            Query = query,
            Salary = salaryMin,
            WorkplaceTypeFilter = workplaceTypeFilter
        };

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsQuery(loggedInPage.Data, filter, 1));


        Assert.IsNotEmpty(searchResult.MatchesFilter);
        Assert.IsNotEmpty(searchResult.DoesNotMatchFilter);

        Assert.True(searchResult.MatchesFilter.All(p => p.SalaryMin >= salaryMin && workplaceTypeFilter.HasFlag(p.WorkplaceType)));
        Assert.False(searchResult.DoesNotMatchFilter.Any(p => p.SalaryMin >= salaryMin && workplaceTypeFilter.HasFlag(p.WorkplaceType)));
    }



    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#", 1)]
    public async Task JobSearchDoesNotReturnResultsThatDoNotContainKeywords(string name, string url, string query, int numberOfPages)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsQuery(loggedInPage.Data, new JobFilter(query), numberOfPages));

        foreach (var result in searchResult.MatchesFilter)
        {
            if (result.DescriptionHtml == null)
            {
                Assert.Fail("Description was empty");
                break;
            }

            if(!result.DescriptionHtml.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                && !result.DescriptionHtml.Contains(query.HtmlEncode(), StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.Fail("Description did not contain keyword");
                break;
            }
        }
    }

    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#")]
    public async Task SearchJobsDoesNotReturnDuplicates(string name, string url, string query)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsQuery(loggedInPage.Data, new JobFilter(query), 3));

        string[] distinctUrls = searchResult
            .MatchesFilter
            .Select(p => p.Uri.PathAndQuery)
            .Distinct()
            .ToArray();

        Assert.AreEqual(distinctUrls.Length, searchResult.MatchesFilter.Length);
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
            new SearchJobsQuery(loggedInPage.Data, new JobFilter(query), NumberOfPages:1));

        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.Company != null));
        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.DescriptionHtml != null));
        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.Location != null));
        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.Title != null));
        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.JobType != JobType.Unknown));
        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.JobLevel != JobLevel.Unknown));
        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.WorkplaceType != WorkplaceType.Unknown));
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
            new SearchJobsQuery(loggedInPage.Data, new JobFilter(query), NumberOfPages: 1));

        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.SalaryMin != null));
        Assert.IsTrue(searchResult.MatchesFilter.Any(p => p.SalaryMax != null));
    }
}
