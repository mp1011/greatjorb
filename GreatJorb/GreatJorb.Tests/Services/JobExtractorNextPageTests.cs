namespace GreatJorb.Tests.Services
{
    [Category(TestType.WebTest2)]
    public class JobExtractorNextPageTests
    {
        [TestCase(Site.LinkedIn,false)]
        [TestCase(Site.GoogleJobs,true)]
        [TestCase(Site.Monster,true)]
        [TestCase(Site.SimplyHired,false)]
        [TestCase(Site.Dice,false)]
        [TestCase(Site.Indeed,false)]
        public async Task CanGoToNextPageOfResults(Site site, bool isSinglePage)
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

            var page = loginResult.Data.Page;

            await navigator.GotoJobsListPage(page, "c#", new CancellationToken());
            string jobsUrl = loginResult.Data.Page.Url;

            await Task.Delay(1000);
            var job1 = await extractor.ExtractNextJob(page, new HashSet<string>(), new CancellationToken());

            if (page.Url != jobsUrl)
                await page.GoToAsync(jobsUrl);

            var knownJobs = new HashSet<string>();
            if (isSinglePage)
                knownJobs.Add(job1!.StorageKey);

            await page.WaitForDOMIdle(new CancellationToken());

            Assert.IsTrue(await extractor!.GotoNextPage(loginResult.Data.Page, new CancellationToken()));
            var job2 = await extractor.ExtractNextJob(loginResult.Data.Page, knownJobs, new CancellationToken());

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
