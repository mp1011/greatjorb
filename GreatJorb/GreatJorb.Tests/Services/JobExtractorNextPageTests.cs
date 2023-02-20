namespace GreatJorb.Tests.Services
{
    [Category(TestType.WebTest2)]
    public class JobExtractorNextPageTests
    {
        [TestCase(Site.LinkedIn)]
        [TestCase(Site.GoogleJobs)]
        [TestCase(Site.Monster)]
        [TestCase(Site.SimplyHired)]
        [TestCase(Site.Dice)]
        public async Task CanGoToNextPageOfResults(Site site)
        {
            using var serviceProvider = TestServiceProvider.CreateServiceProvider(
               includeConfiguration: true,
               includeMediator: true,
               includePuppeteer: true);

            var webSite = (await serviceProvider.Mediator.Send(new GetSitesQuery(site))).First();

            var navigator = await serviceProvider.Mediator.Send(
                new GetNavigatorQuery(webSite));

            var extractor = await serviceProvider.Mediator.Send(
                new GetExtractorQuery(webSite));
            
            var loginResult = await serviceProvider.Mediator.Send(new LoginQuery(webSite));

            if (navigator == null || loginResult == null || loginResult.Data == null || loginResult.Data.Page == null)
            {
                Assert.Fail();
                return;
            }

            await navigator.GotoJobsListPage(loginResult.Data.Page, "c#", new CancellationToken());
            Assert.IsTrue(await extractor!.GotoNextPage(loginResult.Data.Page, new CancellationToken()));

            //do it again
            Assert.IsTrue(await extractor!.GotoNextPage(loginResult.Data.Page, new CancellationToken()));
        }
    }
}
