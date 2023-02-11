namespace GreatJorb.Business.Services.WebSiteNavigators;

public class IndeedNavigator : IWebSiteNavigator
{
    public Site Website => Site.Indeed;

    public Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken)
    {
        return await page.GetElementByInnerText("button", "Sign in", cancellationToken);
    }

    public async Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken)
    {
        await page
            .GetElementByInnerText("a", "Sign In", cancellationToken)
            .ClickAsync();

        return await page.WaitForSelectorSafeAsync("input[type='email']", cancellationToken);
    }

    public async Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken)
    {
        await page
            .GetElementByInnerText("button","Continue", cancellationToken)
            .ClickAsync();

        if(await page.WaitForManualCaptcha(cancellationToken))
        {
            await page
                .GetElementByInnerText("button", "Continue", cancellationToken)
                .ClickAsync();
        }

        await Task.Delay(1000);

        await page
            .GetElementByInnerText("a", "Log in with a password instead", cancellationToken)
            .ClickAsync();

        return await page.WaitForSelectorSafeAsync("input[type='password']", cancellationToken);
    }

    public Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        var signinButton = await page
            .GetElementByInnerText("a","Sign In", cancellationToken);

        return signinButton != null;
    }

    public async Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        await page.WaitForSelectorSafeAsync("a[data-gnav-element-name='Notifications']", cancellationToken);
    }
}
