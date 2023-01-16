namespace GreatJorb.Tests.Features;

[Category(TestType.UnitTest)]
public class ExtractKeywordLinesQueryTests
{
    [TestCase("samplehtml.txt", "c#", null)]
    [TestCase("samplehtml_bullets.txt", "c#", null)]
    [TestCase("samplehtml_google.txt", "c#", "Expert (5 Years of recent hands on experience) in .Net, C# Expert (4 Years of recent hands on")]
    [TestCase("samplehtml_linkedin.txt", "c#", "ddd")]

    public async Task TestExtractKeywordLinesQuery(string file, string query, string firstExpected)
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);

        var html = File.ReadAllText(TestContext.CurrentContext.TestDirectory + @$"\TestData\{file}");

        var result = await serviceProvider.Mediator.Send(new ExtractKeywordLinesQuery(query, html));

        Assert.IsNotEmpty(result);

        foreach(var line in result)
        {
            Assert.IsTrue(line.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (firstExpected != null)
            Assert.AreEqual(firstExpected, result[0]);
    }
}
