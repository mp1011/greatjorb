namespace GreatJorb.Business.Services.WebSiteNavigators;

public class IndeedNavigator : IWebSiteNavigator
{
    public Site Website => Site.Indeed;

    public async Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        await SetFilter(page,
            menuText: "Remote",
            selectionText: filter.WorkplaceTypeFilter switch
            {
                WorkplaceType.Remote => "Remote",
                _ => null,
            },
            clearFilterLabel: "Clear Remote filter",
            cancellationToken);

        await SetFilter(page,
            menuText: "Experience level",
            selectionText: filter.JobLevelFilter switch
            {
                JobLevel.SeniorLevel => "Senior Level",
                JobLevel.MidLevel => "Mid Level",
                JobLevel.EntryLevel => "Entry Level",
                _ => null,
            },
            clearFilterLabel: "Clear Experience level filter",
            cancellationToken);

        await SetFilter(page,
           menuText: "Experience level",
           selectionText: filter.JobTypeFilter switch
           {
               JobType.FullTime => "Full-Time",
               JobType.PartTime => "Part-Time",
               JobType.Contract => "Contract",
               _ => null,
           },
           clearFilterLabel: "Clear Job Type filter",
           cancellationToken);

        return page;
    }

    private async Task SetFilter(IPage page, string menuText, string? selectionText, string clearFilterLabel, CancellationToken cancellationToken)
    {
             await page
                .GetElementLabelledBy(clearFilterLabel, cancellationToken, repeatUntilFound:false)
                .ClickAsync();

        if (selectionText == null)
            return;

        await page
               .GetElementByInnerText("div.yosegi-FilterPill-pillLabel", "Remote", cancellationToken)
               .ClickAsync();

        await page
            .GetElementByInnerText("a.yosegi-FilterPill-dropdownListItemLink",
                selectionText + "*",
                cancellationToken,
                wildCardMatch: true);
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

    public async Task<IPage> GotoJobsListPage(IPage page, string query, CancellationToken cancellationToken)
    {
        await page.GoToAsync($"https://www.indeed.com/jobs?q={query.UrlEncode()}");
        return page;
    }

    public async Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        var signinButton = await page
            .GetElementByInnerText("a","Sign In", cancellationToken);

        return signinButton != null;
    }

    public async Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        await page.WaitForSelectorSafeAsync("a[data-gnav-element-name='Notifications']", 
            cancellationToken,
            retryUntilFound: true);
    }
}
