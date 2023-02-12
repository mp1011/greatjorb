namespace GreatJorb.Business.Services.WebSiteNavigators;

public class DiceNavigator : IWebSiteNavigator
{
    private readonly ISettingsService _settingsService;
    public Site Website => Site.Dice;

    public DiceNavigator(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    private async Task ClearCurrentFilters(IPage page, CancellationToken cancellation)
    {
        var activeFilter = await page.QuerySelectorAsync("js-selected-filters-display button");
        while(activeFilter != null)
        {
            await activeFilter.ClickAsync();
            await Task.Delay(500);
            activeFilter = await page.QuerySelectorAsync("js-selected-filters-display button");
        }
    }

    public async Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        await page
            .GetElementByInnerText("span", "Filter Results", cancellationToken)
            .ClickAsync();

        await Task.Delay(2000);

        await ClearCurrentFilters(page, cancellationToken);

        string[] workplaceTypeFilters = filter.WorkplaceTypeFilter switch
        {
            WorkplaceType.Remote => new[] { "Remote Only" },
            WorkplaceType.OnSite => new[] { "Exclude Remote" },
            WorkplaceType.Hybrid => new[] { "Work From Home Available" },
            WorkplaceType.Hybrid | WorkplaceType.OnSite => new[] { "Exclude Remote", "Work From Home Available"},
            _ => Array.Empty<string>()
        };

        foreach(var filterText in workplaceTypeFilters)
        {
            await page.GetElementByInnerText("li", filterText, cancellationToken)
                .ClickAsync();
        }

        if(filter.JobTypeFilter.HasFlag(JobType.FullTime))
        {
            await page
                .GetElementByInnerText("li", "Full-time*", cancellationToken, wildCardMatch: true)
                .ClickAsync();
        }

        if (filter.JobTypeFilter.HasFlag(JobType.PartTime))
        {
            await page
                .GetElementByInnerText("li", "Part-time*", cancellationToken, wildCardMatch: true)
                .ClickAsync();
        }

        if (filter.JobTypeFilter.HasFlag(JobType.Contract))
        {
            await page
                .GetElementByInnerText("li", "Contract*", cancellationToken, wildCardMatch: true)
                .ClickAsync();
        }

        await page
            .QuerySelectorAsync(".cdk-overlay-backdrop")
            .ClickAsync();

        return page;
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

    public async Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken)
    {
        var country = _settingsService.Country;
        await page.GoToAsync($"https://www.dice.com/jobs?q={query.UrlEncode()}&location={country}");
        return page;
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
