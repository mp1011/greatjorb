﻿namespace GreatJorb.Tests.Features;

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
            new SearchJobsQuery(loggedInPage.Data, query, numberOfPages));

        Assert.IsNotEmpty(searchResult);
        Assert.IsTrue(searchResult.All(p => p.Url != null)); 
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
            new SearchJobsQuery(loggedInPage.Data, query, numberOfPages));

        foreach (var result in searchResult)
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
            new SearchJobsQuery(loggedInPage.Data, query, 3));

        string[] distinctUrls = searchResult
            .Select(p => p.Url ?? "")
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
            new SearchJobsQuery(loggedInPage.Data, query, NumberOfPages:1));

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
            new SearchJobsQuery(loggedInPage.Data, query, NumberOfPages: 1));

        Assert.IsTrue(searchResult.Any(p => p.SalaryMin != null));
        Assert.IsTrue(searchResult.Any(p => p.SalaryMax != null));
    }
}
