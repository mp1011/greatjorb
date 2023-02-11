namespace GreatJorb.Business.Services.WebSiteNavigators;

public class DiceNavigator : IWebSiteNavigator
{
    public Site Website => Site.Dice;

    public Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken)
    {
        return await page.GetElementByInnerText("button", "Sign In", cancellationToken);
    }

    public async Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken)
    {
        await page
            .GetElementByJavascript("document.querySelector('#cmpwrapper').shadowRoot.querySelector('#cmpwelcomebtnyes > a')")
            .ClickAsync();

        await page
            .GetElementLabelledBy("Toggle navigation", cancellationToken)
            .ClickAsync();

        await page
            .NavigateMenus(cancellationToken, "a", "Login/Register", "Login");

        return await page.WaitForSelectorSafeAsync("#email", cancellationToken);
    }

    public async Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken)
    {
        return await page.WaitForSelectorSafeAsync("#password", cancellationToken);
    }

    public Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        var loginButton = await page.GetElementByInnerText("a", "*Login*", cancellationToken,
            wildCardMatch: true,
            includeHidden: true);

        return loginButton != null;
    }

    public async Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        await page.WaitForSelectorSafeAsync(".personal-info-section", 
            cancellationToken, 
            retryUntilFound: true);
    }
}
