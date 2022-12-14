namespace GreatJorb.Business.Services.WebSiteNavigators;

public class GoogleJobsNavigator : IWebSiteNavigator
{
    public string WebsiteName => "Google Jobs";

    public async Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        if(filter.WorkplaceTypeFilter != WorkplaceType.Unknown)
        {
            await page
                .GetElementByInnerText("span", "Location", cancellationToken)
                .ClickAsync();

            var workFromHomeButton = await page
                .GetElementByInnerText("span", "Work from Home", cancellationToken)
                .ParentElementAsync(page, cancellationToken)
                .ParentElementAsync(page, cancellationToken);

            if (workFromHomeButton != null)
            {
                bool isClicked = (await workFromHomeButton.GetBooleanAttribute("aria-pressed")).GetValueOrDefault();

                if (isClicked != (filter.WorkplaceTypeFilter == WorkplaceType.Remote))
                {
                    await workFromHomeButton.ClickAsync();

                    //need a better way to wait for results
                    await Task.Delay(2000);
                }
            }
        }

        return page;
    }

    public Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken)
    {      
        throw new NotImplementedException();
    }

    public async Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken)
    {
        var currentPage = page.Url;

        await page
          .GetElementByInnerText("a", "Sign In", cancellationToken)
          .ClickAsync();

        await page.WaitForNavigationFromAsync(currentPage);

        return await page.WaitForSelectorAsync("#identifierId");
    }

    public Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("google doesn't allow signing from automated browsers");
    }

    public async Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken)
    {
        var url = page.Url;

        await page
            .GetElementLabelledBy("Search", cancellationToken)
            .SetText(page, $"{query} jobs", pressEnter: true);

        await page.WaitForNavigationFromAsync(url);

        url = page.Url;
        await page
            .GetElementLabelledBy("Google interactive job search.", cancellationToken)
            .ClickAsync();

        await page.WaitForNavigationFromAsync(url);

        var jobCards = await page.QuerySelectorAllAsync("li.iFjolb");

        while(--pageNumber > 0)
        {
            await jobCards.Last().ClickAsync();

            var newJobCards = await TaskHelper.RepeatUntilCondition(
                createTask: () => page.QuerySelectorAllAsync("li.iFjolb"),
                condition: p => p.Length > jobCards.Length);

            if (newJobCards == null)
                break;

            jobCards = newJobCards;
        }

        return page;

    }

    public Task<bool> IsLoginRequired(IPage page)
    {
        return Task.FromResult(false);
    }

    public Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
