using System.Collections.Generic;

namespace GreatJorb.Tests.Services;

[Category(TestType.UnitTest)]
internal class JobPostingExtractorTests
{

    [TestCase("TestData/google.html")]
    public async Task CanExtractInfoFromGoogleJob(string samplePage)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        FileInfo f = new FileInfo(samplePage);

        var extractor = new GoogleJobsExtractor(serviceProvider.Mediator);
        var page = await serviceProvider.Mediator.Send(new BrowseToPageQuery(@$"file://{f.FullName}"));

        var result = await extractor.ExtractNextJob(
            page,
            new HashSet<string>(),
            cancellationToken: new CancellationToken());

        Assert.AreNotEqual("https://none/", result.StorageKey);
        Assert.AreEqual(WorkplaceType.Remote, result.WorkplaceType);
        Assert.AreEqual(JobType.Contract, result.JobType);
        Assert.That(result.DescriptionHtml!.Contains("C#"));

        var keywordLines = await serviceProvider.Mediator.Send(
                new ExtractKeywordLinesQuery("c#", result.DescriptionHtml ?? ""));

        Assert.AreEqual("Expert (5 Years of recent hands on experience) in .Net, C# Expert (4 Years of recent hands on",
            keywordLines[0]);
    }

    [TestCase("TestData/dice_sample.html")]
    [TestCase("TestData/dice_sample2.html")]

    public async Task CanExtractInfoFromDice(string samplePage)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
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
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
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

        Assert.AreEqual(90000, result.SalaryMin);
        Assert.AreEqual(140000, result.SalaryMax);
        Assert.AreEqual("Hauppauge, NY 11788", result.Location);
        Assert.AreEqual("Intelligent Product Solutions", result.Company);
        Assert.AreEqual(WorkplaceType.Hybrid, result.WorkplaceType);

        var keywordLines = await serviceProvider.Mediator.Send(
               new ExtractKeywordLinesQuery("c#", result.DescriptionHtml ?? ""));

        Assert.IsNotEmpty(keywordLines.Where(p => p.Contains("c#", StringComparison.OrdinalIgnoreCase)).ToArray());
    }

    [TestCase("TestData/samplehtml_monster.html")]

    public async Task CanExtractInfoFromMonster(string samplePage)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        FileInfo f = new FileInfo(samplePage);

        var extractor = new MonsterJobPostingExtractor(serviceProvider.Mediator);
        var page = await serviceProvider.Mediator.Send(
            new BrowseToPageQuery(@$"file://{f.FullName}", DisableJavascript: true));

        var result = await extractor.ExtractJob(
            page,
            @$"file://{f.FullName}",
            new CancellationToken());

        Assert.AreEqual(92436, result.SalaryMin);
        Assert.AreEqual(101956, result.SalaryMax);
        Assert.AreEqual("New York City, NY 10014", result.Location);
        Assert.AreEqual("Isospin Technologies, LLC", result.Company);
        Assert.AreEqual(JobType.FullTime, result.JobType);
    }
}
