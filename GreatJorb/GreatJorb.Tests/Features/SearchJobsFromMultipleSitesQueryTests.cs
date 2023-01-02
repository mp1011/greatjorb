namespace GreatJorb.Tests.Features;

public class SearchJobsFromMultipleSitesQueryTests
{
    [Test]
    public async Task CanSearchFromMultipleSites()
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true,
            includeDataContext: true);

        var filter = new JobFilter
        {
            Query = "c#",
            Sites = Site.GoogleJobs | Site.LinkedIn
        };

        var result = await serviceProvider.Mediator.Send(new SearchJobsFromMultipleSitesQuery(filter, 2));

        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result);
    }
}
