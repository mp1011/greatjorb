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
        [TestCase(Site.Indeed)]
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

            if (navigator == null || loginResult == null || loginResult.Data == null || loginResult.Data.Page == null || extractor == null)
            {
                Assert.Fail();
                return;
            }

            await navigator.GotoJobsListPage(loginResult.Data.Page, "c#", new CancellationToken());
            var job1 = await extractor.ExtractNextJob(loginResult.Data.Page, new HashSet<string>(), new CancellationToken());

            Assert.IsTrue(await extractor!.GotoNextPage(loginResult.Data.Page, new CancellationToken()));
            var job2 = await extractor.ExtractNextJob(loginResult.Data.Page, new HashSet<string>(), new CancellationToken());

            if (job1 == null || job2 == null)
            {
                Assert.IsNotNull(job1);
                Assert.IsNotNull(job2);
                return;
            }

            Assert.AreNotEqual(job1.StorageKey, job2.StorageKey);
        }
    }
}
