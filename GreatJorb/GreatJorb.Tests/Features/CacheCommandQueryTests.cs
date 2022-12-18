namespace GreatJorb.Tests.Features;

public class CacheCommandQueryTests
{

    [Test]
    public async Task CanCacheDataAndRetrieveIt()
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
         includeConfiguration: true,
         includeMediator: true,
         includePuppeteer: false,
         includeDataContext: true);

        var fakeJobPosting = new JobPosting("https://www.google.com/FakeResult")
        {
            DescriptionHtml = Guid.NewGuid().ToString(),
        };

        var site = new WebSite("Test", "https://www.google.com");

        var cached = await serviceProvider.Mediator.Send(
            new AddJobResultToCacheCommand(site, fakeJobPosting));


        var retrieved = await serviceProvider.Mediator.Send(
            new SearchJobsFromCacheQuery(site, new JobFilter(fakeJobPosting.DescriptionHtml)));

        Assert.AreEqual(1, retrieved.MatchesFilter.Length);
        Assert.AreEqual(fakeJobPosting.DescriptionHtml, retrieved.MatchesFilter[0].DescriptionHtml);

        var cachedAgain = await serviceProvider.Mediator.Send(
          new AddJobResultToCacheCommand(site, fakeJobPosting));

        Assert.That(cachedAgain.CacheDate > cached.CacheDate);

        var retrievedAgain = await serviceProvider.Mediator.Send(
         new SearchJobsFromCacheQuery(site, new JobFilter(fakeJobPosting.DescriptionHtml)));

        Assert.AreEqual(1, retrievedAgain.MatchesFilter.Length);

    }
}
