namespace GreatJorb.Tests.Services;

[Category(TestType.UnitTest)]
internal class JobPostingExtractorTests
{

    [TestCase("TestData/google.html")]
    public async Task CanExtractInfoFromGoogleJob(string samplePage)
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        FileInfo f = new FileInfo(samplePage);

        var extractor = new GoogleJobsExtractor(serviceProvider.Mediator);
        var page = await serviceProvider.Mediator.Send(new BrowseToPageQuery(@$"file://{f.FullName}"));

        var results = await extractor.ExtractJobsFromPage(
            page,
            1,
            new WebSite("Google", "https://www.google.com"),
            new CancellationToken(),
            new JobFilter(),
            PageSize: 3);

        Assert.AreNotEqual("https://none/", results[0].StorageKey);
        Assert.AreEqual(WorkplaceType.Remote, results[0].WorkplaceType);
        Assert.AreEqual(JobType.Contract, results[0].JobType);
        Assert.That(results[0]!.DescriptionHtml!.Contains("C#"));
        Assert.AreEqual("C# Developer", results[2].Title);

        var keywordLines = await serviceProvider.Mediator.Send(
                new ExtractKeywordLinesQuery("c#", results[0].DescriptionHtml ?? ""));

        Assert.AreEqual("Expert (5 Years of recent hands on experience) in .Net, C# Expert (4 Years of recent hands on",
            keywordLines[0]);
    }
}
