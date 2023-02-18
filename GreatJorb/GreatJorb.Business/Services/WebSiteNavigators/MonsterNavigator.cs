namespace GreatJorb.Business.Services.WebSiteNavigators;

public class MonsterNavigator : IWebSiteNavigator
{
    public Site Website => Site.Monster;

    public async Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        await page
                .GetElementByInnerText("button", "Filter", cancellationToken)
                .ClickAsync();

        await Task.Delay(500);

        await page
            .GetElementByInnerText("span", "All Job Types", cancellationToken)
            .ClickAsync();

        if(filter.WorkplaceTypeFilter == WorkplaceType.Remote)
        {
            await page
                .GetElementByInnerText("span", "Remote Jobs Only", cancellationToken)
                .ClickAsync();
        }

        if(filter.JobTypeFilter.HasFlag(JobType.FullTime))
        {
            await page
                .GetElementByInnerText("span", "Full-Time", cancellationToken)
                .ClickAsync();
        }

        if (filter.JobTypeFilter.HasFlag(JobType.PartTime))
        {
            await page
                .GetElementByInnerText("span", "Part-Time", cancellationToken)
                .ClickAsync();
        }

        if (filter.JobTypeFilter.HasFlag(JobType.Contract))
        {
            await page
                .GetElementByInnerText("span", "Contract", cancellationToken)
                .ClickAsync();
        }

        await page
              .GetElementByInnerText("button", "View Results", cancellationToken)
              .ClickAsync();

        return page;
    }

    public async Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken)
    {
        return await page.QuerySelectorAsync("button[type='submit']");
    }

    public async Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken)
    {
        await page
           .GetElementByInnerText("a", "Log In", cancellationToken)
           .ClickAsync();

        return await page.WaitForSelectorSafeAsync("input[type='email']", cancellationToken);
    }

    public async Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken)
    {
        return await page.WaitForSelectorSafeAsync("#password", cancellationToken);
    }

    public async Task<IPage> GotoJobsListPage(IPage page, string query, CancellationToken cancellationToken)
    {
        await page.GoToAsync($"https://www.monster.com/jobs/search?q={query.UrlEncode()}&where=remote");
        return page;
    }

    public async Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        var loginButton = await page
            .GetElementByInnerText("a", "Log In", cancellationToken);

        return loginButton != null;
    }

    public async Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var profile = page.GetElementLabelledBy("Profile", cancellationToken);
            if (profile != null)
                return;

            await Task.Delay(100);
        }
    }
}
