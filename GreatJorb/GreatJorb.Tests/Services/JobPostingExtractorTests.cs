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

    [TestCase("TestData/dice_sample.html")]
    [TestCase("TestData/dice_sample2.html")]

    public async Task CanExtractInfoFromDice(string samplePage)
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        FileInfo f = new FileInfo(samplePage);

        var extractor = new DiceJobPostingExtractor(serviceProvider.Mediator);
        var page = await serviceProvider.Mediator.Send(
            new BrowseToPageQuery(@$"file://{f.FullName}", DisableJavascript:true));
        
        var result = await extractor.ExtractJobDetail(
            page,
            new CancellationToken());

        Assert.IsNotNull(result);

        if (samplePage == "TestData/dice_sample.html")
        {
            Assert.AreEqual(125000, result.SalaryMin);
            Assert.AreEqual(150000, result.SalaryMax);
            Assert.AreEqual("Las Vegas, NV", result.Location);
            Assert.AreEqual("Cascade Financial Technology", result.Company);
            Assert.AreEqual(WorkplaceType.Remote, result.WorkplaceType);
        }
        else if(samplePage == "TestData/dice_sample2.html")
        {
            Assert.AreEqual("Senior Software Engineer (C# / Azure)", result.Title);
            Assert.AreEqual("Los Angeles, CA", result.Location);
            Assert.AreEqual(SalaryType.Hourly, result.SalaryType);
            Assert.AreEqual(75, result.SalaryMin);
            Assert.AreEqual(85, result.SalaryMax);
        }

        var keywordLines = await serviceProvider.Mediator.Send(
               new ExtractKeywordLinesQuery("c#", result.DescriptionHtml ?? ""));

        Assert.IsNotEmpty(keywordLines.Where(p => p.Contains("c#", StringComparison.OrdinalIgnoreCase)).ToArray());
    }

    [TestCase("TestData/indeed_sample.html")]

    public async Task CanExtractInfoFromIndeed(string samplePage)
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        FileInfo f = new FileInfo(samplePage);

        var extractor = new IndeedJobPostingExtractor(serviceProvider.Mediator);
        var page = await serviceProvider.Mediator.Send(
            new BrowseToPageQuery(@$"file://{f.FullName}", DisableJavascript: true));

        var result = await extractor.ExtractJobDetail(
            page,
            new CancellationToken());

        Assert.IsNotNull(result);

        Assert.Fail();

        //Assert.AreEqual(125000, result.SalaryMin);
        //Assert.AreEqual(150000, result.SalaryMax);
        //Assert.AreEqual("Las Vegas, NV", result.Location);
        //Assert.AreEqual("Cascade Financial Technology", result.Company);
        //Assert.AreEqual(WorkplaceType.Remote, result.WorkplaceType);
      
        //var keywordLines = await serviceProvider.Mediator.Send(
        //       new ExtractKeywordLinesQuery("c#", result.DescriptionHtml ?? ""));

        //Assert.IsNotEmpty(keywordLines.Where(p => p.Contains("c#", StringComparison.OrdinalIgnoreCase)).ToArray());
    }
}
