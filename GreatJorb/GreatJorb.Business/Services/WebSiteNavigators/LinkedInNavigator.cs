﻿namespace GreatJorb.Business.Services.WebSiteNavigators;

public class LinkedInNavigator : IWebSiteNavigator
{
    private readonly IMediator _mediator;

    public LinkedInNavigator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Site Website => Site.LinkedIn;

    public async Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        await page
            .GetElementByInnerText("button", "All filters", cancellationToken)
            .ClickAsync();

        //how can we know that the results are loaded?
        await Task.Delay(2000);

        foreach(var workplaceTypeLabel in GetLabelsFor(filter.WorkplaceTypeFilter))
        {
            await page
                .GetElementLabelledBy(workplaceTypeLabel, cancellationToken)
                .ClickAsync();
        }

        await page
            .GetElementByInnerText("button", "show*results", cancellationToken, wildCardMatch: true)
            .ClickAsync();

        //need a better way to wait for results
        await Task.Delay(2000);

        return page;
    }

    public IEnumerable<string> GetLabelsFor(WorkplaceType workplaceType)
    {
        if (workplaceType.HasFlag(WorkplaceType.OnSite))
            yield return "On-Site";

        if (workplaceType.HasFlag(WorkplaceType.Remote))
            yield return "Remote";

        if (workplaceType.HasFlag(WorkplaceType.Hybrid))
            yield return "Hybrid";
    }

    public async Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken) =>
        await page.GetElementByInnerText("button","Sign in", cancellationToken);

    public async Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken) =>
        await page.GetElementLabelledBy("Email or phone", cancellationToken);

    public async Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken) =>
        await page.GetElementLabelledBy("Password", cancellationToken);

    public async Task<IPage> GotoJobsListPage(IPage page, string query, CancellationToken cancellationToken)
    {
        await page.GoToAsync($"https://www.linkedin.com/jobs/search/?keywords={query.UrlEncode()}");
        return page;
    }

    public async Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        var signedInElement = await page.WaitForSelectorAsync("img.global-nav__me-photo",
                new WaitForSelectorOptions { Timeout = 1000 })
                .DefaultIfError();

        return signedInElement == null;
    }

    public async Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await page.WaitForSelectorAsync("img.global-nav__me-photo",
                new WaitForSelectorOptions { Timeout = 5000 })
                .DefaultIfError();

            await _mediator.Publish(new BrowserPageChanged(page));

            if (result != null)
                return;
        }
    }
}
