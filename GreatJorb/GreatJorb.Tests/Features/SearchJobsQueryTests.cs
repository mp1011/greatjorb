namespace GreatJorb.Tests.Features;

public class SearchJobsQueryTests
{
    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#", 1)]
    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#", 2)]
    [TestCase("Google Jobs", "https://www.google.com/", "c#", 1)]
    [TestCase("Google Jobs", "https://www.google.com/", "c#", 2)]
    [TestCase("Google Jobs", "https://www.google.com/", "c#", 3)]
    public async Task CanSearchJobs(string name, string url, string query, int pageNumber)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), pageNumber));

        Assert.IsNotEmpty(searchResult);
        Assert.IsTrue(searchResult
            .SelectMany(p => p.FilterMatches)
            .Any(p => p.Level == FilterMatchLevel.PositiveMatch));
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, filter, 1));


        Assert.IsNotEmpty(searchResult);

        Assert.IsTrue(searchResult
            .SelectMany(p => p.FilterMatches)
            .Any(p => p.Field == nameof(JobFilter.Salary) && p.Level == FilterMatchLevel.PositiveMatch));

        Assert.IsTrue(searchResult
             .SelectMany(p => p.FilterMatches)
             .Any(p => p.Field == nameof(JobFilter.WorkplaceTypeFilter) && p.Level == FilterMatchLevel.PositiveMatch));
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), numberOfPages));

        bool noMatches = true;

        foreach (var result in searchResult)
        {
            if (!result.FilterMatches.Any(p => p.Field == nameof(JobFilter.Query) && p.Level == FilterMatchLevel.PositiveMatch))
                continue;
            
            if (result.Job.DescriptionHtml == null)
            {
                Assert.Fail("Description was empty");
                break;
            }

            if(!result.Job.DescriptionHtml.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                && !result.Job.DescriptionHtml.Contains(query.HtmlEncode(), StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.Fail("Description did not contain keyword");
                break;
            }

            noMatches = false;
        }

        Assert.IsFalse(noMatches);
    }

    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#")]
    public async Task CanExtractLinesWithKeyword(string name, string url, string query)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), 1));

        foreach (var result in searchResult)
        {
            Assert.IsNotEmpty(result.KeywordLines);
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), 3));

        string[] distinctUrls = searchResult
            .Select(p => p.Job.Uri.PathAndQuery)
            .Distinct()
            .ToArray();

        Assert.AreEqual(distinctUrls.Length, searchResult.Length);
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), PageNumber:1));

        Assert.IsTrue(searchResult.Select(p=>p.Job).Any(p => p.Company != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.DescriptionHtml != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.Location != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.Title != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.JobType != JobType.Unknown));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.JobLevel != JobLevel.Unknown));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.WorkplaceType != WorkplaceType.Unknown));
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), PageNumber: 1));

        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.SalaryMin != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.SalaryMax != null));
    }
}
