using System.IO;

namespace GreatJorb.Tests.Features
{
    public class ExtractKeywordLinesQueryTests
    {
        [TestCase("c#")]
        public async Task TestExtractKeywordLinesQuery(string query)
        {
            var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);

            var html = File.ReadAllText(TestContext.CurrentContext.TestDirectory + @"\TestData\samplehtml.txt");

            var result = await serviceProvider.Mediator.Send(new ExtractKeywordLinesQuery(query, html));

            Assert.IsNotEmpty(result);

            foreach(var line in result)
            {
                Assert.IsTrue(line.Contains(query, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
