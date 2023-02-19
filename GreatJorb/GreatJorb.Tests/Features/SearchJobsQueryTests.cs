namespace GreatJorb.Tests.Features;


public class SearchJobsQueryTests
{
    [Category(TestType.WebTest2)]
    [TestCase(Site.LinkedIn, "c#")]
    [TestCase(Site.GoogleJobs, "c#")]
    [TestCase(Site.Indeed, "c#")]
    [TestCase(Site.SimplyHired, "c#")]
    [TestCase(Site.Dice, "c#")]
    [TestCase(Site.Monster, "c#")]
    public async Task CanSearchJobs(Site site, string query)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var sites = await serviceProvider.Mediator.Send(new GetSitesQuery(site));

        var webSite = sites.Single();

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var searchResult = await serviceProvider.Mediator.Send(
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), new HashSet<string>(), 3));

        Assert.IsNotEmpty(searchResult);

        var idCount = searchResult
            .Select(p => p.Job.StorageKey)
            .Distinct()
            .Count();

        Assert.AreEqual(3, idCount);
    }


    [Category(TestType.WebTest3)]
    [TestCase(Site.LinkedIn, "c#")]
    [TestCase(Site.GoogleJobs, "c#")]
    [TestCase(Site.Indeed, "c#")]
    [TestCase(Site.SimplyHired, "c#")]
    [TestCase(Site.Dice, "c#")]
    [TestCase(Site.Monster, "c#")]
    public async Task JobSearchSkipsPreviouslyFoundJobs(Site site, string query)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var sites = await serviceProvider.Mediator.Send(new GetSitesQuery(site));

        var webSite = sites.Single();

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        //var searchResult = await serviceProvider.Mediator.Send(
        //    new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), 20));

        
        //var newSearchResult = await serviceProvider.Mediator.Send(
        //    new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), 20));

        throw new Exception("check new result does not contain anything from first result");
        throw new Exception("need to deal with previous urls");

    }

    [Category(TestType.WebTest3)]
    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#", 70000, WorkplaceType.OnSite)]
    [TestCase("Google Jobs", "https://www.google.com/", "c#", 0, WorkplaceType.Remote)]

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
            new SearchJobsFromSiteQuery(loggedInPage.Data, filter, new HashSet<string>(), 1));

        Assert.IsNotEmpty(searchResult);

        if (salaryMin > 0)
        {
            Assert.IsTrue(searchResult
                .SelectMany(p => p.FilterMatches)
                .Any(p => p.Field == nameof(JobFilter.Salary) && p.Level == FilterMatchLevel.PositiveMatch));
        }

        Assert.IsTrue(searchResult
             .SelectMany(p => p.FilterMatches)
             .Any(p => p.Field == nameof(JobFilter.WorkplaceTypeFilter) && p.Level == FilterMatchLevel.PositiveMatch));
    }

    [Category(TestType.WebTest3)]
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query),new HashSet<string>(), numberOfPages));

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

    [Category(TestType.WebTest3)]
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), new HashSet<string>(), 1));

        foreach (var result in searchResult)
        {
            if(result.KeywordLines.Any())
            {
                return;
            }
        }

        Assert.Fail("no keyword matches found");
    }

    [Category(TestType.WebTest3)]
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), new HashSet<string>(), 3));

        string[] distinctUrls = searchResult
            .Select(p => p.Job.Uri.PathAndQuery)
            .Distinct()
            .ToArray();

        Assert.AreEqual(distinctUrls.Length, searchResult.Length);
    }

    [Category(TestType.WebTest3)]
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), new HashSet<string>(), Limit:5));

        Assert.IsTrue(searchResult.Select(p=>p.Job).Any(p => p.Company != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.DescriptionHtml != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.Location != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.Title != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.JobType != JobType.Unknown));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.JobLevel != JobLevel.Unknown));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.WorkplaceType != WorkplaceType.Unknown));
    }

    [Category(TestType.WebTest3)]
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
            new SearchJobsFromSiteQuery(loggedInPage.Data, new JobFilter(query), new HashSet<string>(), Limit: 5));

        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.SalaryMin != null));
        Assert.IsTrue(searchResult.Select(p => p.Job).Any(p => p.SalaryMax != null));
    }
}
