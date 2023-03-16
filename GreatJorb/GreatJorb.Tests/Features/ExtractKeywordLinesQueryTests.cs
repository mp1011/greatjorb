namespace GreatJorb.Tests.Features;


public class ExtractKeywordLinesQueryTests
{
    [Category(TestType.UnitTest)]
    [TestCase("samplehtml.txt", "c#", null)]
    [TestCase("samplehtml_bullets.txt", "c#", null)]
    [TestCase("samplehtml_google.txt", "c#", "Expert (5 Years of recent hands on experience) in .Net, C# Expert (4 Years of recent hands on")]
    [TestCase("samplehtml_linkedin.html", "c#", "Demonstrated experience using C#, .NET Framework, JavaScript, SQL and SQL Server, Angular, ASP.NET MVC, and other .NET frameworks.")]
    [TestCase("sampledetail_google.html", "c#", "Good experience with Java, C#.net, Apache web server, Apache Tomcat, and Eloquence designs")]

    public async Task TestExtractKeywordLinesQuery(string file, string query, string expected)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);

        var html = File.ReadAllText(TestContext.CurrentContext.TestDirectory + @$"\TestData\{file}");

        var filter = new JobFilter { Query = query };
        var result = await serviceProvider.Mediator.Send(new ExtractKeywordLinesQuery(filter, html));

        Assert.IsNotEmpty(result);

        foreach (var line in result)
        {
            Assert.IsTrue(line.Line.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (expected != null)
        {
            Assert.IsTrue(result.Any(p => p.Line == expected));
        }
    }

    [Category(TestType.WebTest1)]

    [TestCase(Site.GoogleJobs, 
        "https://www.google.com/search?q=c%23+jobs&source=hp&ei=Q7_-Y5SeK9Cs5NoP67-G4A8&iflsig=AK50M_UAAAAAY_7NUzpQiDdwiZpTImdt-dCJPoxn2y2p&uact=5&oq=c%23+jobs&gs_lcp=Cgdnd3Mtd2l6EANQAFgZYB1oAHAAeACAAQCIAQCSAQCYAQCgAQE&sclient=gws-wiz&ibp=htl;jobs&sa=X&ved=2ahUKEwjh4POP3rn9AhV8MlkFHdIqAQwQutcGKAF6BAgIEAU#htivrt=jobs&fpstate=tldetail&htichips=&htischips=&htilrad=-1.0&htiltype=1&htidocid=Mz2OFxNrYKUAAAAAAAAAAA%3D%3D",
        "c#",
        "Deep experience in Microsoft Technology stack. C# a MUST")]

    [TestCase(Site.Indeed,
        "https://www.indeed.com/jobs?q=c%23&vjk=c72ea3f11447476d&advn=4850438377140069",
        ".net",
        "Assist in design and development of new solutions and enhance existing products utilizing .Net, T-SQL, SSIS and/or other back-end technologies.")]

    [TestCase(Site.Indeed,
        "https://www.indeed.com/jobs?q=c%23&vjk=c72ea3f11447476d&advn=4850438377140069",
        "c#",
        "Our current backend technology stack includes (but not limited to) Microsoft Azure, SQL Server, C#, SSIS.Responsibilities")]

    [TestCase(Site.Dice,
        "https://www.dice.com/job-detail/628f9c31-4c11-4902-b2f2-ceb41e64d492?searchlink=search%2F%3Fq%3Dc%2523%26countryCode%3DUS%26radius%3D30%26radiusUnit%3Dmi%26page%3D1%26pageSize%3D20%26filters.isRemote%3Dtrue%26language%3Den%26eid%3DS2Q_%2CgKQ_%2CkA_2&searchId=086974f0-92d0-458d-8b82-3ea7b2f0062a",
        "c#",
        "Job Title : Sr. C# SDET Developer")]
    public async Task TestExtractKeywordLinesQuery_Web(Site site, string url, string query, string firstExpected)
    {      
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
           includeConfiguration: true,
           includeMediator: true,
           includePuppeteer: true);

        var webSite = (await serviceProvider.Mediator.Send(new GetSitesQuery(site))).First();

        var extractor = await serviceProvider.Mediator.Send(new GetExtractorQuery(webSite));
        if(extractor == null)
        {
            Assert.IsNotNull(extractor);
            return;
        }

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var page = await serviceProvider.Mediator.Send(new BrowseToPageQuery(url));

        var html = await extractor
            .GetDescriptionElement(page, new CancellationToken())
            .GetInnerHTML();

        var result = await serviceProvider.Mediator.Send(new ExtractKeywordLinesQuery(new JobFilter {  Query=query }, html));

        Assert.AreEqual(firstExpected, result[0]);
    }
}
