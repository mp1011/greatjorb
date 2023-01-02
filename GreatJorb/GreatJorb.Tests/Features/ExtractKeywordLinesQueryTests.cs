using System.IO;

namespace GreatJorb.Tests.Features
{
    public class ExtractKeywordLinesQueryTests
    {
        [TestCase("samplehtml.txt", "c#")]
        [TestCase("samplehtml_bullets.txt", "c#")]

        public async Task TestExtractKeywordLinesQuery(string file, string query)
        {
            var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);

            var html = File.ReadAllText(TestContext.CurrentContext.TestDirectory + @$"\TestData\{file}");

            var result = await serviceProvider.Mediator.Send(new ExtractKeywordLinesQuery(query, html));

            Assert.IsNotEmpty(result);

            foreach(var line in result)
            {
                Assert.IsTrue(line.Contains(query, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
