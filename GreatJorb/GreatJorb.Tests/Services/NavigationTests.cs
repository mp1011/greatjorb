namespace GreatJorb.Tests.Services;


[Category(TestType.WebTest3)]
public class NavigationTests
{
    [TestCase("LinkedIn", "https://www.linkedin.com/", "c#")]
    public async Task CanNavigateToPageTwoOfResults(string name, string url, string query)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeMediator: true,
            includePuppeteer: true);

        var webSite = new WebSite(name, url);

        var loggedInPage = await serviceProvider.Mediator.Send(
            new LoginQuery(webSite));

        var navigator = await serviceProvider.Mediator.Send(new GetNavigatorQuery(webSite));
        if (navigator == null || loggedInPage?.Data?.Page == null)
        {
            Assert.Fail();
            return;
        }

        await navigator.GotoJobsListPage(loggedInPage.Data.Page, query, 2, new CancellationToken());

        //unsure how to verify this but at least check we didn't crash
    }
}
