﻿namespace GreatJorb.Tests.Features;

[Category(TestType.UnitTest)]
public class CacheCommandQueryTests
{
    [Test]
    public async Task CanSaveFilterToCache()
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: false,
            includeDataContext: true);

        var jobFilter = new JobFilter { Query = Guid.NewGuid().ToString() };

        await serviceProvider.Mediator.Send(new AddFilterToCacheCommand(jobFilter));

        var cached = await serviceProvider.Mediator.Send(new LoadFiltersFromCacheQuery());
        Assert.IsNotNull(cached);
        Assert.IsNotEmpty(cached);

        Assert.AreEqual(jobFilter.Query, cached.OrderByDescending(p => p.Date).First().Filter.Query);
    }

    [Test]
    public async Task CanCacheDataAndRetrieveIt()
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
         includeConfiguration: true,
         includeMediator: true,
         includePuppeteer: false,
         includeDataContext: true);

        var fakeJobPosting = new JobPosting("https://www.google.com/FakeResult", "https://www.google.com/FakeResult")
        {
            DescriptionHtml = Guid.NewGuid().ToString(),
        };

        var site = new WebSite("Test", "https://www.google.com",false);

        var cached = await serviceProvider.Mediator.Send(
            new AddJobResultToCacheCommand(site, fakeJobPosting));


        var retrieved = await serviceProvider.Mediator.Send(
            new SearchJobsFromSiteCachedQuery(site, new JobFilter(fakeJobPosting.DescriptionHtml)));

        Assert.AreEqual(1, retrieved.Length);
        Assert.AreEqual(fakeJobPosting.DescriptionHtml, retrieved[0].Job.DescriptionHtml);

        var cachedAgain = await serviceProvider.Mediator.Send(
          new AddJobResultToCacheCommand(site, fakeJobPosting));

        Assert.That(cachedAgain.CacheDate > cached.CacheDate);

        var retrievedAgain = await serviceProvider.Mediator.Send(
         new SearchJobsFromSiteCachedQuery(site, new JobFilter(fakeJobPosting.DescriptionHtml)));

        Assert.AreEqual(1, retrievedAgain.Length);

    }
}
